using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //OESListener.WebServer webServer = new OESListener.WebServer();
            //webServer.StartServer();
            //var tempData = new short[70];
            //tempData[68] = 1;
            //tempData[69] = 1;

            //var tempData1 = new short[5];
            //tempData1[0] = 10;

            //var setupWriter = new OESListener.PlcWriter();
            //var tempREs = setupWriter.MicroLogixResponse("10.53.21.125", tempData1, "N241:0");
            //var tempRes = setupWriter.MicroLogixResponse("10.53.27.50", tempData1, "N240:0");

            //var production = new OesWriter.ProductionTransaction();
            //production.CellId = "K04902";
            //production.ItemId = "QG912345678";
            //production.RequestCode = 0;
            //production.Status = 0;
            //production.ProcessHistoryValues = new string[5] { "1.1", "2.2", "3.3", "4", "5" };

            //production.SendTransaction("10.50.5.34", 55001);

            //Console.WriteLine(production.ResponseString);

            //var result = production.SendCsvTransaction("10.50.5.34");

            //Console.WriteLine(result);

            //var setuptrans = new OesWriter.SetupTransaction();
            //setuptrans.CellId = "K04902";
            //setuptrans.RequestCode = 4;
            //setuptrans.ModelNumber = "B0871400-00";
            //setuptrans.OpNumber = "49";

            //var setupresponse = setuptrans.SendCsvTransaction("10.50.5.34");

            //Console.WriteLine(setupresponse.ResponseString);

            //var pt = new OesWriter.LoginTransaction();
            //pt.CellId = "130401";
            //pt.OperatorId = "50034";
            //pt.RequestCode = 3;


            //var sr = pt.SendCsvTransaction("10.50.5.34");

            //Console.WriteLine(sr);

            //var serial = new OesWriter.SerialRequest();
            //serial.CellId = "130401";


            //Console.WriteLine(serial.SendTransaction("10.50.5.34"));

            //pt = new OesWriter.SetupTransaction();
            //pt.CellId = "K04902";
            //pt.ModelNumber = "B0871400-00";
            //pt.OpNumber = "49";
            //pt.RequestCode = 5;
            //pt.AccessId = 1;
            //pt.Component = "34661S1491375";

            //pt.SendTransaction("10.50.5.34", 55001);

            //Console.WriteLine(pt.ResponseString);

            //pt = new OesWriter.SetupTransaction();
            //pt.CellId = "K04902";
            //pt.ModelNumber = "B0871400-00";
            //pt.OpNumber = "49";
            //pt.RequestCode = 5;
            //pt.AccessId = 2;
            //pt.Component = "TRYD28210017";

            //pt.SendTransaction("10.50.5.34", 55001);

            //Console.WriteLine(pt.ResponseString);

            //pt = new OesWriter.SetupTransaction();
            //pt.CellId = "K04902";
            //pt.ModelNumber = "B0871400-00";
            //pt.OpNumber = "49";
            //pt.RequestCode = 5;
            //pt.AccessId = 3;
            //pt.Component = "85071580D8W05";

            //pt.SendTransaction("10.50.5.34", 55001);

            //Console.WriteLine(pt.ResponseString);

            //Console.Read();

            //var testArr = new string[]{ "1.1", "2.2", "3", "4.4", "5.5", "6"};
            //var realArr = new List<double>();
            //var intArr = new List<int>();

            //foreach (var item in testArr)
            //{
            //    double.TryParse(item, out double result);
            //    realArr.Add(result);
            //    intArr.Add((int)Math.Truncate(result));
            //}

            ////Console.Read();
            //var p = new OESListener.PlcWriter();
            //var data = new short[10];
            //data[0] = 15;
            //data[1] = 4;
            //data[2] = 3;
            ////data[10] = 10000;
            ////data[49] = 2;

            ////p.LogixResponse("10.50.201.100", data, "n228[0]");
            ////var status = p.LogixResponse("10.64.41.165", data, "N198[0]");
            //var status = p.LogixResponse("10.50.71.117", data, "N198[0]");
            //var status1 = p.PlcResponse("10.50.193.133", data, "N199:50");
            //var status2 = p.MicroLogixResponse("10.53.29.46", data, "N199:10");




            //Console.Read();

            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            //startInfo.FileName = @"C:\OES\ws\server.exe";
            //startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = null;

            //try
            //{
            //    // Start the process with the info we specified.
            //    // Call WaitForExit and then the using statement will close.
            //    using (Process exeProcess = Process.Start("C:\\OES\\ws\\server.exe"))
            //    {
            //        exeProcess.WaitForExit();
            //    }
            //}
            //catch
            //{
            //    // Log error.
            //}

            var l = new OESListener.Listener("10.50.71.124");

            //var l = new OESListener.Listener("127.0.0.1");
            l.Listen();
            
            l.PrintFromFile = true;
            l.LoginReceived += L_LoginReceived;
            l.SetupReceived += L_SetupReceived;
            l.ProductionReceived += L_ProductionReceived;
            l.SerialRequestReceived += L_SerialRequestReceived;
            l.LabelPrintReceived += L_LabelPrintReceived;

            //OESListener.PlcWriter p = new OESListener.PlcWriter();
            //p.MicroLogixResponse("10.53.29.46", new short[3] { 1700, 500, 500 }, "N219:0");
            //p.MicroLogixResponse("10.53.29.46", new short[10] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 }, "N219:0");

            OESListener.Logger.LogPath = @"C:\OesLog";
            OESListener.Logger.LogToFile = false;
            OESListener.Logger.LogToConsole = true;
            OESListener.Logger.EnableDllLogging = true;

            OESListener.Logger.Log("here is a message");


            //OESListener.WebServer webServer = new OESListener.WebServer();
            //webServer.StartServer();

            Console.Read();

        }

        private static void L_LabelPrintReceived(object sender, OESListener.LabelPrintEventArgs e)
        {
            e.PrintCode = @"^XA^JZN^POI^PQ1^BY3,3,100,^LH070,030^FS
^FO0065,040^B3N,N,160,N,N^FD*_SerialNumber_*^FS
^FO065,210,^A0N,30,30^FDP/N : B0656800-01        USA ARC: _SerialNumber_^FS
^FO065,380,^A0N,27,27^FDDOT-SP 12122/5300 M4636 UN3268: EX2000060075^FS
^FO065,410,^A0N,27,27^FDEINFUHRER:^FS
^FO126,440,^A0N,27,27^FDTEL.^FS
^FO520,530,^A0N,27,27^FDBAM PT -1073^FS
^FO605,542,^A0N,27,27^FD1^FS
^FO520,570,^A0N,27,27^FDBA01772^FS
~DGR:0AF4A8C3,00064,004,0FFF80,1FFF80,3FFF80,2318,4318,0318,
0318,0318,0718,0618,0618,0E1880,0E1D80,1E1F80,1C0F,
1C0E,^FO155,285^XG0AF4A8C3,1,1^FS
^FO065,570,^A0N,27,27^FDHERSTELLUNG: _Year_. KNOXVILLE^FS
^FO100,250,^A0N,30,30^FD1.8mm .196KG 0.044L PW332.8 PH499.2BAR^FS
^FO180,280,^A0N,30,30^FD 0531 -40C A  JS _Year_ / _Month_^FS
^FO240,410,^A0N,27,27^FD TAKATA-PETRI AG^FS
^FO240,440,^A0N,27,27^FD BAHNWEG 1^FS
^FO240,470,^A0N,27,27^FD 63743 ASCHAFFENBURG^FS
^FO065,470,^A0N,20,20^FD+49 6021-65-0(83)^FS
^FO150,325,^A0N,30,30^FD_Weight_^FS
^BY2,2,50
^FO0300,320^BCN,040,N,N^FD_Weight_^FS^XZ";
            var resp = new OESListener.ListenerResponse();
            resp.FinalPrintResponse(e);
        }

        private static void L_SerialRequestReceived(object sender, OESListener.SerialRequestEventArgs e)
        {
            var resp = new OESListener.ListenerResponse();
            e.ResponseArray[0] = 22597;
            e.ResponseArray[1] = 13893;
            e.ResponseArray[2] = 12592;
            e.ResponseArray[3] = 13616;

            e.ItemId = "new serial number";
            resp.SerialRequestResponse(e);
        }

        private static void L_ProductionReceived(object sender, OESListener.ProductionEventArgs e)
        {
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
            e.ResponseArray[65] = 1925;
            e.ResponseArray[66] = 0;
            e.PlcModelSetup = new short[] { 1 , 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
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
