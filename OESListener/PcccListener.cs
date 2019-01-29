using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace OESListener
{
    public class PcccListener
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

        protected virtual void OnLabelPrintReceived(LabelPrintEventArgs e)
        {
            LabelPrintReceived?.Invoke(this, e);
        }
        public event EventHandler<LabelPrintEventArgs> LabelPrintReceived;

        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        public string myIPAddress { get; set; }
        private const int Port = 2222;

        public PcccListener(string ipAddress)
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
                        Logger.Log("PCCC Listener ready for connection...");

                    var client = server.AcceptTcpClient();
                    if (Logger.Enabled)
                        Logger.Log("Connected");

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
                Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);

                if (receivedBytes[0] == 0x01 && receivedBytes[1] == 0x01)
                {
                    if (Logger.Enabled)
                        Logger.Log("Connection Received");

                    var tempArr = new byte[receivedBytes.Length];
                    Buffer.BlockCopy(receivedBytes, 0, tempArr, 0, tempArr.Length);
                    tempArr[0] = 0x02;
                    //tempArr[5] = 0x01;
                    tempArr[15] = 0x05;
                    
                    stream.Write(tempArr, 0, tempArr.Length);
                }
                if (receivedBytes[0] == 0x01 && receivedBytes[1] == 0x07)
                {
                    var retArray = new byte[36];
                    Buffer.BlockCopy(receivedBytes, 0, retArray, 0, retArray.Length);
                    retArray[0] = 0x02;
                    retArray[3] = 0x08;
                    retArray[32] = 0x4f;
                    stream.Write(retArray, 0, retArray.Length);

                    var dataLen = receivedBytes.Length - 37;
                    var tempArr = new byte[dataLen];
                    Array.Copy(receivedBytes, 37, tempArr, 0, tempArr.Length);
                    ParseSlcRecievedData(client, tempArr);
                    break;
                }

                if (receivedBytes[1] != 0x01 && receivedBytes[1] != 0x07){
                    
                    break;
                }

            }

            client.GetStream().Close();
            client.Close();
            stream.Flush();
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
                case 19:
                    ParseLoginTransaction(client, dataArray, tagName);
                    break;
                case 4:
                case 5:
                case 6:
                case 14:
                case 15:
                case 16:
                    ParseSetupTransaction(client, dataArray, tagName);
                    break;
                case 21:
                    ParseRequestSerialTransaction(client, dataArray, tagName);
                    break;
                case 31:
                case 32:
                    // print final label
                    ParsePrintLabelTransaction(client, dataArray, tagName);
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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

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

            e.ItemId = Encoding.Default.GetString(bytes.ToArray());

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

            e.listenerType = ListenerType.PCCC;
            OnProductionReceived(e);
        }

        private void ParseLoginTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new LoginEventArgs(client);
            e.InTagName = tagName;

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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

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

            e.listenerType = ListenerType.PCCC;
            OnLoginReceived(e);
        }

        private void ParseSetupTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new SetupEventArgs(client);
            e.InTagName = tagName;

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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

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
            e.listenerType = ListenerType.PCCC;
            OnSetupReceived(e);
        }

        private void ParseRequestSerialTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new SerialRequestEventArgs(client);

            e.InTagName = tagName;

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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

            e.listenerType = ListenerType.PCCC;
            OnSerialRequestReceived(e);
        }

        private void ParsePrintLabelTransaction(TcpClient client, short[] dataArray, string tagName)
        {
            var e = new LabelPrintEventArgs(client);

            e.InTagName = tagName;

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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

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

            e.ItemId = Encoding.Default.GetString(bytes.ToArray());
            e.AlphaCode = e.ItemId.Substring(0, 2);

            var weightString = dataArray[22].ToString();
            var weightDecString = (dataArray[23] < 10) ? "0" + dataArray[23].ToString() : dataArray[23].ToString();

            if (dataArray[24] != 1)
                dataArray[24] = 2;

            var weightStr = weightDecString.Substring(0, dataArray[24]);

            e.Weight = string.Format("{0}.{1}", weightString, weightStr.Substring(0, dataArray[24]));
            e.RevLevel = (dataArray[25] < 10) ? "0" + dataArray[25].ToString() : dataArray[25].ToString();
            e.PrinterIpAddress = dataArray[26].ToString() + "." + dataArray[27].ToString() + "." + dataArray[28].ToString() + "." + dataArray[29].ToString();

            if (dataArray[18] == 31)
                e.PrintType = "Final";

            if (dataArray[18] == 32)
            {
                e.PrintType = "Interim";
                e.InterimFile = dataArray[30].ToString();
            }

            e.listenerType = ListenerType.PCCC;
            OnLabelPrintReceived(e);

        }


    }


}
