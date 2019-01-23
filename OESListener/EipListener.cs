using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OESListener
{
    public class EipListener
    {
        protected virtual void OnProductionReceived(ProductionEventArgs e)
        {
            ProductionReceived?.Invoke(this, e);
        }
        public event EventHandler<ProductionEventArgs> ProductionReceived;

        protected virtual void OnSetupReceived(SetupEventArgs e)
        {
            SetupReceived?.Invoke(this, e);
        }
        public event EventHandler<SetupEventArgs> SetupReceived;

        protected virtual void OnLoginReceived(LoginEventArgs e)
        {
            LoginReceived?.Invoke(this, e);
        }
        public event EventHandler<LoginEventArgs> LoginReceived;

        protected virtual void OnSerialRequestReceived(SerialRequestEventArgs e)
        {
            SerialRequestReceived?.Invoke(this, e);
        }
        public event EventHandler<SerialRequestEventArgs> SerialRequestReceived;

        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        public string myIPAddress { get; set; }
        private const int Port = 44818;

        public EipListener(string ipAddress)
        {
            myIPAddress = ipAddress;
        }

        public void Listen()
        {
            Thread myNewThread = new Thread(() => StartListening());
            myNewThread.Start();
        }

        private void StartListening()
        {
            System.Net.Sockets.TcpListener server = null;

            try
            {
                server = new System.Net.Sockets.TcpListener(IPAddress.Parse(myIPAddress), Port);
                server.Start();

                while (true)
                {
                    AllDone.Reset();
                    if (Logger.Enabled)
                        Logger.Log("Ready for connection...");

                    var client = server.AcceptTcpClient();
                    if (Logger.Enabled)
                        Logger.Log("Connected!");

                    new Thread(() => HandleMessage(client)).Start();
                }
            }
            catch (SocketException e)
            {
                if (Logger.Enabled)
                    Logger.Log(string.Format("SocketException: {0}", e));
            }
            finally
            {
                //Stop listening for new clients
                server.Stop();
            }
        }

        private void HandleMessage(TcpClient client)
        {
            var senderIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            var plcID = new byte[4];
            var bytes = new byte[1024];

            var stream = client.GetStream();
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var receivedBytes = new byte[i];
                Logger.Log(receivedBytes.ToString());
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
                else if (receivedBytes[0] == 0x65)
                {
                    RegisterSesision(stream, receivedBytes);
                }

                // plc sending SendRRData Request with 0x6f in first byte
                // from CIP Network Library Vol 2 section 2-4.7
                // acknowledge response is echoing the first 44 words of the request, changing word 2 to value of 0x14
                // values for words 38 - 43 must be changed before sending the response
                // word 48 is the length of the complete write request
                // word 50 begins the write request Publication 1756-PM020A-EN-P Logix5000 DAta Access page 22
                else if (receivedBytes[0] == 0x6f)
                {
                    // value 0x54 in element 40 indicates Foward Open. Data will be sent seperately after connection is
                    // established.
                    // value 0x52 indicates unconnected message. Data is appending to this transaction.
                    if (receivedBytes[40] == 0x54)
                    {
                        FowardOpen(stream, receivedBytes, plcID, senderIP);
                    }
                    else if (receivedBytes[40] == 0x52)
                    {
                        UnconnectedMessage(client, receivedBytes, senderIP);
                        break;
                    }
                    else
                    {
                        return;
                    }
                }

                else if (receivedBytes[0] == 0x70)
                {
                    SendUnitData(client, receivedBytes, plcID);
                }

                else { break; }

            }

            client.GetStream().Close();
            client.Close();
            stream.Flush();
        }

        private void ListServices(System.IO.Stream stream, byte[] bytes)
        {
            bytes[2] = 26;
            byte[] resArr = { 0x01, 0x00, 0x00, 0x01, 0x14, 0x00, 0x01, 0x00, 0x20, 0x00 };
            byte[] nameOfService = { 0x43, 0x6f, 0x6d, 0x6d, 0x75, 0x6e, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x73, 0x00, 0x00 };

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

            var sessionHandle = Util.ConvertIntToFourBytes(sessionInt);
            Buffer.BlockCopy(sessionHandle, 0, bytes, 4, 4);
            stream.Write(bytes, 0, bytes.Length);
        }
        private void UnconnectedMessage(TcpClient client, byte[] bytes, string senderIP)
        {
            var stream = client.GetStream();
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

            ParseLogixRecievedData(client, dataArray, senderIP);

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

        private void SendUnitData(TcpClient client, byte[] bytes, byte[] plcID)
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

            var stream = client.GetStream();
            stream.Write(rArr, 0, 61);

            var copyLenth = bytes[64] + 5;

            var bytesToParse = new byte[copyLenth];

            Buffer.BlockCopy(bytes, 64, bytesToParse, 0, bytesToParse.Length);

            ParseSlcRecievedData(client, bytesToParse);
        }

        private void ParseLogixRecievedData(TcpClient client,byte[] bytes, string senderIP)
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

            var dataArray = new Int16[50];

            for (int i = 0; i < tempArr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var c = Convert.ToInt16(Util.Convert2BytesToInteger(tempArr[i], tempArr[i + 1]));
                    dataArray[i / 2] = c;
                }
            }

            switch (dataArray[18])
            {
                case 0: //pre process
                case 1:
                    ParseProductionTransaction(client, dataArray, tagName);
                    break;
                case 2:
                case 3:
                    ParseLoginTransaction(client, dataArray, tagName);
                    break;
                case 4:
                case 5:
                case 6:
                    ParseSetupTransaction(client, dataArray, tagName);
                    break;
                case 21: //serial request
                    ParseRequestSerialTransaction(client, dataArray, tagName);
                    break;
                default:
                    if (tagName == "N247[20]")
                        ParseRequestSerialTransaction(client, dataArray, tagName);

                    break;
            }

        }

        private void ParseSlcRecievedData(TcpClient client, byte[] bytes)
        {
            var retArr = new int[bytes[0] / 2];

            var prefix = Util.GetSlcDataType(bytes[2]);
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
                    var c = Convert.ToInt16(Util.Convert2BytesToInteger(tempArr[i], tempArr[i + 1]));
                    dataArray[i / 2] = c;
                }
            }

            switch (dataArray[18])
            {
                case 0:
                case 1:
                    ParseProductionTransaction(client, dataArray, tagName);
                    break;
                case 2:
                case 3:
                    ParseLoginTransaction(client, dataArray, tagName);
                    break;
                case 4:
                case 5:
                case 6:
                    ParseSetupTransaction(client, dataArray, tagName);
                    break;
                case 21:
                    ParseRequestSerialTransaction(client, dataArray, tagName);
                    break;
                default:
                    if (tagName == "N247:20")
                        ParseRequestSerialTransaction(client, dataArray, tagName);

                    break;
            }

        }
        
        private void ParseProductionTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new ProductionEventArgs(client);
            e.InTagName = tagName;
            if (e.InTagName.Contains(":"))
                e.listenerType = ListenerType.PCCC;
            else
                e.listenerType = ListenerType.EIP;

            var bytes = new List<byte>();
            for (int i = 0; i < 5; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.CellID = Encoding.Default.GetString(bytes.ToArray());

            bytes = new List<byte>();
            for (var i = 5; i < 11; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.ItemID = Encoding.Default.GetString(bytes.ToArray());

            bytes = new List<byte>
            {
                (byte)((dataArray[11] & 0xff00) >> 8),
                (byte)(dataArray[11] & 0x00ff)
            };

            e.GeneratedBarcode = Encoding.Default.GetString(bytes.ToArray());

            e.Request = dataArray[18].ToString();
            e.Status = dataArray[19].ToString();
            e.FailureCode = dataArray[20].ToString();

            for (var i = 22; i < dataArray.Length; i++)
            {
                if (i % 2 == 0)
                {
                    e.ProcessHistoryValues.Add((dataArray[i] + (dataArray[i + 1] * .01)).ToString());
                }
            }

            OnProductionReceived(e);
        }

        private void ParseLoginTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new LoginEventArgs(client);
            var bytes = new List<byte>();
            for (int i = 0; i < 5; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.CellID = Encoding.Default.GetString(bytes.ToArray());

            bytes = new List<byte>();
            for (var i = 5; i < 10; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.OperatorID = Encoding.Default.GetString(bytes.ToArray());

            e.Request = dataArray[18].ToString();

            OnLoginReceived(e);
        }

        private void ParseSetupTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new SetupEventArgs(client);
            var bytes = new List<byte>();
            for (int i = 0; i < 5; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.CellID = Encoding.Default.GetString(bytes.ToArray());

            if (dataArray[18] == 5 || dataArray[18] == 6 || dataArray[18] == 17 || dataArray[18] == 18)
            {
                bytes = new List<byte>();
                for (int i = 5; i < 12; i++)
                {
                    var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                    var b2 = (byte)(dataArray[i] & 0x00ff);
                    if (b1 != 0)
                        bytes.Add(b1);

                    if (b2 != 0)
                        bytes.Add(b2);
                }

                e.Component = Encoding.Default.GetString(bytes.ToArray());

                e.AccessId = dataArray[16].ToString();
            }

            e.Request = dataArray[18].ToString();

            bytes = new List<byte>();
            for (int i = 22; i < 30; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.ModelNumber = Encoding.Default.GetString(bytes.ToArray());

            bytes = new List<byte>();
            for (int i = 30; i < 32; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.OpNumber = Encoding.Default.GetString(bytes.ToArray());

            OnSetupReceived(e);
        }

        private void ParseRequestSerialTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new SerialRequestEventArgs(client);

            e.listenerType = ListenerType.PCCC;
            var bytes = new List<byte>();
            for (int i = 0; i < 5; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.CellID = Encoding.Default.GetString(bytes.ToArray());

            OnSerialRequestReceived(e);
        }
        

    }
}
