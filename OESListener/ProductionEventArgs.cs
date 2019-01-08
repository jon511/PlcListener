using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{

    public class ProcessHistoryReturn
    {
        public string CellID { get; set; }
        public string ItemID { get; set; }
        public string GeneratedBarcode { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public string FailureCode { get; set; }

        public string[] ProcessHistoryValues = new string[14];
    }

    public class ProductionEventArgs : OesEventArgs
    {
        //public bool UseJson { get; set; }
        //public System.Net.Sockets.TcpClient Client { get; set; }
        public ProcessHistoryReturn Data = new ProcessHistoryReturn();
        public ProductionEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }
        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, string requestType, string status, string failureCode, string[] data)
        {
            Client = client;
            Data.CellID = cellID;
            Data.ItemID = itemID;
            Data.GeneratedBarcode = "";
            Data.RequestType = requestType;
            Data.Status = status;
            Data.FailureCode = failureCode;
            Data.ProcessHistoryValues = data;
        }

        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, string generatedBarcode, string requestType, string status, string failureCode, string[] data)
        {
            Client = client;
            Data.CellID = cellID;
            Data.ItemID = itemID;
            Data.GeneratedBarcode = generatedBarcode;
            Data.RequestType = requestType;
            Data.Status = status;
            Data.FailureCode = failureCode;
            Data.ProcessHistoryValues = data;
        }
    }
}
