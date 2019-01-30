using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class LabelPrintEventArgs : OesEventArgs
    {
        public string Weight { get; set; }
        public string PrinterIpAddress { get; set; }
        public string RevLevel { get; set; }
        public string AlphaCode { get; set; }
        public string PrintCode { get; set; }
        public string Response { get; set; }
        public LabelPrintEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }

        public LabelPrintEventArgs(string senderIp)
        {
            SenderIp = senderIp;
        }
    }
}
