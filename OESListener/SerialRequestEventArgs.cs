using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class SerialRequestEventArgs : OesEventArgs
    {
        public string ItemID { get; set; }

        public SerialRequestEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }

        public SerialRequestEventArgs(string senderIp)
        {
            SenderIp = senderIp;
        }

        public SerialRequestEventArgs(System.Net.Sockets.TcpClient client, string itemId)
        {
            Client = client;
            ItemID = itemId;
        }
    }
}
