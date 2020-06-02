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
                return "Error: Timeout connecting to target";
            }


            var responseString = Command + "," + CellId + "," + RequestCode;
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
                if (inString.Contains(","))
                {
                    var spStr = inString.Split(',');

                    if (spStr[1].Length >= 11 && !string.IsNullOrEmpty(spStr[1]) && !spStr[1].StartsWith("\0"))
                    {
                        foreach (var c in spStr[1])
                        {
                            if (c > 20)
                            {
                                status += c;
                            }
                        }
                    }
                    else
                    {
                        status = "Error: Valid serial number was not returned";
                    }

                    
                }
                else
                {
                    status = "Error: Returned format is different that expected";
                }
                

                

            }
            catch
            {
                status = "Error: Timeout waiting on target response";
            }

            if (client.Connected)
                client.GetStream().Close();

            

            client.Close();
            stream.Flush();
            return status;
        }
    }
}
