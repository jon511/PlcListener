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

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, string processRequest)
        {
            Client = client;
            CellId = cellID;
            Request = processRequest;
        }

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, string operatorID, string processRequest)
        {
            Client = client;
            CellId = cellID;
            OperatorID = operatorID;
            Request = processRequest;
        }

    }
}
