using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class SerialRequestEventArgs : OesEventArgs
    {

        public new string ItemId {
            get
            {
                return Util.AbIntArrayToString(ResponseArray);
            }
            set
            {
                ItemId = value;
            }
        }

        public SerialRequestEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
            ResponseArray = new short[12];
        }

        public SerialRequestEventArgs(string senderIp)
        {
            SenderIp = senderIp;
            ResponseArray = new short[12];
        }

        public SerialRequestEventArgs(System.Net.Sockets.TcpClient client, string itemId)
        {
            Client = client;
            ItemId = itemId;
            ResponseArray = new short[12];
        }
    }
}
