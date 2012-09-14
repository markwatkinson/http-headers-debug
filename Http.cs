using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace HTTP
{
    public partial class Http : Form
    {
        /// <summary>
        /// Worker encapsulation class
        /// </summary>
        Request req;

        /// <summary>
        /// Background worker/thread for HTTP requests
        /// </summary>
        BackgroundWorker worker;

        /// <summary>
        /// semaphore lock inside the main UI thread to ensure only one request is 
        /// made at once
        /// </summary>
        bool requestLock = false;

        /// <summary>
        /// Semaphore lock for main UI thread updating headers input and URL input
        /// as these both trigger parsing/updating of each other
        /// </summary>
        bool textUpdateLock = false;

        public Http()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            errorLbl.Text = "";
            otherInformationLbl.Text = "";
            httpVersionLbl.Text = "";
            statusCodeLbl.Text = "";
            InputTxt.Text = @"GET / HTTP/1.1
Host: example.org
User-Agent: HTTPHeadersDebug
Accept: text/html, application/xml;q=0.9, application/xhtml xml, image/png, image/jpeg, image/gif, image/x-xbitmap, */*;q=0.1
Accept-Language: en
Accept-Charset: iso-8859-1, utf-8, utf-16, *;q=0.1
Connection: Keep-Alive";

            InputTxt.KeyDown += new KeyEventHandler(InputTxt_KeyDown);
        }

        void InputTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        /// <summary>
        /// Returns whether or not the given string is a valid HTTP method, 
        /// e.g. GET or POST
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool isHTTPMethod(string method)
        {
            string[] methods = new string[] {
                        "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", "PATCH"
                    };
            return methods.Contains(method.ToUpper()); 
        }


        /// <summary>
        /// Extracts the HOST field of the HTTP headers
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns>True if successful, false otherwise (in which case things will be null)</returns>
        private bool extractHostNameAndPortAndPath(string headers, out string hostname, out int port, out string path)
        {
            hostname = null;
            port = 80;
            path = null;
            // the host/port should be in the headers
            Match hostMatch = Regex.Match(headers, "^Host:([^:\r\n]*)(:(\\d+))?",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            if (!hostMatch.Success)
            {                
                return false;
            }

            int gLen = hostMatch.Groups.Count;
          
            hostname = hostMatch.Groups[1].Value.Trim();

            // https default port is 443
            if (hostname.ToLower().StartsWith("https"))
            {
                port = 443;
            }
            // if we fail to convert the port we can just break and leave it as 80/443
            if (gLen > 2)
            {
                try
                {
                    port = System.Convert.ToInt32(hostMatch.Groups[3].Value);
                }
                catch (Exception)
                {
                }
            }

            // the path

            var lines = headers.Split(new[] { Environment.NewLine },
                                     StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                string top = lines[0];
                var segments = top.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 1 && isHTTPMethod(segments[0]))
                {
                    path = segments[1];
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the request URL. The appropriate sections of the headers input
        /// are updated or created
        /// </summary>
        /// <param name="url"></param>
        private bool SetHeadersForUrl(string url)
        {
            string hostname = null, path = null;
            int port = 80;
            bool https = false;
            Match urlSectionMatch = Regex.Match(url, 
                @"((?:https?://)?)([^/:]+)((:(\d+))?)((/.*)?)", 
                RegexOptions.IgnoreCase);
            if (!urlSectionMatch.Success)
            {
                return false;
            }
            if (urlSectionMatch.Groups[1].Value.ToLower().StartsWith("https"))
            {
                port = 443;
                https = true;
            }
            hostname = urlSectionMatch.Groups[2].Value.Trim();
            path = urlSectionMatch.Groups[6].Value.Trim();
            if (path.Length == 0) path = "/";
            try
            {
                port = System.Convert.ToInt32(urlSectionMatch.Groups[5].Value);
            }
            catch (Exception)
            {
                // leave port as 80/443
            }           

            // now we've got everything we can start subbing it into the textbox

            string orig = InputTxt.Text.Trim();
            string repl;
            string hostReplacement = "Host: " + hostname;
            if (port != 80) hostReplacement += ":" + port;
            bool isPathSet = false;
            bool isHostSet = false;


            List<string> lines = orig.Split(new[] { Environment.NewLine },  
                                     StringSplitOptions.RemoveEmptyEntries).ToList();
            // Handle the path part of the URL.
            // we expect to find this in the first line
            if (lines.Count > 0)
            {
                string firstLine = lines[0].Trim();

                // we'll split it up the old fashioned way
                string[] segments = firstLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 3) 
                {
                    string[] methods = new string[] {
                        "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", "PATCH"
                    };
                    if (methods.Contains(segments[0].ToUpper())) 
                    {
                        segments[1] = path;
                        int lastSegmentIndex = segments.Length - 1;
                        if (https && lines[lastSegmentIndex].IndexOf("HTTPS") < 0)
                        {
                            lines[lastSegmentIndex] = lines[lastSegmentIndex].Replace("HTTP", "HTTPS");
                        }
                        lines[0] = String.Join(" ", segments);
                        isPathSet = true;
                    }
                }
            }
            if (!isPathSet)
            {
                lines.Insert(0, "GET " + path + " HTTP" + (https? "S" : "") + "/1.1");
            }

            // now we'll find the host line and replace it
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("Host:"))
                {
                    lines[i] = hostReplacement;
                    isHostSet = true;
                    break;
                }
            }
            if (!isHostSet) {
                // no replacement made
                // we will put this on the second line since we've already set the first 
                // and therefore know it exists
                lines.Insert(1, hostReplacement);                
            }
            repl = String.Join("\r\n", lines);
            InputTxt.Text = repl;
            return true;
        }


        private void GoBtn_Click(object sender, EventArgs e)
        {
            string headers = InputTxt.Text;
            string hostname;
            string path;
            int port;

            if (requestLock)
            {
                worker.CancelAsync();
                return;
            }            

            if (!extractHostNameAndPortAndPath(headers, out hostname, out port, out path)) 
            {
                errorLbl.Text = "Could not find HOST field or path";
                return;
            }

            headers = headers.Trim();
            headers += "\r\n\r\n";

            errorLbl.Text = "";
            OutputTxt.Text = "";

            requestLock = true;
            GoBtn.Text = "Cancel";

            req = new Request() {
                hostname = hostname,
                port = port,
                headers = headers,
                errors = new List<string>(),
                worker = worker
                
            };

            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            req.doWorkEventArgs = e;
            req.response = req.GetResponse();
        }


        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                OutputTxt.Text = "Cancelled";
            }

            else if (req.errors.Count > 0)
            {
                OutputTxt.Text = "";
                errorLbl.Text = String.Join("\r\n", req.errors);
            }
            else
            {
                FormatOutput();
            }
            GoBtn.Text = "Go";
            requestLock = false;
        }


        private void FormatOutput()
        {
            string response = req.response;
            string responseHeaders;
            int index = response.IndexOf("\r\n\r\n");
            if (index >= 0)
                responseHeaders = response.Substring(0, index);
            else
                responseHeaders = response;
            OutputTxt.Text = responseHeaders;

            HeaderFormatter formatter = new HeaderFormatter(responseHeaders);
            formatter.Parse();
            if (formatter.IsOk)
                statusCodeLbl.ForeColor = Color.Green;
            else if (formatter.IsError)
                statusCodeLbl.ForeColor = Color.Red;
            else if (formatter.IsRedirect) 
                statusCodeLbl.ForeColor = Color.Orange;
            statusCodeLbl.Text = formatter.StatusCode + " (" + formatter.StatusText + ")";
            httpVersionLbl.Text = formatter.HTTPVersion;
            otherInformationLbl.Text = String.Join("\r\n", formatter.Information);
        }


        private void InputTxt_TextChanged(object sender, EventArgs e)
        {
            if (textUpdateLock) return;
            textUpdateLock = true;
            TextBox tbox = (TextBox)sender;
            string hostname, path;
            int port;
            if (extractHostNameAndPortAndPath(tbox.Text, out hostname, out port, out path))
            {
                hostLbl.Text = hostname + ":" + port + path;
                requestUrlTxt.Text = hostname + ":" + port + path;
            }
            else
            {
                hostLbl.Text = "";
                requestUrlTxt.Text = "";
            }
            textUpdateLock = false;
        }

        private void requestUrlTxt_TextChanged(object sender, EventArgs e)
        {
            if (textUpdateLock) return;
            textUpdateLock = true;
            string url = ((TextBox)sender).Text.Trim();
            url = url.Replace(" ", "%20");
            SetHeadersForUrl(url);
            textUpdateLock = false;
        }
    }
}