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

        protected virtual void OnLabelPrintReceived(LabelPrintEventArgs e)
        {
            LabelPrintReceived?.Invoke(this, e);
        }
        public event EventHandler<LabelPrintEventArgs> LabelPrintReceived;

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
                                    p.CellId = jsonData.CellId;
                                    p.ItemId = jsonData.ItemId;
                                    p.Request = jsonData.RequestCode;
                                    p.Status = jsonData.Status;
                                    p.FailureCode = jsonData.FailureCode;
                                    p.ProcessHistoryValues = new List<string>(jsonData.ProcessHistoryValues);
                                    p.UseJson = true;
                                    p.listenerType = ListenerType.TCP;
                                    OnProductionReceived(p);
                                    break;
                                case "SETUP":
                                    if (jsonData.RequestCode == "4" || jsonData.RequestCode == "16")
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellId = jsonData.CellId;
                                        s.Request = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
                                        s.listenerType = ListenerType.TCP;
                                        OnSetupReceived(s);
                                    }
                                    if (jsonData.RequestCode == "5" || jsonData.RequestCode == "17" || jsonData.RequestCode == "6" || jsonData.RequestCode == "18")
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellId = jsonData.CellId;
                                        s.Component = jsonData.Component;
                                        s.AccessId = jsonData.AccessId;
                                        s.Request = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
                                        s.listenerType = ListenerType.TCP;
                                        OnSetupReceived(s);
                                    }
                                    break;
                                case "LOGIN":
                                    LoginEventArgs l;
                                    switch (jsonData.RequestCode)
                                    {
                                        case "2":
                                        case "3":
                                            l = new LoginEventArgs(client);
                                            l.CellId = jsonData.CellId;
                                            l.OperatorID = jsonData.OperatorID;
                                            l.Request = jsonData.RequestCode;
                                            l.UseJson = true;
                                            l.listenerType = ListenerType.TCP;
                                            OnLoginReceived(l);
                                            break;
                                        case "17":
                                            l = new LoginEventArgs(client);
                                            l.CellId = jsonData.CellId;
                                            l.Request = jsonData.RequestCode;
                                            l.UseJson = true;
                                            l.listenerType = ListenerType.TCP;
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
                                    sr.Request = jsonData.RequestCode;
                                    sr.listenerType = ListenerType.TCP;
                                    OnSerialRequestReceived(sr);

                                    break;
                                case "PRINT":
                                    var lp = new LabelPrintEventArgs(client);
                                    lp.CellId = jsonData.CellId;
                                    lp.ItemId = jsonData.ItemId;
                                    lp.AlphaCode = lp.ItemId.Substring(0, 2);
                                    lp.Weight = jsonData.Weight;
                                    lp.RevLevel = jsonData.RevLevel;
                                    lp.PrinterIpAddress = jsonData.PrinterIpAddress;
                                    lp.PrintType = jsonData.PrintType;
                                    lp.InterimFile = jsonData.InterimFile;
                                    lp.listenerType = ListenerType.TCP;
                                    OnLabelPrintReceived(lp);
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
                            var data = Encoding.Default.GetString(receivedBytes).Trim().Split(',');
                            switch (data[0])
                            {
                                case "PROD":
                                    var dArr = new string[14];
                                    if (data.Length > 6)
                                    {
                                        var copLen = (data.Length > 20) ? 14 : data.Length - 6;
                                        Array.Copy(data, 6, dArr, 0, copLen);
                                    }
                                    var p = new ProductionEventArgs(client, data[1], data[2], data[3], data[4], data[5], dArr);
                                    p.UseJson = false;
                                    p.listenerType = ListenerType.TCP;
                                    OnProductionReceived(p);
                                    break;
                                case "SETUP":
                                    if (data.Length == 5)
                                    {
                                        var s = new SetupEventArgs(client, data[1], data[2], data[3], data[4]);
                                        s.listenerType = ListenerType.TCP;
                                        OnSetupReceived(s);
                                    }
                                    if (data.Length == 7)
                                    {
                                        var s = new SetupEventArgs(client, data[1], data[2], data[3], data[4], data[5], data[6]);
                                        s.listenerType = ListenerType.TCP;
                                        OnSetupReceived(s);
                                    }
                                    // get error for wrong length
                                    break;
                                case "LOGIN":
                                    LoginEventArgs l;
                                    switch (data.Length)
                                    {
                                        case 4:
                                            l = new LoginEventArgs(client, data[1], data[2], data[3]);
                                            l.listenerType = ListenerType.TCP;
                                            OnLoginReceived(l);
                                            break;
                                        case 3:
                                            l = new LoginEventArgs(client, data[1], data[2]);
                                            l.listenerType = ListenerType.TCP;
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
                                    sr.Request = data[2];
                                    sr.listenerType = ListenerType.TCP;
                                    OnSerialRequestReceived(sr);
                                    break;
                                case "PRINT":
                                    var lp = new LabelPrintEventArgs(client);
                                    lp.CellId = data[1];
                                    lp.ItemId = data[2];
                                    lp.AlphaCode = lp.ItemId.Substring(0, 2);
                                    lp.Weight = data[3];
                                    lp.RevLevel = data[4];
                                    lp.PrinterIpAddress = data[5];
                                    lp.PrintType = data[6];
                                    lp.InterimFile = data[7];
                                    lp.listenerType = ListenerType.TCP;
                                    OnLabelPrintReceived(lp);
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
