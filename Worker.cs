using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace HTTP
{
    /// <summary>
    /// Information for HTTP request
    /// </summary>
    public class Request
    {
        public string hostname;
        public int port;
        public string headers;
        public string response;

        private bool errored = false;
        /// <summary>
        /// A set of errors encountered by the worker
        /// </summary>
        public List<string> errors;

        /// <summary>
        /// Reference to the worker invoked by the main thread
        /// </summary>
        public BackgroundWorker worker;

        /// <summary>
        /// Reference to the Do Work event args for this worker
        /// </summary>
        public DoWorkEventArgs doWorkEventArgs;

        public void WorkerError(string message)
        {
            errors.Add(message);
            errored = true;
        }

        /// <summary>
        /// Checks to see if the main thread has requested the worker thread
        /// to be cancelled
        /// </summary>
        /// <returns></returns>
        public bool CheckCancel()
        {
            if (worker.CancellationPending)
            {
                doWorkEventArgs.Cancel = true;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Gets a socket to the given Host and IP.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private Socket GetSocket(string hostname, int port = 80)
        {
            IPAddress ip;
            IPAddress[] ipList;
            // the 'fail safe' method of using the GetHostEntry method
            // is incredibly slow sometimes, so let's avoid it if we can
            // i.e. if the hostname is already an IP address

            if (IPAddress.TryParse(hostname, out ip)) 
            {
                ipList = new IPAddress[] { ip };
            } 
            else 
            {
                IPHostEntry hostEntry;
                try
                {
                    hostEntry = Dns.GetHostEntry(hostname);
                }
                catch (Exception)
                {
                    WorkerError(String.Format("DNS lookup for {0} failed", hostname));
                    return null;
                }
                ipList = hostEntry.AddressList;
            }


            foreach (IPAddress address in ipList)
            {
                IPEndPoint endPoint = null;
                Socket s = null;

                try
                {
                    endPoint = new IPEndPoint(address, port);
                    // the above should throw for bad port numbers
                    // except port 0 which throws later. We'll force-throw it
                    // here for convenience.
                    if (port <= 0)
                    {
                        throw new Exception();
                    }
                   
                }
                catch (Exception)
                {                    
                    WorkerError(String.Format("Failed to get endpoint for {0}:{1}, " +
                        "is this a valid address/port?", address, port));
                    return null;
                }
                s = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = s.BeginConnect(endPoint, null, null);
                // split this up so we can check for cancellations without hanging
                // until the timeout hits
                for (int i = 0; i < 20; i++)
                {
                    if (CheckCancel())
                    {
                        break;
                    }
                    bool success = result.AsyncWaitHandle.WaitOne(250, true);
                    if (s.Connected)
                    {
                        break;
                    }
                }


                if (s.Connected)
                {
                    s.ReceiveTimeout = 1000;
                    s.SendTimeout = 1000;
                    s.NoDelay = true;
                    return s;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an HTTP response to the given Host and IP given the 
        /// headers
        /// </summary>
        public string GetResponse()
        {
            Socket s = GetSocket(hostname, port);
            if (s == null)
            {
                if (!errored)
                    WorkerError(String.Format("Socket connection failed to {0}:{1}", hostname, port));
                return null;
            }
            Byte[] send, receive = new Byte[1024 * 100];
            send = Encoding.ASCII.GetBytes(headers);
            s.Send(send, send.Length, 0);
            int numBytes = 0;
            string response = "";


            // XXX: this might be a bug,
            // we check for numbytes > 0, but can a server send a message to say
            // 'no more data'? if so, then we loop infinitely (user can interrupt though)
            // If we check for numBytes == receve.Length then this can interrupt too
            // soon if the server doesn't send it all at once (example - Werkzeug).
            // As all we're interested in right now is the headers, we'll break after
            // we've received them so we don't need to worry about the body.
            // In future we might need to parse the headers and grab the content length
            // and check for chunked encoding
            bool recvHeaders = false;
            do
            {
                string r;
                if (CheckCancel())
                {
                    break;
                }
                try
                {
                    numBytes = s.Receive(receive, receive.Length, 0);
                }
                catch (Exception ex)
                {
                    WorkerError("Socket exception: " + ex.Message);
                    break;
                }
                r = Encoding.ASCII.GetString(receive, 0, numBytes);
                response += r;
                recvHeaders = response.IndexOf("\r\n\r\n") >= 0;
            } while (numBytes > 0 && !recvHeaders);
            return response;
        }
    }
}
