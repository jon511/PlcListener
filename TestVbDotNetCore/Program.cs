using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OesListener;

namespace TestVbDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {

            var l = new Listener();
            l.Listen();
            l.ProductionReceived += L_ProductionReceived;
            l.SetupReceived += L_SetupReceived;
            l.LoginReceived += L_LoginReceived;
            l.SerialRequestReceived += L_SerialRequestReceived;
            l.LabelPrintReceived += L_LabelPrintReceived;

            Logger.EnableDllLogging = true;
            Logger.LogToConsole = true;
            Logger.LogToFile = false;



            Console.ReadLine();

        }

        private static void L_LabelPrintReceived(object sender, LabelPrintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void L_SerialRequestReceived(object sender, SerialRequestEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void L_LoginReceived(object sender, LoginEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void L_SetupReceived(object sender, SetupEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void L_ProductionReceived(object sender, ProductionEventArgs e)
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
            e.ResponseArray[44] = e.P_Val_23;
            e.ResponseArray[45] = e.P_Val_24;
            e.ResponseArray[46] = e.P_Val_25;
            e.ResponseArray[47] = e.P_Val_26;
            e.ResponseArray[48] = e.P_Val_27;
            e.ResponseArray[49] = e.P_Val_28;

            var r = new ListenerResponse();
            r.ProductionResponse(e);
        }
    }
}
