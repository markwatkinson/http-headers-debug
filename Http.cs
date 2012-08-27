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
            // if we fail to convert the port we can just break and leave it as 80
            if (gLen > 2)
            {
                try
                {
                    port = System.Convert.ToInt32(hostMatch.Groups[3].Value);
                }
                catch (Exception ex)
                {
                }
            }

            // the path
            Match m = Regex.Match(headers.Trim(), @"^[\S]+\s+([\S]+)");
            if (m.Success)
            {
                path = m.Groups[1].Value;
            }
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
            TextBox tbox = (TextBox)sender;
            string hostname, path;
            int port;
            if (extractHostNameAndPortAndPath(tbox.Text, out hostname, out port, out path))
            {
                hostLbl.Text = hostname + ":" + port + path;
            }
            else
            {
                hostLbl.Text = "";
            }
        }
    }
}