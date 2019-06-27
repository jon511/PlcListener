using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OesWriter
{
    public class ProductionTransaction
    {
        private string Command { get; set; }
        public string CellId { get; set; }
        public string ItemId { get; set; }
        public string GeneratedBarcode { get; set; }
        public int RequestCode { get; set; }
        public int Status { get; set; }
        public int FailureCode { get; set; }
        public string[] ProcessHistoryValues { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string ResponseString { get; set; }

        public ProductionTransaction()
        {
            Command = "PROD";
            GeneratedBarcode = "";
            RequestCode = 0;
            Status = 0;
            FailureCode = 0;
            ProcessHistoryValues = new string[15];

            for (var i = 0; i < 15; i++)
            {
                ProcessHistoryValues[i] = string.Format("{0}", 0);
            }
            
        }

        private void SendTransaction(string ipAddress)
        {
            var port = 55001;
            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, port);
            }
            catch
            {
                ResponseString = "Could not connect to target";
                return;
            }
            

            var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var stream = client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);

            var bytes = new byte[1024];
            client.ReceiveTimeout = 5000;
            try
            {
                var i = stream.Read(bytes, 0, bytes.Length);
                var receivedBytes = new byte[i];
                Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);
                var inString = Encoding.Default.GetString(receivedBytes).Trim();
                ResponseString = inString;
            }
            catch 
            {
                ResponseString = "Receive timeout";
            }

            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();

        }

        public string SendCsvTransaction(string ipAddress)
        {
            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, 55001);
            }
            catch
            {
                return "Could not connect to target";
            }
            var sb = new StringBuilder();
            sb.Append(Command + ",");
            sb.Append(CellId + ",");
            sb.Append(ItemId + ",");
            sb.Append(GeneratedBarcode + ",");
            sb.Append(RequestCode + ",");
            sb.Append(Status + ",");
            sb.Append(FailureCode + ",");
            foreach (var item in ProcessHistoryValues)
            {
                sb.Append(item + ",");
            }


            var writeString = sb.ToString();
            var stream = client.GetStream();
            var outData = Encoding.ASCII.GetBytes(writeString);
            stream.Write(outData, 0, outData.Length);

            var bytes = new byte[1024];
            client.ReceiveTimeout = 5000;
            try
            {
                var i = stream.Read(bytes, 0, bytes.Length);
                var receivedBytes = new byte[i];
                Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);
                var inString = Encoding.Default.GetString(receivedBytes).Trim();
                ResponseString =  inString;
            }
            catch
            {
                ResponseString = "Receive timeout";
            }

            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();

            return ResponseString;
        }


    }
}
