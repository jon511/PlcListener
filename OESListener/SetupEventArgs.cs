using System;
using System.Collections.Generic;

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
        public short AccessId { get; set; }
        public string ModelNumber { get; set; }
        public string OpNumber { get; set; }

        public short[] PlcModelSetup { get; set; }

        public ReturnData Response = new ReturnData();
        
        public SetupEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
            ResponseArray = new short[70];
            PlcModelSetup = new short[34];
        }

        public SetupEventArgs(string senderIp)
        {
            SenderIp = senderIp;
            ResponseArray = new short[70];
            PlcModelSetup = new short[34];
        }

        public SetupEventArgs(System.Net.Sockets.TcpClient client, string cellID, short transactionRequest, string modelNumber, string opNumber)
        {
            Client = client;
            CellId = cellID;
            ProcessIndicator = transactionRequest;
            ModelNumber = modelNumber;
            OpNumber = opNumber;
            ResponseArray = new short[70];
            PlcModelSetup = new short[34];

        }

        public SetupEventArgs(System.Net.Sockets.TcpClient client, string cellID, string component, short accessId, short transactionRequest, string modelNumber, string opNumber)
        {
            Client = client;
            CellId = cellID;
            Component = component;
            AccessId = accessId;
            ProcessIndicator = transactionRequest;
            ModelNumber = modelNumber;
            OpNumber = opNumber;
            ResponseArray = new short[70];
            PlcModelSetup = new short[34];
        }

        internal void ParseReturnData()
        {
            var tempArr = new short[10];
            Response.Component1.AccessId = ResponseArray[0].ToString();
            Array.Copy(ResponseArray, 1, tempArr, 0, 10);
            Response.Component1.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Component2.AccessId = ResponseArray[11].ToString();
            Array.Copy(ResponseArray, 12, tempArr, 0, 10);
            Response.Component2.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Component3.AccessId = ResponseArray[22].ToString();
            Array.Copy(ResponseArray, 23, tempArr, 0, 10);
            Response.Component3.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Component4.AccessId = ResponseArray[33].ToString();
            Array.Copy(ResponseArray, 34, tempArr, 0, 10);
            Response.Component4.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Component5.AccessId = ResponseArray[44].ToString();
            Array.Copy(ResponseArray, 45, tempArr, 0, 10);
            Response.Component5.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Component6.AccessId = ResponseArray[55].ToString();
            Array.Copy(ResponseArray, 56, tempArr, 0, 10);
            Response.Component6.ModelNumber = Util.AbIntArrayToString(tempArr);

            Response.Quantity = ((ResponseArray[66] * 10000) + ResponseArray[65]).ToString();

            Response.Acknowledge = ResponseArray[67].ToString();
            Response.ErrorCode = ResponseArray[68].ToString();

            var tempPlcModel = new List<string>();

            foreach (var item in PlcModelSetup)
            {
                tempPlcModel.Add(item.ToString());
            }

            Response.PlcModelSetup = tempPlcModel.ToArray();
        }


    }
}
