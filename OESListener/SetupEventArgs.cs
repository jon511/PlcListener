using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public struct BomModelData
    {
        public string AccessId;
        public string ModelNumber;

    }

    public class ReturnData
    {
        public BomModelData Component1;
        public BomModelData Component2;
        public BomModelData Component3;
        public BomModelData Component4;
        public BomModelData Component5;
        public BomModelData Component6;
        public string Quantity;
        public string Acknowledge;
        public string ErrorCode;

        public string[] PlcModelSetup = new string[34];
        
    }

    public class SetupEventArgs : OesEventArgs
    {
        public string Component { get; set; }
        public string AccessId { get; set; }
        public string ModelNumber { get; set; }
        public string OpNumber { get; set; }

        public ReturnData Response = new ReturnData();
        
        public SetupEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }

        public SetupEventArgs(System.Net.Sockets.TcpClient client, string cellID, string transactionRequest, string modelNumber, string opNumber)
        {
            Client = client;
            CellID = cellID;
            Request = transactionRequest;
            ModelNumber = modelNumber;
            OpNumber = opNumber;

        }

        public SetupEventArgs(System.Net.Sockets.TcpClient client, string cellID, string component, string accessId, string transactionRequest, string modelNumber, string opNumber)
        {
            Client = client;
            CellID = cellID;
            Component = component;
            AccessId = accessId;
            Request = transactionRequest;
            ModelNumber = modelNumber;
            OpNumber = opNumber;
        }


    }
}
