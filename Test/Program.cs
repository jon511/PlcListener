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

            var l = new OESListener.Listener("10.50.71.118");

            //var l = new OESListener.Listener("127.0.0.1");
            l.Listen();
            l.PrintFromFile = true;
            l.LoginReceived += L_LoginReceived;
            l.SetupReceived += L_SetupReceived;
            l.ProductionReceived += L_ProductionReceived;
            l.SerialRequestReceived += L_SerialRequestReceived;

            //OESListener.PlcWriter p = new OESListener.PlcWriter();
            //p.MicroLogixResponse("10.53.29.46", new short[3] { 1700, 500, 500 }, "N219:0");
            //p.MicroLogixResponse("10.53.29.46", new short[10] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 }, "N219:0");

            OESListener.Logger.LogPath = @"C:\OesLog";
            OESListener.Logger.LogToFile = true;
            OESListener.Logger.Log("here is a message");

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
            e.ResponseArray[18] = e.ProcessIndicator;
            e.ResponseArray[19] = e.SuccessIndicator;
            e.ResponseArray[20] = e.FaultCode;
            e.ResponseArray[21] = 308;

            e.ResponseArray[22] = e.P_Val_1;
            e.ResponseArray[23] = e.P_Val_2;
            e.ResponseArray[24] = e.P_Val_3;
            e.ResponseArray[25] = e.P_Val_4;
            e.ResponseArray[26] = e.P_Val_5;
            e.ResponseArray[27] = e.P_Val_6;
            e.ResponseArray[28] = e.P_Val_7;
            e.ResponseArray[29] = e.P_Val_8;
            e.ResponseArray[30] = e.P_Val_9;
            e.ResponseArray[31] = e.P_Val_10;
            e.ResponseArray[32] = e.P_Val_11;
            e.ResponseArray[33] = e.P_Val_12;
            e.ResponseArray[34] = e.P_Val_13;
            e.ResponseArray[35] = e.P_Val_14;
            e.ResponseArray[36] = e.P_Val_15;
            e.ResponseArray[37] = e.P_Val_16;
            e.ResponseArray[38] = e.P_Val_17;
            e.ResponseArray[39] = e.P_Val_18;
            e.ResponseArray[40] = e.P_Val_19;
            e.ResponseArray[41] = e.P_Val_20;
            e.ResponseArray[42] = e.P_Val_21;
            e.ResponseArray[43] = e.P_Val_22;
            e.ResponseArray[44] = e.P_Val_23;
            e.ResponseArray[45] = e.P_Val_24;
            e.ResponseArray[46] = e.P_Val_25;
            e.ResponseArray[47] = e.P_Val_26;
            e.ResponseArray[48] = e.P_Val_27;
            e.ResponseArray[49] = e.P_Val_28;


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
