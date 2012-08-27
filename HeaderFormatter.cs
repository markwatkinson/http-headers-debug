using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTP
{
    class HeaderFormatter
    {
        private string _headers = null;

        public int StatusCode = -1;
        public string StatusText = null;
        public string HTTPVersion = null;

        public bool IsOk = false;
        public bool IsRedirect = false;
        public bool IsError = false;

        private Dictionary<string, string> _headersDict = new Dictionary<string, string>();


        public List<string> Information;

        public HeaderFormatter(string headers)
        {
            Information = new List<string>();
            _headers = headers;

            foreach (string line in _headers.Split(System.Environment.NewLine.ToCharArray()))
            {
                string[] fieldVal = line.Split(":".ToCharArray(), 2);
                if (fieldVal.Length == 2)
                {
                    _headersDict[fieldVal[0].Trim().ToLower()] = fieldVal[1].Trim();
                }
            }
        }


        /// <summary>
        /// Extracts HTTP version and status code
        /// </summary>
        private void ExtractHTTPInformation()
        {
            Match m = Regex.Match(_headers, @"^HTTP/([^\s]+)\s+(\d+)(.*)");
            if (m.Success)
            {
                HTTPVersion = m.Groups[1].Value;
                StatusText = m.Groups[3].Value.Trim();
                StatusCode = System.Convert.ToInt32(m.Groups[2].Value);
            }
        }

        private void ParseRedirect()
        {
            if (!_headersDict.ContainsKey("location"))
            {
                return;
            }

            string redirText = null;
            if (StatusCode == 301 || StatusCode == 308)
                redirText = "permanently";
            else if (StatusCode == 302 || StatusCode == 303 || StatusCode == 307)
                redirText = "temporarily";
            Information.Add("The resource has been " + (redirText ?? "") +
                    " redirected to " + _headersDict["location"]);
        }

        private void ParseOk()
        {
            if (StatusCode == 206)
            {
                string range = _headersDict["content-range"];
                string type = null;
                int lower = -1, upper = -1;
                //FIXME this is a bit flakey, it probably shouldn't require the whole line
                // to catch the first groups
                Match match = Regex.Match(range, @"^\s*([^\s]+)\s+(\d+\s*)-(\d+\s*)/(\d+)");
                if (match.Success)
                {
                    type = match.Groups[1].Value;
                    lower = System.Convert.ToInt32(match.Groups[2].Value);
                    upper = System.Convert.ToInt32(match.Groups[3].Value);
                }

                string text = "The server returned a " + (type ?? "data") + " range";
                if (lower > -1 && upper > -1)
                {
                    text += " from " + lower + " to " + upper;
                }

                Information.Add(text);
            }

            if (_headersDict.ContainsKey("content-type"))
            {
                string contentType = _headersDict["content-type"];
                if (contentType.ToLower().StartsWith("text/html"))
                {
                    Information.Add("The server returned an HTML page");
                }
                else
                {
                    Information.Add("The returned a " + contentType + " file");
                }
            }

            if (_headersDict.ContainsKey("content-length"))
            {
                Information.Add("The data is " + _headersDict["content-length"] + " bytes long");
            }
        }

        public void Parse()
        {

            ExtractHTTPInformation();

            IsOk = StatusCode >= 200 && StatusCode < 300;
            IsRedirect = StatusCode >= 300 && StatusCode < 400;
            IsError = StatusCode >= 400;


            if (IsRedirect) ParseRedirect();
            if (IsOk) ParseOk();

        }
    }
}
