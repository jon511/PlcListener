using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OESListener
{
    public class TcpClientWithTimeout
    {
        protected string _hostName;
        protected int _port;
        protected int _timeOutMilliseconds;
        protected TcpClient connection;
        protected bool connected;
        protected Exception exception;

        public TcpClientWithTimeout(string hostName, int port, int timeOutMilliseconds)
        {
            _hostName = hostName;
            _port = port;
            _timeOutMilliseconds = timeOutMilliseconds;
        }

        public TcpClient Connect()
        {
            connected = false;
            exception = null;
            Thread thread = new Thread(new ThreadStart(BeginConnect));
            thread.IsBackground = true;
            thread.Start();
            thread.Join(_timeOutMilliseconds);

            if (connected == true)
            {
                thread.Abort();
                return connection;
            }

            if (exception != null)
            {
                thread.Abort();
                throw exception;
            }
            else
            {
                thread.Abort();
                string message = string.Format("TcpClient connection to {0}:{1} timed out", _hostName, _port);
                throw new TimeoutException(message);
            }
        }

        protected void BeginConnect()
        {
            try
            {
                connection = new TcpClient(_hostName, _port);
                connected = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }
    }
}
