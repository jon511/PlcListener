using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[assembly: CLSCompliant(true)]
namespace PlcListener
{
    public class ABListener
    {
        public event EventHandler MessageRecieved;
        
        protected virtual void OnMessageRecieved(MessageEventArgs e)
        {
            MessageRecieved?.Invoke(this, e);
        }
        
        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);

        private string _localIp = "";
        private const int Port = 44818;

        public void Listen(string ipAddress)
        {
            Thread myNewThread = new Thread(() => StartListening(ipAddress));
            myNewThread.Start();
        }
        
        private void StartListening(string ipAddress)
        {
            _localIp = ipAddress;
            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse(_localIp), Port);
                server.Start();

                while (true)
                {
                    AllDone.Reset();
                    Console.WriteLine("Ready for connection...");

                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    new Thread(() => HandleMessage(client)).Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: ${0}", e);
            }
            finally
            {
                //Stop listening for new clients
                server.Stop();
            }
        }

        private void HandleMessage(TcpClient client)
        {
            var senderIP = ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString();
            var plcID = new byte[4];
            var bytes = new byte[1024];

            var stream = client.GetStream();
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var receivedBytes = new byte[i];
                Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);

                // plc sending ListServices request with 0x04 in first byte
                // not required in all processors
                // from CIP Network Library Vol 2 section 2-4.6
                if (receivedBytes[0] == 0x04)
                {
                    ListServices(stream, receivedBytes);
                }

                //plc is requesting register session with 0x65 in first byte
                //from CIP Network Library Vol 2 section 2-4.4
                //generate a 4 byte session handle and return to requesting device in words 4-7.
                // the rest of the return is echoing back what is received.
                if (receivedBytes[0] == 0x65)
                {
                    RegisterSesision(stream, receivedBytes);
                }

                // plc sending SendRRData Request with 0x6f in first byte
                // from CIP Network Library Vol 2 section 2-4.7
                // acknowledge response is echoing the first 44 words of the request, changing word 2 to value of 0x14
                // values for words 38 - 43 must be changed before sending the response
                // word 48 is the length of the complete write request
                // word 50 begins the write request Publication 1756-PM020A-EN-P Logix5000 DAta Access page 22
                if (receivedBytes[0] == 0x6f)
                {
                    // value 0x54 in element 40 indicates Foward Open. Data will be sent seperately after connection is
                    // established.
                    // value 0x52 indicates unconnected message. Data is appending to this transaction.
                    if (receivedBytes[40] == 0x54)
                    {
                        FowardOpen(stream, receivedBytes, plcID, senderIP);
                    }else if (receivedBytes[40] == 0x52)
                    {
                        UnconnectedMessage(stream, receivedBytes, senderIP);
                        break;
                    }
                    else
                    {
                        return;
                    }
                }

                if (receivedBytes[0] == 0x70)
                {
                    SendUnitData(stream, receivedBytes, plcID, senderIP);
                }
                
                if (receivedBytes[0] == '*' && receivedBytes[1] == '*')
                {
                    ParseTcpReceivedData(receivedBytes, senderIP);
//                    new Thread(() => ParseData()).Start();
                    break;
                }

            }

            client.GetStream().Close();
            client.Close();
            stream.Flush();
        }

        private void ParseTcpReceivedData(byte[] bytes, string senderIP)
        {
            var s = System.Text.Encoding.Default.GetString(bytes);
                    var str = s.Substring(2);
                    var itemArr = str.Split(',');
            if (itemArr.Length < 4)
                return;

            var tagName = itemArr[0];
            var cellId = ConvertStringToByteArray(itemArr[1]);
            var serialNumber = ConvertStringToByteArray(itemArr[2]);
            Int16 processType;
            var success = Int16.TryParse(itemArr[3], out processType);
            if (!success)
                return;

            Int16 successIndicator;
            success = Int16.TryParse(itemArr[4], out successIndicator);
            {
                if (!success)
                    return;
            }

            Int16 faultCode;
            if (!Int16.TryParse(itemArr[5], out faultCode))
            {
                return;
            }
            
            var processHistoryArr = new string[14];
            if (itemArr.Length > 6)
            {
                for (var j = 6; j < itemArr.Length; j++)
                {
                    if (j > 22)
                        break;
                    processHistoryArr[j - 6] = itemArr[j];
                }
            }

            var processHistoryIntArr = new List<short>();
            foreach (var p in processHistoryArr)
            {
                if (double.TryParse(p, out double d))
                {
                    var a = Convert.ToInt16(Math.Truncate(d));
                    var b = Convert.ToInt16((d - (double)a) * 100);
                    processHistoryIntArr.Add(a);
                    processHistoryIntArr.Add(b);
                }
                else
                {
                    processHistoryIntArr.Add(0);
                    processHistoryIntArr.Add(0);
                }
                
            }

            var n = processHistoryIntArr.ToArray();
            
            Console.WriteLine("Process type {0}", processType);

            var dataArray = new short[50];
            Array.Copy(cellId, 0, dataArray, 0, cellId.Length);
            Array.Copy(serialNumber, 0, dataArray, 5, serialNumber.Length);
            Array.Copy(n, 0, dataArray, 22, n.Length);
            dataArray[18] = processType;
            dataArray[19] = successIndicator;
            dataArray[20] = faultCode;

            var m = new MessageEventArgs(senderIP, _localIp, tagName, dataArray, 50, TypeCode.Int16, DateTime.Now);
            OnMessageRecieved(m);
        }
        
        private void ListServices(System.IO.Stream stream, byte[] bytes)
        {
            bytes[2] = 26;
            byte[] resArr = {0x01, 0x00, 0x00, 0x01, 0x14, 0x00, 0x01, 0x00, 0x20, 0x00};
            byte[] nameOfService = {0x43, 0x6f, 0x6d, 0x6d, 0x75, 0x6e, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x73, 0x00, 0x00};
            
            var retArr = new byte[bytes.Length + resArr.Length + nameOfService.Length];

            Buffer.BlockCopy(bytes, 0, retArr, 0, bytes.Length);
            Buffer.BlockCopy(resArr, 0, retArr, bytes.Length, resArr.Length);
            Buffer.BlockCopy(nameOfService, 0, retArr, bytes.Length + resArr.Length, nameOfService.Length);

            stream.Write(retArr, 0, retArr.Length);
        }

        private void RegisterSesision(System.IO.Stream stream, byte[] bytes)
        {
            var rnd = new Random();
            var sessionInt = rnd.Next(1, 63000);

            var sessionHandle = ConvertIntToFourBytes(sessionInt);
            Buffer.BlockCopy(sessionHandle, 0, bytes, 4, 4);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void UnconnectedMessage(System.IO.Stream stream, byte[] bytes, string senderIP)
        {
            var retArr = new byte[44];
            Buffer.BlockCopy(bytes, 0, retArr, 0, retArr.Length);
            
            retArr[2] = 0x14;
            retArr[3] = 0x00;
            retArr[38] = 0x04;
            retArr[39] = 0x00;
            retArr[40] = 0xcd;
            retArr[41] = 0x00;
            retArr[42] = 0x00;
            retArr[43] = 0x00;
            
            stream.Write(retArr, 0, retArr.Length);

            var writeReqLength = Convert.ToInt32(bytes[48]);
                    
            var dataArray = new byte[writeReqLength];
            Buffer.BlockCopy(bytes, 50, dataArray, 0, writeReqLength);

            ParseLogixRecievedData(dataArray, senderIP);
            
        }

        private void FowardOpen(System.IO.Stream stream, byte[] bytes, byte[] plcID, string senderIP)
        {
            var retArr = new byte[72];
            Buffer.BlockCopy(bytes, 0, retArr, 0, retArr.Length);
            Buffer.BlockCopy(bytes, 52, plcID, 0, 4);

            retArr[2] = 0x2e;
            retArr[3] = 0x00;

            retArr[38] = 0x1e;
            retArr[39] = 0x00;
            retArr[40] = 0xd4;
            retArr[41] = 0x00;
            retArr[42] = 0x00;
            retArr[43] = 0x00;
            retArr[44] = 0x00; //0x07
            retArr[45] = 0x00; //0x01
            retArr[46] = 0x00; //0x00
            retArr[47] = 0x00; //0x8a
            retArr[48] = bytes[52]; //0x00
            retArr[49] = bytes[53]; //0x07
            retArr[50] = bytes[54]; //0x00
            retArr[51] = bytes[55]; //0x11
            retArr[52] = 0x11;
            retArr[53] = 0x00;
            retArr[54] = 0x01;
            retArr[55] = 0x00;
            retArr[56] = 0x02; //0x8a
            retArr[57] = 0xbb; //0x76
            retArr[58] = 0x47; //0x1e
            retArr[59] = 0x60; //0xbc
            retArr[60] = 0xe0; //0x40
            retArr[61] = 0x70; //0x82
            retArr[62] = 0x72; //0x1f
            retArr[63] = 0x00; //0x00
            retArr[64] = 0x01;
            retArr[65] = 0x00;
            retArr[66] = 0x00;
            retArr[67] = 0x00;
            retArr[68] = 0x00;
            retArr[69] = 0x00;

            stream.Write(retArr, 0, 70);
                    
                    
        }

        private void SendUnitData(System.IO.Stream stream, byte[] bytes, byte[] plcID, string senderIP)
        {
            var rArr = new byte[61];
            Buffer.BlockCopy(bytes, 0, rArr, 0, rArr.Length);
                    
            rArr[2] = 0x25;

            rArr[36] = plcID[0];
            rArr[37] = plcID[1];
            rArr[38] = plcID[2];
            rArr[39] = plcID[3];

            rArr[42] = 0x11;
            rArr[43] = 0x00;
            rArr[44] = bytes[44];
            rArr[45] = 0x00;
            rArr[46] = 0xcb;
            rArr[47] = 0x00;
            rArr[48] = 0x00;
            rArr[49] = 0x00;
            rArr[50] = 0x07;
            rArr[51] = 0x01;
            rArr[52] = 0x00;
            rArr[53] = bytes[55];
            rArr[54] = bytes[56];
            rArr[55] = bytes[57];
            rArr[56] = bytes[58];
            rArr[57] = 0x4f;
            rArr[58] = bytes[60];
            rArr[59] = bytes[61];
            rArr[60] = bytes[62];

            stream.Write(rArr, 0, 61);

            Console.WriteLine(bytes[64]);

            var copyLenth = bytes[64] + 5;

            var bytesToParse = new byte[copyLenth];
                    
            Buffer.BlockCopy(bytes, 64, bytesToParse, 0, bytesToParse.Length);

            ParseSlcRecievedData(bytesToParse, senderIP);
        }

        private void ParseLogixRecievedData(byte[] bytes, string senderIP)
        {
            var tagNameLen = (int)bytes[3];
            var tagNameArr = new byte[tagNameLen];
            Buffer.BlockCopy(bytes, 4, tagNameArr, 0, tagNameLen);
            var tagName = System.Text.Encoding.Default.GetString(tagNameArr) + "[0]";
            var dataLength = 3 + tagNameLen + 5;
            var startingPoint = 3 + tagNameLen + 5 + 2;

            var arrLen = bytes[dataLength] * 2;

            var tempArr = new byte[arrLen];
            Buffer.BlockCopy(bytes, startingPoint, tempArr, 0, arrLen);
            
            Console.WriteLine(tagName);

            var dataArray = new Int16[50];
            
            for (int i = 0; i < tempArr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var c = Convert.ToInt16(Convert2BytesToInteger(tempArr[i], tempArr[i + 1]));
                    dataArray[i / 2] = c;
                }
            }
            var m = new MessageEventArgs(senderIP, _localIp, tagName, dataArray, 50, TypeCode.Int16, DateTime.Now);
            OnMessageRecieved(m);

        }
        
        private void ParseSlcRecievedData(byte[] bytes, string senderIP)
        {
            
            var retArr = new int[bytes[0] / 2];

            var prefix = GetSlcDataType(bytes[2]);
            var dataTable = bytes[1].ToString();
            var element = bytes[3].ToString();

            var tagName = prefix + dataTable + ":" + element;

            var tempArr = new byte[bytes[0]];
            
            Buffer.BlockCopy(bytes, 5, tempArr, 0, tempArr.Length);

            var dataArray = new Int16[50];
            
            for (var i = 0; i < tempArr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var c = Convert.ToInt16(Convert2BytesToInteger(tempArr[i], tempArr[i + 1]));
                    dataArray[i / 2] = c;
                }
            }
            
            var m = new MessageEventArgs(senderIP, _localIp, tagName, dataArray,50,TypeCode.Int16, DateTime.Now);
            OnMessageRecieved(m);
            
        }

        private string GetSlcDataType(byte val)
        {
            // file types from slc
            //80-􏰀83 hex: reserved •84 hex: status
            // •85 hex: bit
            // •86 hex: timer
            // •87 hex: counter
            // •88 hex: control
            // •89 hex: integer
            // •8A hex: floating point •8D hex: string
            // •8E hex: ASCII
            switch (val)
            {
                case 0x85:
                    return "B";
                case 0x86:
                    return "T";
                case 0x87:
                    return "C";
                case 0x88:
                    return "R";
                case 0x89:
                    return "N";
                case 0x8a:
                    return "F";
                case 0x8d:
                    return "S";
                case 0x8e:
                    return "A";
                default:
                    return "";
            }

        }

        private void ParseData()
        {
            Console.WriteLine("parsing data");
            Thread.Sleep(10000);
            Console.WriteLine("sleep done");
        }

