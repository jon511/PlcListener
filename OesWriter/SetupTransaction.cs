using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OesWriter
{
    public class SetupTransaction
    {
        /// <summary>
        /// Command for transaction type:
        /// PROD = Production
        /// SETUP = Setup
        /// LOGIN = Login
        /// SERIAL = Serial request
        /// </summary>
        public string Command { get; set; }
        public string CellId { get; set; }
        public string ModelNumber { get; set; }
        public int RequestCode { get; set; }
        public string OpNumber { get; set; }
        public string Component { get; set; }
        public int AccessId { get; set; }
        public string ResponseString { get; set; }

        public SetupTransaction()
        {
            Command = "SETUP";
            RequestCode = 4;

        }

        public SetupResponse SendTransaction(string ipAddress)
        {
            var port = 55001;
            var sr = new SetupResponse();

            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, port);
            }
            catch
            {
                sr.ResponseString = "Timeout connecting to target";
                return sr;
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
                ResponseString = inString;
                //if (RequestCode == 5)
                //{
                return Newtonsoft.Json.JsonConvert.DeserializeObject<SetupResponse>(inString);
                //    Console.WriteLine("");
                //}

            }
            catch
            {
                sr.ResponseString = "Timeout waiting on target response";
            }

            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();
            return sr;

        }

        public SetupResponse SendCsvTransaction(string ipAddress)
        {
            var port = 55001;
            var sr = new SetupResponse();

            var client = new TcpClient();
            try
            {
                client.Connect(ipAddress, port);
            }
            catch
            {
                sr.ResponseString = "Timeout connecting to target";
                return sr;
            }

            //var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
            //                new Newtonsoft.Json.JsonSerializerSettings
            //                {
            //                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            //                });

            var sb = new StringBuilder();

            if (RequestCode == 4 | RequestCode == 16)
            {
                sb.Append(Command + ",");
                sb.Append(CellId + ",");
                sb.Append(RequestCode + ",");
                sb.Append(ModelNumber + ",");
                sb.Append(OpNumber);
            }else if (RequestCode == 5 || RequestCode == 6 || RequestCode == 17 || RequestCode == 18)
            {
                sb.Append(Command + ",");
                sb.Append(CellId + ",");
                sb.Append(RequestCode + ",");
                sb.Append(ModelNumber + ",");
                sb.Append(OpNumber + ",");
                sb.Append(AccessId + ",");
                sb.Append(Component);
            }
            else
            {
                sr.ResponseString = "Request code is not valid";
            }
            var responseString = sb.ToString();

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

                sr.ResponseString = inString;

            }
            catch
            {
                sr.ResponseString = "Timeout waiting on target response";
            }

            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();
            return sr;
        }
    }
}
