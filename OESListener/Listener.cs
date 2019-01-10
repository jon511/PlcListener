using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

[assembly: CLSCompliant(true)]
namespace OESListener
{
    public class Listener
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

        private static readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        public string myIPAddress { get; set; }
        public int Port { get; set; }
        
        public Listener()
        {
            Port = 55001;
            myIPAddress = "127.0.0.1";
        }

        public Listener(string ipAddress)
        {
            myIPAddress = ipAddress;
            Port = 55001;
        }
        
        public Listener(string ipAddress, int port)
        {
            myIPAddress = ipAddress;
            Port = port;
        }

        public void Listen()
        {
            
            Thread myNewThread = new Thread(() => StartListening());
            if (Logger.Enabled)
                Console.WriteLine("Starting new thread");

            myNewThread.Start();
        }

        private void StartListening()
        {
            Console.WriteLine("Listening...");

            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse(myIPAddress), Port);
                server.Start();

                while (true)
                {
                    AllDone.Reset();
                    if (Logger.Enabled)
                        Logger.Log("waiting for connection...");
                        //Logger.Log("Ready for connection...");
                        
                        //Console.WriteLine("Ready for connection...");

                    var client = server.AcceptTcpClient();

                    if (Logger.Enabled)
                        Logger.Log("connected");

                    new Thread(() => HandleMessage(client)).Start();
                }
            }
            catch (SocketException e)
            {
                Logger.Log(string.Format("Socket Error : {0}", e));
                //Console.WriteLine("SocketException: ${0}", e);
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
                                    p.CellID = jsonData.CellId;
                                    p.ItemID = jsonData.ItemId;
                                    p.Request = jsonData.RequestCode;
                                    p.Status = jsonData.Status;
                                    p.FailureCode = jsonData.FailureCode;
                                    p.ProcessHistoryValues = new List<string>(jsonData.ProcessHistoryValues);
                                    p.UseJson = true;
                                    OnProductionReceived(p);
                                    break;
                                case "SETUP":
                                    if (jsonData.RequestCode == "4" || jsonData.RequestCode == "16")
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellID = jsonData.CellId;
                                        s.Request = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
                                        OnSetupReceived(s);
                                    }
                                    if (jsonData.RequestCode == "5" || jsonData.RequestCode == "17" || jsonData.RequestCode == "6" || jsonData.RequestCode == "18")
                                    {
                                        var s = new SetupEventArgs(client);
                                        s.CellID = jsonData.CellId;
                                        s.Component = jsonData.Component;
                                        s.AccessId = jsonData.AccessId;
                                        s.Request = jsonData.RequestCode;
                                        s.ModelNumber = jsonData.ModelNumber;
                                        s.OpNumber = jsonData.OpNumber;
                                        s.UseJson = true;
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
                                            l.CellID = jsonData.CellId;
                                            l.OperatorID = jsonData.OperatorID;
                                            l.Request = jsonData.RequestCode;
                                            l.UseJson = true;
                                            OnLoginReceived(l);
                                            break;
                                        case "17":
                                            l = new LoginEventArgs(client);
                                            l.CellID = jsonData.CellId;
                                            l.Request = jsonData.RequestCode;
                                            l.UseJson = true;
                                            OnLoginReceived(l);
                                            break;
                                        default:
                                            //handle wrong data size
                                            Console.WriteLine("invalid data size");
                                            break;
                                    }
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
                                    OnProductionReceived(p);
                                    break;
                                case "SETUP":
                                    if (data.Length == 5)
                                    {
                                        var s = new SetupEventArgs(client, data[1], data[2], data[3], data[4]);
                                        OnSetupReceived(s);
                                    }
                                    if (data.Length == 7)
                                    {
                                        var s = new SetupEventArgs(client, data[1], data[2], data[3], data[4], data[5], data[6]);
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
                                            OnLoginReceived(l);
                                            break;
                                        case 3:
                                            l = new LoginEventArgs(client, data[1], data[2]);
                                            OnLoginReceived(l);
                                            break;
                                        default:
                                            
                                            Console.WriteLine("invalid data size");
                                            break;
                                    }
                                    break;
                                default:
                                    Console.WriteLine("invalid format");
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

        public static void ProductionResponse(ProductionEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellID);
                sb.Append(",");
                sb.Append(e.ItemID);
                sb.Append(",");
                sb.Append(e.Request);
                sb.Append(",");
                sb.Append(e.Status);
                sb.Append(",");
                sb.Append(e.FailureCode);
                sb.Append(",");
                sb.Append(string.Join(",", e.ProcessHistoryValues));
                responseString = sb.ToString();
            }
            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public static void SetupResponse(SetupEventArgs e)
        {
            string responseString;
            
            if (e.UseJson)
            {
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e.Response);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.Response.Component1.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component1.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Component2.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component2.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Component3.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component3.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Component4.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component4.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Component5.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component5.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Component6.AccessId);
                sb.Append(",");
                sb.Append(e.Response.Component6.ModelNumber);
                sb.Append(",");
                sb.Append(e.Response.Quantity);
                sb.Append(",");
                sb.Append(e.Response.Acknowledge);
                sb.Append(",");
                sb.Append(e.Response.ErrorCode);
                sb.Append(",");
                sb.Append(string.Join(",", e.Response.PlcModelSetup));
                responseString = sb.ToString();
            }

            var stream = e.Client.GetStream();
            
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public static void LoginResponse(LoginEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                responseString = $"{e.CellID}, {e.OperatorID}, {e.Status}, {e.FailureCode}";
            }

            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString.ToString());
            stream.Write(outData, 0, outData.Length);
        }


    }
}
