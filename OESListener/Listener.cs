using System;
using System.Diagnostics;

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

        protected virtual void OnSerialNumberRequest(SerialRequestEventArgs e)
        {
            SerialRequestReceived?.Invoke(this, e);
        }
        public event EventHandler<SerialRequestEventArgs> SerialRequestReceived;

        protected virtual void OnLabelPrintReceived(LabelPrintEventArgs e)
        {
            if (!PrintFromFile)
            {
                LabelPrintReceived?.Invoke(this, e);
            }
            else
            {
                var p = new LabelPrinter(e);
                p.UseFile = true;
                var result = p.PrintLabel();
                e.Response = result.ToString();
                if (Logger.Enabled)
                    Logger.Log(result.ToString());

                var resp = new ListenerResponse();
                resp.FinalPrintResponse(e);
            }
            
        }
        public event EventHandler<LabelPrintEventArgs> LabelPrintReceived;

        public string myIPAddress { get; set; }
        public int TcpPort { get; set; }

        public bool PrintFromFile = false;

        string fileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

        public string GetVersion()
        {
            return fileVersion;
        }

        public Listener()
        {
            TcpPort = 55001;
            myIPAddress = "127.0.0.1";

            if (Logger.Enabled)
                Logger.Log("logging enabled");
        }

        public Listener(string ipAddress)
        {
            myIPAddress = ipAddress;
            TcpPort = 55001;

            if (Logger.Enabled)
                Logger.Log("logging enabled");
        }

        public Listener(string ipAddress, int port)
        {
            myIPAddress = ipAddress;
            TcpPort = port;

            if (Logger.Enabled)
                Logger.Log("logging enabled");
        }

        public void Listen()
        {
            var tcpListener = new TcpListener(myIPAddress);
            tcpListener.Listen();
            tcpListener.LoginReceived += TcpListener_LoginReceived;
            tcpListener.SetupReceived += TcpListener_SetupReceived;
            tcpListener.ProductionReceived += TcpListener_ProductionReceived;
            tcpListener.SerialRequestReceived += TcpListener_SerialRequestReceived;
            tcpListener.FinalLabelPrintReceived += TcpListener_FinalLabelPrintReceived;

            var pcccListener = new PcccListener(myIPAddress);
            pcccListener.Listen();
            pcccListener.LoginReceived += PcccListener_LoginReceived;
            pcccListener.SetupReceived += PcccListener_SetupReceived;
            pcccListener.ProductionReceived += PcccListener_ProductionReceived;
            pcccListener.SerialRequestReceived += PcccListener_SerialRequestReceived;
            pcccListener.FinalLabelPrintReceived += PcccListener_FinalLabelPrintReceived;
            var eipListener = new EipListener(myIPAddress);
            eipListener.Listen();
            eipListener.ProductionReceived += EipListener_ProductionReceived;
            eipListener.SetupReceived += EipListener_SetupReceived;
            eipListener.LoginReceived += EipListener_LoginReceived;
            eipListener.SerialRequestReceived += EipListener_SerialRequestReceived;
            eipListener.FinalLabelPrintReceived += EipListener_FinalLabelPrintReceived;
            //
        }

        private void PcccListener_FinalLabelPrintReceived(object sender, LabelPrintEventArgs e)
        {
            OnLabelPrintReceived(e);
        }

        private void TcpListener_FinalLabelPrintReceived(object sender, LabelPrintEventArgs e)
        {
            OnLabelPrintReceived(e);
        }

        private void EipListener_FinalLabelPrintReceived(object sender, LabelPrintEventArgs e)
        {
            OnLabelPrintReceived(e);
        }

        private void TcpListener_ProductionReceived(object sender, ProductionEventArgs e)
        {
            OnProductionReceived(e);
        }

        private void TcpListener_SetupReceived(object sender, SetupEventArgs e)
        {
            OnSetupReceived(e);
        }

        private void TcpListener_LoginReceived(object sender, LoginEventArgs e)
        {
            OnLoginReceived(e);
        }

        private void TcpListener_SerialRequestReceived(object sender, SerialRequestEventArgs e)
        {
            OnSerialNumberRequest(e);
        }

        private void PcccListener_ProductionReceived(object sender, ProductionEventArgs e)
        {
            OnProductionReceived(e);
        }

        private void PcccListener_SetupReceived(object sender, SetupEventArgs e)
        {
            OnSetupReceived(e);
        }

        private void PcccListener_LoginReceived(object sender, LoginEventArgs e)
        {
            OnLoginReceived(e);
        }

        private void PcccListener_SerialRequestReceived(object sender, SerialRequestEventArgs e)
        {
            OnSerialNumberRequest(e);
        }

        private void EipListener_LoginReceived(object sender, LoginEventArgs e)
        {
            OnLoginReceived(e);
        }

        private void EipListener_SetupReceived(object sender, SetupEventArgs e)
        {
            OnSetupReceived(e);
        }

        private void EipListener_ProductionReceived(object sender, ProductionEventArgs e)
        {
            OnProductionReceived(e);
        }

        private void EipListener_SerialRequestReceived(object sender, SerialRequestEventArgs e)
        {
            OnSerialNumberRequest(e);
        }
        
    }
}
