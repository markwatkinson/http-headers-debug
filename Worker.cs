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

            IPHostEntry hostEntry;
            try
            {
                hostEntry = Dns.GetHostEntry(hostname);
            }
            catch (Exception ex)
            {
                WorkerError(String.Format("DNS lookup for {0} failed", hostname));
                return null;
            }
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint endPoint = new IPEndPoint(address, port);
                Socket s = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
                WorkerError(String.Format("Socket connection failed to {0}:{1}", hostname, port));
                return null;
            }
            Byte[] send, receive = new Byte[1024 * 100];
            send = Encoding.ASCII.GetBytes(headers);
            s.Send(send, send.Length, 0);
            int numBytes = 0;
            string response = "";
            do
            {
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
                }
                response += Encoding.ASCII.GetString(receive, 0, numBytes);
            } while (numBytes == receive.Length);
            return response;
        }
    }
}
