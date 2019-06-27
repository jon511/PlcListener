using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OesWriter
{
    public class SerialRequest
    {
        private string Command { get; set; }
        public string CellId { get; set; }
        public int RequestCode { get; set; }

        public SerialRequest()
        {
            Command = "SERIAL";
            RequestCode = 21;
        }

        public string SendTransaction(string ipAddress)
        {
            var port = 55001;
            var status = "";

            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, port);
            }
            catch
            {
                return "Timeout connecting to target";
            }


            var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
                            new Newtonsoft.Json.JsonSerializerSettings
                            {
                                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                            });
            var stream = client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);

            var bytes = new byte[2048];
            client.ReceiveTimeout = 5000;
            try
            {
                var i = stream.Read(bytes, 0, bytes.Length);
                var receivedBytes = new byte[i];
                Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);
                var inString = Encoding.Default.GetString(receivedBytes).Trim();
                status = inString;

            }
            catch
            {
                status = "Timeout waiting on target response";
            }

            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();
            return status;
        }
    }
}
