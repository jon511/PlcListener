using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{

    public class LoginEventArgs : OesEventArgs
    {
        public string OperatorID { get; set; }

        public LoginEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }

        public LoginEventArgs(string senderIp)
        {
            SenderIp = senderIp;
        }

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, short processRequest)
        {
            Client = client;
            CellId = cellID;
            ProcessIndicator = processRequest;
        }

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, string operatorID, short processRequest)
        {
            Client = client;
            CellId = cellID;
            OperatorID = operatorID;
            ProcessIndicator = processRequest;
        }

    }
}
