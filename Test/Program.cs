using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            var l = new OESListener.Listener();
            l.Listen();
            l.LoginReceived += L_LoginReceived;
            l.SetupReceived += L_SetupReceived;
            l.ProductionReceived += L_ProductionReceived;
            

            Console.Read();

        }

        private static void L_ProductionReceived(object sender, OESListener.ProductionEventArgs e)
        {
            OESListener.Logger.Log(e.CellID);
            OESListener.Logger.Log(e.ItemID);
            OESListener.Listener.ProductionResponse(e);
            OESListener.Logger.Log("production response");
        }

        private static void L_SetupReceived(object sender, OESListener.SetupEventArgs e)
        {
            e.Response.Component1.AccessId = "1";
            e.Response.Component1.ModelNumber = "B0884400-00";
            e.Response.Acknowledge = "1";
            e.Response.ErrorCode = "1";
            e.Response.PlcModelSetup = new string[] { "1.1", "2.2", "3.3", "4.4", "5.5", "6.6", "7.7", "8.8", "9.9" };
            OESListener.Listener.SetupResponse(e);
        }

        private static void L_LoginReceived(object sender, OESListener.LoginEventArgs e)
        {

            OESListener.Listener.LoginResponse(e);
        }
    }
}
