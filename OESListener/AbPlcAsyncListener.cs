using System;
using System.Collections.Generic;
using System.Text;

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

        protected void ParseSlcRecievedData(string senderIp, byte[] bytes, bool useMicroLogix = false)
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

            var usePlcFive = false;

            if (dataArray[18] > 99)
            {
                usePlcFive = true;
                dataArray[18] -= 100;
            }

            switch (dataArray[18])
            {
                case 0:
                case 1:
                    ParseProductionTransaction(senderIp, dataArray, tagName, usePlcFive, useMicroLogix);
                    break;
                case 2:
                case 3:
                case 19:
                    ParseLoginTransaction(senderIp, dataArray, tagName, usePlcFive, useMicroLogix);
                    break;
                case 4:
                case 5:
                case 6:
                case 16:
                case 17:
                case 18:
                    ParseSetupTransaction(senderIp, dataArray, tagName, usePlcFive, useMicroLogix);
                    break;
                case 21:
                    ParseRequestSerialTransaction(senderIp, dataArray, tagName, usePlcFive, useMicroLogix);
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
                        ParseRequestSerialTransaction(senderIp, dataArray, tagName, usePlcFive, useMicroLogix);

                    break;
            }

        }

        protected void ParseProductionTransaction(string senderIp, short[] dataArray, string tagName, bool usePlcFive = false, bool useMicroLogix = false)
        {
            var e = new ProductionEventArgs(senderIp);
            e.InTagName = tagName;
            if (e.InTagName.Contains(":"))
                e.listenerType = ListenerType.PCCC;
            else
                e.listenerType = ListenerType.EIP;

            e.UsePlcFive = usePlcFive;
            e.UsePlcMicrologix = useMicroLogix;

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

            bytes = new List<byte>();
            //{
            //    (byte)((dataArray[11] & 0xff00) >> 8),
            //    (byte)(dataArray[11] & 0x00ff)
            //};

            for (var i = 11; i < 12; i++)
            {
                var b1 = (byte)((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte)(dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);

                if (b2 != 0)
                    bytes.Add(b2);
            }

            e.GeneratedBarcode = Encoding.Default.GetString(bytes.ToArray());

            e.In_Word_0 = dataArray[0];
            e.In_Word_1 = dataArray[1];
            e.In_Word_2 = dataArray[2];
            e.In_Word_3 = dataArray[3];
            e.In_Word_4 = dataArray[4];
            e.In_Word_5 = dataArray[5];
            e.In_Word_6 = dataArray[6];
            e.In_Word_7 = dataArray[7];
            e.In_Word_8 = dataArray[8];
            e.In_Word_9 = dataArray[9];
            e.In_Word_10 = dataArray[10];
            e.In_Word_11 = dataArray[11];


            e.ProcessIndicator = dataArray[18];
            e.SuccessIndicator = dataArray[19];
            e.FaultCode = dataArray[20];

            e.P_Val_1 = dataArray[22];
            e.P_Val_2 = dataArray[23];
            e.P_Val_3 = dataArray[24];
            e.P_Val_4 = dataArray[25];
            e.P_Val_5 = dataArray[26];
            e.P_Val_6 = dataArray[27];
            e.P_Val_7 = dataArray[28];
            e.P_Val_8 = dataArray[29];
            e.P_Val_9 = dataArray[30];
            e.P_Val_10 = dataArray[31];
            e.P_Val_11 = dataArray[32];
            e.P_Val_12 = dataArray[33];
            e.P_Val_13 = dataArray[34];
            e.P_Val_14 = dataArray[35];
            e.P_Val_15 = dataArray[36];
            e.P_Val_16 = dataArray[37];
            e.P_Val_17 = dataArray[38];
            e.P_Val_18 = dataArray[39];
            e.P_Val_19 = dataArray[40];
            e.P_Val_20 = dataArray[41];
            e.P_Val_21 = dataArray[42];
            e.P_Val_22 = dataArray[43];
            e.P_Val_23 = dataArray[44];
            e.P_Val_24 = dataArray[45];
            e.P_Val_25 = dataArray[46];
            e.P_Val_26 = dataArray[47];
            e.P_Val_27 = dataArray[48];
            e.P_Val_28 = dataArray[49];

            OnProductionReceived(e);
        }

        protected void ParseLoginTransaction(string senderIp, short[] dataArray, string tagName, bool usePlcFive = false, bool useMicroLogix = false)
        {
            var e = new LoginEventArgs(senderIp);

            e.InTagName = tagName;
            if (e.InTagName.Contains(":"))
                e.listenerType = ListenerType.PCCC;
            else
                e.listenerType = ListenerType.EIP;

            e.UsePlcFive = usePlcFive;
            e.UsePlcMicrologix = useMicroLogix;

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

            e.ProcessIndicator = dataArray[18];

            OnLoginReceived(e);
        }

        protected void ParseSetupTransaction(string senderIp, short[] dataArray, string tagName, bool usePlcFive = false, bool useMicroLogix = false)
        {
            var e = new SetupEventArgs(senderIp);

            e.InTagName = tagName;
            if (e.InTagName.Contains(":"))
                e.listenerType = ListenerType.PCCC;
            else
                e.listenerType = ListenerType.EIP;

            e.UsePlcFive = usePlcFive;
            e.UsePlcMicrologix = useMicroLogix;

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

                e.AccessId = dataArray[16];
            }

            e.ProcessIndicator = dataArray[18];

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

        protected void ParseRequestSerialTransaction(string senderIp, short[] dataArray, string tagName, bool usePlcFive = false, bool useMicroLogix = false)
        {
            var e = new SerialRequestEventArgs(senderIp);

            e.InTagName = tagName;
            if (e.InTagName.Contains(":"))
                e.listenerType = ListenerType.PCCC;
            else
                e.listenerType = ListenerType.EIP;

            e.UsePlcFive = usePlcFive;
            e.UsePlcMicrologix = useMicroLogix;

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