//        private IPAddress GetLocalIpAddress()
//        {
//            var host = Dns.GetHostEntry(Dns.GetHostName());
//            foreach (var ip in host.AddressList)
//            {
//                if (ip.AddressFamily == AddressFamily.InterNetwork)
//                {
//                    return ip;
//                }
//            }
//
//            throw new Exception("No network adapters with an IPv4 address in the system!");
//        }

        private static byte[] ConvertIntToFourBytes(int val)
        {
            var b = new byte[4];
            b[0] = Convert.ToByte(val & 0x000000ff);
            b[1] = Convert.ToByte((val & 0x0000ff00) >> 8);
            b[2] = Convert.ToByte((val & 0x00ff0000) >> 16);
            b[3] = Convert.ToByte((val & 0xff000000) >> 24);

            return b;
        }

        private static int Convert2BytesToInteger(byte b1, byte b2)
        {
            return Convert.ToInt32(b2 << 8) + Convert.ToInt32(b1);
        }

        private static short[] ConvertStringToByteArray(string s)
        {
            var bc = (char)0;
            if (s.Length % 2 == 1)
                s += bc;
            
            var len = s.Length;

            var retArr = new List<short>();
            for (var i = 0; i < len; i++)
            {
                if (i % 2 == 0)
                {
                    retArr.Add(Convert.ToInt16((s[i] <<8) + s[i + 1]));
                }
            }

            return retArr.ToArray();
        }
    }
}