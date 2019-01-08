using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public struct LoginResponse
    {
        public string Status;
        public string FaultCode;
    }

    public class LoginEventArgs : OesEventArgs
    {
        public string OperatorID { get; set; }
        public string ProcessRequest { get; set; }
        public LoginResponse Response;

        public LoginEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, string processRequest)
        {
            Client = client;
            CellID = cellID;
            ProcessRequest = processRequest;
        }

        public LoginEventArgs(System.Net.Sockets.TcpClient client, string cellID, string operatorID, string processRequest)
        {
            Client = client;
            CellID = cellID;
            OperatorID = operatorID;
            ProcessRequest = processRequest;
        }

    }
}
