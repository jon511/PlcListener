using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OESListener
{
    public class AbPlcAsyncListener
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

        protected virtual void OnFinalLabelPrintReceived(LabelPrintEventArgs e)
        {
            FinalLabelPrintReceived?.Invoke(this, e);
        }
        public event EventHandler<LabelPrintEventArgs> FinalLabelPrintReceived;

        public string myIPAddress { get; set; }
        public int Port { get; set; }

        public AbPlcAsyncListener()
        {

        }

        protected void ParseSlcRecievedData(string senderIp, byte[] bytes)
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
                    ParseProductionTransaction(senderIp, dataArray, tagName);
                    break;
                case 2:
                case 3:
                    ParseLoginTransaction(senderIp, dataArray, tagName);
                    break;
                case 4:
                case 5:
                case 6:
                    ParseSetupTransaction(senderIp, dataArray, tagName);
                    break;
                case 21:
                    ParseRequestSerialTransaction(senderIp, dataArray, tagName);
                    break;
                case 31:
                    // print final label
                    ParsePrintFinalLabelTransaction(senderIp, dataArray, tagName);
                    break;
                case 32:
                    // print interim label

                    break;
                default:
                    if (tagName == "N247:20")
                        ParseRequestSerialTransaction(senderIp, dataArray, tagName);

                    break;
            }

        }

        protected void ParseProductionTransaction(string senderIp, short[] dataArray, string tagName)
        {
            var e = new ProductionEventArgs(senderIp);
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

            OnProductionReceived(e);
        }

        protected void ParseLoginTransaction(string senderIp, short[] dataArray, string tagName)
        {
            var e = new LoginEventArgs(senderIp);
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

            OnLoginReceived(e);
        }

        protected void ParseSetupTransaction(string senderIp, short[] dataArray, string tagName)
        {
            var e = new SetupEventArgs(senderIp);
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

            OnSetupReceived(e);
        }

        protected void ParseRequestSerialTransaction(string senderIp, short[] dataArray, string tagName)
        {
            var e = new SerialRequestEventArgs(senderIp);

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

            e.CellId = Encoding.Default.GetString(bytes.ToArray());

            OnSerialRequestReceived(e);
        }

        protected void ParsePrintFinalLabelTransaction(string senderIp, short[] dataArray, string tagName)
        {
            var e = new LabelPrintEventArgs(senderIp);

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


            OnFinalLabelPrintReceived(e);

        }
    }
}
