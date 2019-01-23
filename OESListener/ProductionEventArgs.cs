using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class ProductionEventArgs : OesEventArgs
    {
        public string ItemID { get; set; }
        public string GeneratedBarcode { get; set; }
        public List<string> ProcessHistoryValues = new List<string>();

        public ProductionEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
        }
        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, string requestType, string status, string failureCode, string[] data)
        {
            Client = client;
            CellID = cellID;
            ItemID = itemID;
            GeneratedBarcode = "";
            Request = requestType;
            Status = status;
            FailureCode = failureCode;
            ProcessHistoryValues = new List<string>(data);
        }

        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, string generatedBarcode, string requestType, string status, string failureCode, string[] data)
        {
            Client = client;
            CellID = cellID;
            ItemID = itemID;
            GeneratedBarcode = generatedBarcode;
            Request = requestType;
            Status = status;
            FailureCode = failureCode;
            ProcessHistoryValues = new List<string>(data);
        }
    }
}
