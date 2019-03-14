using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            //var testArr = new string[]{ "1.1", "2.2", "3", "4.4", "5.5", "6"};
            //var realArr = new List<double>();
            //var intArr = new List<int>();

            //foreach (var item in testArr)
            //{
            //    double.TryParse(item, out double result);
            //    realArr.Add(result);
            //    intArr.Add((int)Math.Truncate(result));
            //}

            //Console.Read();
            //var p = new OESListener.PlcWriter();
            //var data = new short[10];
            //data[0] = 15;
            //data[1] = 4;
            //data[2] = 3;
            ////data[10] = 10000;
            ////data[49] = 2;

            ////p.LogixResponse("10.50.196.160", data, "N218");


            //Task SendWrite = new Task(() =>
            //{
            //    p.LogixResponse("10.50.196.160", data, "N218");
            //});
            //SendWrite.Start();

            //Console.Read();

            //var p1 = new OESListener.PlcWriter();
            //data = new short[10];
            //data[0] = 0;
            //data[1] = 1;
            //data[2] = 2;

            //Task NewWrite = new Task(() =>
            //{
            //    p1.LogixResponse("10.50.196.160", data, "N218");
            //});
            //NewWrite.Start();

            //Console.Read();

            var l = new OESListener.Listener("192.168.86.215");
            OESListener.Logger.EnableDllLogging = false;
            OESListener.Logger.LogToConsole = false;
            OESListener.Logger.LogToFile = true;
            OESListener.Logger.LogPath = @"C:\OesLog\";
            OESListener.Logger.Log("test message");
            OESListener.Logger.Log("test message 2");
            OESListener.Logger.Log("test message 3");
            OESListener.Logger.Log("test message 4");
            OESListener.Logger.Log("test message 5");
            OESListener.Logger.Log("test message 6");

            //var l = new OESListener.Listener("127.0.0.1");
            l.Listen();
            l.PrintFromFile = true;
            l.LoginReceived += L_LoginReceived;
            l.SetupReceived += L_SetupReceived;
            l.ProductionReceived += L_ProductionReceived;
            l.SerialRequestReceived += L_SerialRequestReceived;
            
            Console.Read();

        }

        private static void L_SerialRequestReceived(object sender, OESListener.SerialRequestEventArgs e)
        {
            var resp = new OESListener.ListenerResponse();
            e.ItemId = "new serial number";
            resp.SerialRequestResponse(e);
        }

        private static void L_ProductionReceived(object sender, OESListener.ProductionEventArgs e)
        {

            //var number = new Random();
            //var rand = number.Next(10, 150);
            //Thread.Sleep(rand);
            //e.CellId = "newCell";
            //e.ItemId = "itemID";
            e.P_Val_1 = 101;
            e.P_Val_2 = 10000;
            e.P_Val_3 = 9999;
            e.P_Val_4 = 4444;

            var resp = new OESListener.ListenerResponse();
            resp.ProductionResponse(e);
            
        }

        private static void L_SetupReceived(object sender, OESListener.SetupEventArgs e)
        {
            
            OESListener.Logger.Log("setup response");
            e.Response.Component1.AccessId = "1";
            e.Response.Component1.ModelNumber = "B0884400-00";
            e.Response.Acknowledge = "1";
            e.Response.ErrorCode = "1";
            e.Response.PlcModelSetup = new string[] { "1.1", "2.2", "3.3", "4.4", "5.5", "6.6", "7.7", "8.8", "9.9" };
            var resp = new OESListener.ListenerResponse();
            resp.SetupResponse(e);
        }

        private static void L_LoginReceived(object sender, OESListener.LoginEventArgs e)
        {
            e.CellId = "";
            e.OperatorID = "";
            e.SuccessIndicator = 1;
            e.ResponseArray = new short[34];

            var resp = new OESListener.ListenerResponse();
            resp.LoginResponse(e);
        }
    }
}
