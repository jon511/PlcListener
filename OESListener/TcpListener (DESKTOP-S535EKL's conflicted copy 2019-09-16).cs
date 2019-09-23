using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

[assembly: CLSCompliant(true)]
namespace OESListener
{
    public class TcpListener
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

        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        public string myIPAddress { get; set; }
        public int Port { get; set; }
        
        public TcpListener()
        {
            Port = 55001;
            myIPAddress = "127.0.0.1";
        }

        public TcpListener(string ipAddress)
        {
            myIPAddress = ipAddress;
            Port = 55001;
        }
        
        public TcpListener(string ipAddress, int port)
        {
            myIPAddress = ipAddress;
            Port = port;
        }

        public void Listen()
        {
            
            Thread myNewThread = new Thread(() => StartListening());
            if (Logger.Enabled)
                Logger.Log("Starting new thread");
            myNewThread.IsBackground = true;
            myNewThread.Start();
        }

        private void StartListening()
        {
            if (Logger.Enabled)
                Logger.Log("Listening...");

            System.Net.Sockets.TcpListener server = null;

            try
            {
                server = new System.Net.Sockets.TcpListener(IPAddress.Parse(myIPAddress), Port);
                server.Start();

                while (true)
                {
                    AllDone.Reset();
                    if (Logger.Enabled)
                        Logger.Log("waiting for connection...");

                    var client = server.AcceptTcpClient();

                    if (Logger.Enabled)
                        Logger.Log("connected");

                    new Thread(() => HandleMessage(client)).Start();
                }
            }
            catch (SocketException e)
            {
                if (Logger.Enabled)
                    Logger.Log(string.Format("Socket Error : {0}", e));
            }
            finally
            {
                //Stop listening for new clients
                server.Stop();
            }
        }

        private void HandleMessage(TcpClient client)
        {
            //var senderIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            var bytes = new byte[1024];

            var stream = client.GetStream();
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0 && client != null)
                {
                    var receivedBytes = new byte[i];
                    Buffer.BlockCopy(bytes, 0, receivedBytes, 0, i);
                    var inString = Encoding.Default.GetString(receivedBytes).Trim();

                    //check if incoming string is valid json
                    if (inString.StartsWith("{") && inString.EndsWith("}"))
                    {
                        if (Logger.Enabled)
                            Logger.Log(string.Format("json data received from : {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()));

                        ReceiveData jsonData;
                        try
                        {

                            jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<ReceiveData>(inString);
                            switch (jsonData.Command)
                            {
                                case "PROD":
                                    var p = new ProductionEventArgs(client);
                                    p.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                                    p.CellId = jsonData.CellId;
                                    p.ItemId = jsonData.ItemId;
                                    p.ProcessIndicator = jsonData.RequestCode;
                                    p.SuccessIndicator = jsonData.Status;
                                    p.FaultCode = jsonData.FailureCode;
                                    p.StatusCode = jsonData.Status;
                                    p.GeneratedBarcode = (string.IsNullOrEmpty(jsonData.GeneratedBarcode)) ? "" : (jsonData.GeneratedBarcode.Length == 2) ? jsonData.GeneratedBarcode : "";
                                    

                                    var cellIdArray = Util.StringToAbIntArray(p.CellId);
                                    var itemIdArray = Util.StringToAbIntArray(p.ItemId);
                                    var generatedBarcodeArray = Util.StringToAbIntArray(p.GeneratedBarcode);

                                    //Array.Copy(cellIdArray, 0, p.ResponseArray, 0, cellIdArray.Length);
                                    //Array.Copy(itemIdArray, 0, p.ResponseArray, 5, itemIdArray.Length);
                                    //Array.Copy(generatedBarcodeArray, 0, p.ResponseArray, 11, generatedBarcodeArray.Length);
                                    
                                    short[] sArr = new short[28];
                                    var pointer = 0;

                                    for (var di = 0; di < jsonData.ProcessHistoryValues.Length; di++)
                                    {
                                        var s1 = jsonData.ProcessHistoryValues[di];
                                        if (s1.Contains("."))
                                        {
                                            double.TryParse(s1, out double dResult);
                                            var temp1 = Math.Truncate(dResult);
                                            var temp2 = dResult - temp1;
                                            var temp3 = temp2 * 100;
                                            var myArr = s1.Split('.');
                                            //short.TryParse(myArr[0], out short prodShort);
                                            sArr[pointer] = Convert.ToInt16(temp1);
                                            pointer++;
                                            //short.TryParse(myArr[1], out short prodShort1);
                                            sArr[pointer] = Convert.ToInt16(temp3);
                                            pointer++;
                                        }
                                        else
                                        {
                                            short.TryParse(s1, out short prodShort);
                                            sArr[pointer] = prodShort;
                                            pointer++;
                                            sArr[pointer] = 0;
                                            pointer++;
                                        }
                                    }

                                    p.P_Val_1 = sArr[0];
                                    p.P_Val_2 = sArr[1];
                                    p.P_Val_3 = sArr[2];
                                    p.P_Val_4 = sArr[3];
                                    p.P_Val_5 = sArr[4];
                                    p.P_Val_6 = sArr[5];
                                    p.P_Val_7 = sArr[6];
                                    p.P_Val_8 = sArr[7];
                                    p.P_Val_9 = sArr[8];
                                    p.P_Val_10 = sArr[9];
                                    p.P_Val_11 = sArr[10];
                                    p.P_Val_12 = sArr[11];
                                    p.P_Val_13 = sArr[12];
                                    p.P_Val_14 = sArr[13];
                                    p.P_Val_15 = sArr[14];
                                    p.P_Val_16 = sArr[15];
                                    p.P_Val_17 = sArr[16];
                                    p.P_Val_18 = sArr[17];
                                    p.P_Val_19 = sArr[18];
                                    p.P_Val_20 = sArr[19];
                                    p.P_Val_21 = sArr[20];
                                    p.P_Val_22 = sArr[21];
                                    p.P_Val_23 = sArr[22];
                                    p.P_Val_24 = sArr[23];
                                    p.P_Val_25 = sArr[24];
                                    p.P_Val_26 = sArr[25];
                                    p.P_Val_27 = sArr[26];
                                    p.P_Val_28 = sArr[27];

                                    p.In_Word_0 = (cellIdArray.Length > 0) ? cellIdArray[0] : Convert.ToInt16(0);
                                    p.In_Word_1 = (cellIdArray.Length > 1) ? cellIdArray[1] : Convert.ToInt16(0);
                                    p.In_Word_2 = (cellIdArray.Length > 2) ? cellIdArray[2] : Convert.ToInt16(0);
                                    p.In_Word_3 = (cellIdArray.Length > 3) ? cellIdArray[3] : Convert.ToInt16(0);
                                    p.In_Word_4 = (cellIdArray.Length > 4) ? cellIdArray[4] : Convert.ToInt16(0);
                                    p.In_Word_5 = (itemIdArray.Length > 0) ? itemIdArray[0] : Convert.ToInt16(0);
                                    p.In_Word_6 = (itemIdArray.Length > 1) ? itemIdArray[1] : Convert.ToInt16(0);
                                    p.In_Word_7 = (itemIdArray.Length > 2) ? itemIdArray[2] : Convert.ToInt16(0);
                                    p.In_Word_8 = (itemIdArray.Length > 3) ? itemIdArray[3] : Convert.ToInt16(0);
                                    p.In_Word_9 = (itemIdArray.Length > 4) ? itemIdArray[4] : Convert.ToInt16(0);
                                    p.In_Word_10 = (itemIdArray.Length > 5) ? itemIdArray[5] : Convert.ToInt16(0);
                                    p.In_Word_11 = (generatedBarcodeArray.Length > 0) ? generatedBarcodeArray[0] : Convert.ToInt16(0);

                                    p.UseJson = true;
                                    p.listenerType = ListenerType.TCP;
                                    OnProductionReceived(p);

                                    break;
                                case "SETUP":
                                    if (jsonData.RequestCode == 4 || jsonData.RequestCode == 16)
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.SenderIp = client.Client.RemoteEndPoint.ToString();
                                        s.CellId = jsonData.CellId;
                                        s.ProcessIndicator = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
                                        s.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                        OnSetupReceived(s);
                                    }
                                    if (jsonData.RequestCode == 5 || jsonData.RequestCode == 17 || jsonData.RequestCode == 6 || jsonData.RequestCode == 18)
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellId = jsonData.CellId;
                                        s.Component = jsonData.Component;
                                        s.AccessId = jsonData.AccessId;
                                        s.ProcessIndicator = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
                                        s.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                        OnSetupReceived(s);
                                    }
                                    break;
                                case "LOGIN":
                                    LoginEventArgs l;
                                    switch (jsonData.RequestCode)
                                    {
                                        case 2:
                                        case 3:
                                            l = new LoginEventArgs(client);
                                            l.CellId = jsonData.CellId;
                                            l.OperatorID = jsonData.OperatorID;
                                            l.ProcessIndicator = jsonData.RequestCode;
                                            l.UseJson = true;
                                            l.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                            OnLoginReceived(l);
                                            break;
                                        case 17:
                                            l = new LoginEventArgs(client);
                                            l.CellId = jsonData.CellId;
                                            l.ProcessIndicator = jsonData.RequestCode;
                                            l.UseJson = true;
                                            l.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                            OnLoginReceived(l);
                                            break;
                                        default:
                                            //handle wrong data size
                                            if (Logger.Enabled)
                                                Logger.Log("invalid data size");

                                            break;
                                    }
                                    break;
                                case "SERIAL":
                                    var sr = new SerialRequestEventArgs(client);
                                    sr.CellId = jsonData.CellId;
                                    sr.ProcessIndicator = jsonData.RequestCode;
                                    sr.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                    OnSerialRequestReceived(sr);

                                    break;
                                case "FINALPRINT":
                                    var lp = new LabelPrintEventArgs(client);
                                    lp.CellId = jsonData.CellId;
                                    lp.ItemId = jsonData.ItemId;
                                    lp.AlphaCode = lp.ItemId.Substring(0, 2);
                                    lp.Weight = jsonData.Weight;
                                    lp.RevLevel = jsonData.RevLevel;
                                    lp.PrinterIpAddress = jsonData.PrinterIpAddress;
                                    
                                    lp.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                    OnFinalLabelPrintReceived(lp);
                                    break;
                                case "INTERIMPRINT":

                                    break;
                                default:
                                    break;
                            }

                        }
                        catch (Exception)
                        {
                            if (Logger.Enabled)
                                Logger.Log("could not parse json data");
                        }

                    }
                    else
                    {
                        if (Logger.Enabled)
                            Logger.Log("string received");

                        try
                        {
                            //expected format
                            //command,cell id, item id, en id, process ind, success ind, faultCode, 
                            var data = Encoding.Default.GetString(receivedBytes).Trim().Split(',');
                            switch (data[0])
                            {
                                case "PROD":
                                    var dArr = new string[14];
                                    if (data.Length > 7)
                                    {
                                        var copLen = (data.Length > 20) ? 14 : data.Length - 7;
                                        dArr = new string[copLen];
                                        Array.Copy(data, 7, dArr, 0, copLen);
                                    }
                                    short result = 0;
                                    var p = new ProductionEventArgs(client);
                                    p.CellId = data[1];
                                    p.ItemId = data[2];
                                    p.GeneratedBarcode = (data[3].Length == 2) ? data[3] : "";
                                    short.TryParse(data[4], out result);
                                    p.ProcessIndicator = result;
                                    short.TryParse(data[5], out result);
                                    p.SuccessIndicator = result;
                                    short.TryParse(data[6], out result);
                                    p.FaultCode = result;
                                    p.StatusCode = 0;

                                    p.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                    var cellIdArray = Util.StringToAbIntArray(p.CellId);
                                    var itemIdArray = Util.StringToAbIntArray(p.ItemId);
                                    var generatedBarcodeArray = Util.StringToAbIntArray(p.GeneratedBarcode);

                                    //Array.Copy(cellIdArray, 0, p.ResponseArray, 0, cellIdArray.Length);
                                    //Array.Copy(itemIdArray, 0, p.ResponseArray, 5, itemIdArray.Length);
                                    //Array.Copy(generatedBarcodeArray, 0, p.ResponseArray, 11, generatedBarcodeArray.Length);

                                    short[] sArr = new short[28];
                                    var pointer = 0;
                                    for (var di = 0; di < dArr.Length; di++)
                                    {
                                        var s1 = dArr[di];
                                        if (s1.Contains("."))
                                        {
                                            double.TryParse(s1, out double dResult);
                                            var temp1 = Math.Truncate(dResult);
                                            var temp2 = dResult - temp1;
                                            var temp3 = temp2 * 100;
                                            var myArr = s1.Split('.');
                                            //short.TryParse(myArr[0], out short prodShort);
                                            sArr[pointer] = Convert.ToInt16(temp1);
                                            pointer++;
                                            //short.TryParse(myArr[1], out short prodShort1);
                                            sArr[pointer] = Convert.ToInt16(temp3);
                                            pointer++;
                                        }
                                        else
                                        {
                                            short.TryParse(s1, out short prodShort);
                                            sArr[pointer] = prodShort;
                                            pointer++;
                                            sArr[pointer] = 0;
                                            pointer++;
                                        }
                                    }

                                    p.P_Val_1 = sArr[0];
                                    p.P_Val_2 = sArr[1];
                                    p.P_Val_3 = sArr[2];
                                    p.P_Val_4 = sArr[3];
                                    p.P_Val_5 = sArr[4];
                                    p.P_Val_6 = sArr[5];
                                    p.P_Val_7 = sArr[6];
                                    p.P_Val_8 = sArr[7];
                                    p.P_Val_9 = sArr[8];
                                    p.P_Val_10 = sArr[9];
                                    p.P_Val_11 = sArr[10];
                                    p.P_Val_12 = sArr[11];
                                    p.P_Val_13 = sArr[12];
                                    p.P_Val_14 = sArr[13];
                                    p.P_Val_15 = sArr[14];
                                    p.P_Val_16 = sArr[15];
                                    p.P_Val_17 = sArr[16];
                                    p.P_Val_18 = sArr[17];
                                    p.P_Val_19 = sArr[18];
                                    p.P_Val_20 = sArr[19];
                                    p.P_Val_21 = sArr[20];
                                    p.P_Val_22 = sArr[21];
                                    p.P_Val_23 = sArr[22];
                                    p.P_Val_24 = sArr[23];
                                    p.P_Val_25 = sArr[24];
                                    p.P_Val_26 = sArr[25];
                                    p.P_Val_27 = sArr[26];
                                    p.P_Val_28 = sArr[27];

                                    p.In_Word_0 = (cellIdArray.Length > 0) ? cellIdArray[0] : Convert.ToInt16(0);
                                    p.In_Word_1 = (cellIdArray.Length > 1) ? cellIdArray[1] : Convert.ToInt16(0);
                                    p.In_Word_2 = (cellIdArray.Length > 2) ? cellIdArray[2] : Convert.ToInt16(0);
                                    p.In_Word_3 = (cellIdArray.Length > 3) ? cellIdArray[3] : Convert.ToInt16(0);
                                    p.In_Word_4 = (cellIdArray.Length > 4) ? cellIdArray[4] : Convert.ToInt16(0);
                                    p.In_Word_5 = (itemIdArray.Length > 0) ? itemIdArray[0] : Convert.ToInt16(0);
                                    p.In_Word_6 = (itemIdArray.Length > 1) ? itemIdArray[1] : Convert.ToInt16(0);
                                    p.In_Word_7 = (itemIdArray.Length > 2) ? itemIdArray[2] : Convert.ToInt16(0);
                                    p.In_Word_8 = (itemIdArray.Length > 3) ? itemIdArray[3] : Convert.ToInt16(0);
                                    p.In_Word_9 = (itemIdArray.Length > 4) ? itemIdArray[4] : Convert.ToInt16(0);
                                    p.In_Word_10 = (itemIdArray.Length > 5) ? itemIdArray[5] : Convert.ToInt16(0);
                                    p.In_Word_11 = (generatedBarcodeArray.Length > 0) ? generatedBarcodeArray[0] : Convert.ToInt16(0);

                                    p.UseJson = false;
                                    OnProductionReceived(p);
                                    break;
                                case "SETUP":
                                    if (data.Length == 5)
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellId = data[1];
                                        short.TryParse(data[2], out short setupShort);
                                        s.ProcessIndicator = setupShort;
                                        s.ModelNumber = data[3];
                                        s.OpNumber = data[4];
                                        s.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                        OnSetupReceived(s);
                                    }
                                    if (data.Length == 7)
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                                        s.CellId = data[1];
                                        s.Component = data[2];
                                        short.TryParse(data[3], out short setupShort);
                                        s.AccessId = setupShort;
                                        short.TryParse(data[4], out short setupShort1);
                                        s.ProcessIndicator = setupShort1;
                                        s.ModelNumber = data[5];
                                        s.OpNumber = data[6];

                                        OnSetupReceived(s);
                                    }
                                    // get error for wrong length
                                    break;
                                case "LOGIN":
                                    LoginEventArgs l;
                                    switch (data.Length)
                                    {
                                        case 4:
                                            l = new LoginEventArgs(client);
                                            l.CellId = data[1];
                                            l.OperatorID = data[2];
                                            short.TryParse(data[3], out short loginResult);
                                            l.ProcessIndicator = loginResult;
                                            l.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                            OnLoginReceived(l);
                                            break;
                                        case 3:
                                            l = new LoginEventArgs(client);
                                            l.CellId = data[1];
                                            short.TryParse(data[2], out short loginResult1);
                                            l.ProcessIndicator = loginResult1;
                                            l.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                            OnLoginReceived(l);
                                            break;
                                        default:
                                            if (Logger.Enabled)
                                                Logger.Log("invalid data size");

                                            break;
                                    }
                                    break;
                                case "SERIAL":
                                    var sr = new SerialRequestEventArgs(client);
                                    sr.CellId = data[1];
                                    short.TryParse(data[2], out short res1);
                                    sr.ProcessIndicator = res1;
                                    sr.SenderIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                                    OnSerialRequestReceived(sr);
                                    break;
                                default:
                                    if (Logger.Enabled)
                                        Logger.Log("invalid format");
                                    break;
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }
                }
            }
            catch
            {
                if (Logger.Enabled)
                    Logger.Log("connection error");
            }
            if (client.Connected)
                client.GetStream().Close();

            client.Close();
            stream.Flush();
        }

    }
}
