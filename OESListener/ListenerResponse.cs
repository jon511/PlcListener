using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace OESListener
{
    public class ListenerResponse
    {
        public void ProductionResponse(ProductionEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpProductionResponse(e);
                    break;
                case ListenerType.EIP:
                    EipProductionResponse(e);
                    break;
                case ListenerType.PCCC:
                    PcccProductionResponse(e);
                    break;
                default:
                    break;
            }
        }

        public void SetupResponse(SetupEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpSetupResponse(e);
                    break;
                case ListenerType.EIP:
                    EipSetupResponse(e);
                    break;
                case ListenerType.PCCC:
                    PcccSetupResponse(e);
                    break;
                default:
                    break;
            }
        }

        public void LoginResponse(LoginEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpLoginResponse(e);
                    break;
                case ListenerType.EIP:
                    EipLoginResponse(e);
                    break;
                case ListenerType.PCCC:
                    PcccLoginResponse(e);
                    break;
                default:
                    break;
            }
        }

        public void SerialRequestResponse(SerialRequestEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpSerialRequestRespone(e);
                    break;
                case ListenerType.EIP:
                    EipSerialRequestRespone(e);
                    break;
                case ListenerType.PCCC:
                    PcccSerialRequestRespone(e);
                    break;
                default:
                    break;
            }
        }

        public void FinalPrintResponse(LabelPrintEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpFinalPrintResponse(e);
                    break;
                case ListenerType.EIP:
                    EipFinalPrintResponse(e);
                    break;
                case ListenerType.PCCC:
                    PcccFinalPrintResponse(e);
                    break;
                default:
                    break;
            }
        }

        public void TcpProductionResponse(ProductionEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                e.ProcessIndicator = e.ResponseArray[18];
                e.SuccessIndicator = e.ResponseArray[19];
                e.FaultCode = e.ResponseArray[20];
                e.StatusCode = e.ResponseArray[21];

                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellId);
                sb.Append(",");
                sb.Append(e.ItemId);
                sb.Append(",");
                sb.Append(e.GeneratedBarcode);
                sb.Append(",");
                //sb.Append(e.ProcessIndicator);
                sb.Append(e.ResponseArray[18].ToString());
                sb.Append(",");
                //sb.Append(e.SuccessIndicator);
                sb.Append(e.ResponseArray[19].ToString());
                sb.Append(",");
                //sb.Append(e.FaultCode);
                sb.Append(e.ResponseArray[20].ToString());
                sb.Append(",");
                //sb.Append(e.StatusCode);
                sb.Append(e.ResponseArray[21].ToString());
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[22] + (e.ResponseArray[23] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[24] + (e.ResponseArray[25] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[26] + (e.ResponseArray[27] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[28] + (e.ResponseArray[29] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[30] + (e.ResponseArray[31] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[32] + (e.ResponseArray[33] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[34] + (e.ResponseArray[35] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[36] + (e.ResponseArray[37] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[38] + (e.ResponseArray[39] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[40] + (e.ResponseArray[41] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[42] + (e.ResponseArray[43] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[44] + (e.ResponseArray[45] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[46] + (e.ResponseArray[47] * .01))));
                sb.Append(",");
                sb.Append(string.Format("{0}", (e.ResponseArray[48] + (e.ResponseArray[49] * .01))));
                responseString = sb.ToString();
            }
            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public void TcpSetupResponse(SetupEventArgs e)
        {
            e.ParseReturnData();

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

        public void TcpLoginResponse(LoginEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellId);
                sb.Append(",");
                sb.Append(e.ProcessIndicator);
                sb.Append(",");
                sb.Append(e.SuccessIndicator);
                sb.Append(",");
                sb.Append(e.FaultCode);
                responseString = sb.ToString();
            }
            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public void TcpSerialRequestRespone(SerialRequestEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellId);
                sb.Append(",");
                sb.Append(e.ItemId);
                responseString = sb.ToString();
            }

            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public void TcpFinalPrintResponse(LabelPrintEventArgs e)
        {
            string responseString;
            if (e.UseJson)
                responseString = "{\"response\":\"" + e.Response + "\"}";
            else
                responseString = e.Response;

            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);

        }

        public void PcccProductionResponse(ProductionEventArgs e)
        {
            var retArr = new short[50];

            if (e.InTagName == "N197:0")
                e.OutTagName = "N198:0";
            else if (e.InTagName == "N207:0")
                e.OutTagName = "N208:0";
            else if (e.InTagName == "N217:0")
                e.OutTagName = "N218:0";
            else
                e.OutTagName = e.InTagName;

            var cellArr = Util.StringToAbIntArray(e.CellId);
            Array.Copy(cellArr, 0, retArr, 0, cellArr.Length);

            var itemArr = Util.StringToAbIntArray(e.ItemId);
            Array.Copy(itemArr, 0, retArr, 5, itemArr.Length);

            Array.Copy(e.ResponseArray, 18, retArr, 18, 32);
            
            //retArr[18] = Convert.ToInt16(e.ProcessIndicator);
            //retArr[19] = Convert.ToInt16(e.SuccessIndicator);
            //retArr[20] = 0;
            //retArr[21] = Convert.ToInt16(e.FaultCode);
            //retArr[22] = e.P_Val_1;
            //retArr[23] = e.P_Val_2;
            //retArr[24] = e.P_Val_3;
            //retArr[25] = e.P_Val_4;
            //retArr[26] = e.P_Val_5;
            //retArr[27] = e.P_Val_6;
            //retArr[28] = e.P_Val_7;
            //retArr[29] = e.P_Val_8;
            //retArr[30] = e.P_Val_9;
            //retArr[31] = e.P_Val_10;
            //retArr[32] = e.P_Val_11;
            //retArr[33] = e.P_Val_12;
            //retArr[34] = e.P_Val_13;
            //retArr[35] = e.P_Val_14;
            //retArr[36] = e.P_Val_15;
            //retArr[37] = e.P_Val_16;
            //retArr[38] = e.P_Val_17;
            //retArr[39] = e.P_Val_18;
            //retArr[40] = e.P_Val_19;
            //retArr[41] = e.P_Val_20;
            //retArr[42] = e.P_Val_21;
            //retArr[43] = e.P_Val_22;
            //retArr[44] = e.P_Val_23;
            //retArr[45] = e.P_Val_24;
            //retArr[46] = e.P_Val_25;
            //retArr[47] = e.P_Val_26;
            //retArr[48] = e.P_Val_27;
            //retArr[49] = e.P_Val_28;

            var s = new PlcWriter();
            if (e.UsePlcFive)
            {
                s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else 
            {
                s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            
        }
        
        public void PcccSetupResponse(SetupEventArgs e)
        {
            e.ParseReturnData();

            e.OutTagName = "N238:0";
            var retArr = new short[71];
            
            if (!string.IsNullOrEmpty(e.Response.Component1.AccessId))
            {
                Int16.TryParse(e.Response.Component1.AccessId, out retArr[0]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component1.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 1, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component2.AccessId))
            {
                Int16.TryParse(e.Response.Component2.AccessId, out retArr[11]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component2.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 12, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component3.AccessId))
            {
                Int16.TryParse(e.Response.Component3.AccessId, out retArr[22]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component3.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 23, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component4.AccessId))
            {
                Int16.TryParse(e.Response.Component4.AccessId, out retArr[33]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component4.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 34, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component5.AccessId))
            {
                Int16.TryParse(e.Response.Component5.AccessId, out retArr[44]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component5.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 45, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component6.AccessId))
            {
                Int16.TryParse(e.Response.Component6.AccessId, out retArr[55]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component6.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 56, componentModelArr.Length);
            }
            // add quantity
            if (!Int32.TryParse(e.Response.Quantity, out int result))
            {
                result = 0;
            }
            if (result > 9999)
            {
                var small = result - 10000;
                var big = result - small;
                retArr[65] = (byte)big;
                retArr[66] = (byte)small;
            }
            else
            {
                retArr[66] = (byte)result;
            }

            Int16.TryParse(e.Response.Acknowledge, out retArr[67]);

            Int16.TryParse(e.Response.ErrorCode, out retArr[68]);
            var s = new PlcWriter();
            s.SlcResponse(e.SenderIp, retArr, e.OutTagName);

            if (e.ProcessIndicator == 4)
                PcccPlcModelSetupResponse(e.SenderIp, e);

        }

        public void PcccPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = e.PlcModelSetup;
            //for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            //{
            //    short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            //}

            var s = new PlcWriter();
            s.SlcResponse(ipAddress, retArr, "N241:0");
        }

        public void PcccLoginResponse(LoginEventArgs e)
        {
            var retArr = new short[34];
            e.OutTagName = e.InTagName;
            retArr[20] = e.SuccessIndicator;
            retArr[21] = e.FaultCode;

            var s = new PlcWriter();

            s.SlcResponse(e.SenderIp, retArr, e.OutTagName);

        }

        public void PcccSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247:0";
            var itemArr = Util.StringToAbIntArray(e.ItemId);
            Array.Copy(itemArr, 0, retArr, 0, itemArr.Length);
            var s = new PlcWriter();

            s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
        }

        public void PcccFinalPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);
            var s = new PlcWriter();

            s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
        }

        public void EipProductionResponse(ProductionEventArgs e)
        {
            var retArr = new short[50];

            if (e.InTagName == "N197[0]")
                e.OutTagName = "N198[0]";
            else if (e.InTagName == "N207[0]")
                e.OutTagName = "N208[0]";
            else if (e.InTagName == "N217[0]")
                e.OutTagName = "N218[0]";
            else
                e.OutTagName = e.InTagName;

            var cellArr = Util.StringToAbIntArray(e.CellId);
            Array.Copy(cellArr, 0, retArr, 0, cellArr.Length);

            var itemArr = Util.StringToAbIntArray(e.ItemId);
            Array.Copy(itemArr, 0, retArr, 5, itemArr.Length);

            Array.Copy(e.ResponseArray, 18, retArr, 18, 32);

            //retArr[18] = Convert.ToInt16(e.ProcessIndicator);
            //retArr[19] = Convert.ToInt16(e.SuccessIndicator);
            //retArr[20] = 0;
            //retArr[21] = Convert.ToInt16(e.FaultCode);
            //retArr[22] = e.P_Val_1;
            //retArr[23] = e.P_Val_2;
            //retArr[24] = e.P_Val_3;
            //retArr[25] = e.P_Val_4;
            //retArr[26] = e.P_Val_5;
            //retArr[27] = e.P_Val_6;
            //retArr[28] = e.P_Val_7;
            //retArr[29] = e.P_Val_8;
            //retArr[30] = e.P_Val_9;
            //retArr[31] = e.P_Val_10;
            //retArr[32] = e.P_Val_11;
            //retArr[33] = e.P_Val_12;
            //retArr[34] = e.P_Val_13;
            //retArr[35] = e.P_Val_14;
            //retArr[36] = e.P_Val_15;
            //retArr[37] = e.P_Val_16;
            //retArr[38] = e.P_Val_17;
            //retArr[39] = e.P_Val_18;
            //retArr[40] = e.P_Val_19;
            //retArr[41] = e.P_Val_20;
            //retArr[42] = e.P_Val_21;
            //retArr[43] = e.P_Val_22;
            //retArr[44] = e.P_Val_23;
            //retArr[45] = e.P_Val_24;
            //retArr[46] = e.P_Val_25;
            //retArr[47] = e.P_Val_26;
            //retArr[48] = e.P_Val_27;
            //retArr[49] = e.P_Val_28;

            var s = new PlcWriter();
            s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
            if (Logger.Enabled)
                Logger.Log("listener complete");
        }

        public void EipSetupResponse(SetupEventArgs e)
        {
            e.ParseReturnData();

            e.OutTagName = "N238[0]";
            var retArr = new short[71];

            if (!string.IsNullOrEmpty(e.Response.Component1.AccessId))
            {
                Int16.TryParse(e.Response.Component1.AccessId, out retArr[0]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component1.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 1, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component2.AccessId))
            {
                Int16.TryParse(e.Response.Component2.AccessId, out retArr[11]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component2.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 12, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component3.AccessId))
            {
                Int16.TryParse(e.Response.Component3.AccessId, out retArr[22]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component3.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 23, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component4.AccessId))
            {
                Int16.TryParse(e.Response.Component4.AccessId, out retArr[33]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component4.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 34, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component5.AccessId))
            {
                Int16.TryParse(e.Response.Component5.AccessId, out retArr[44]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component5.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 45, componentModelArr.Length);
            }
            if (!string.IsNullOrEmpty(e.Response.Component6.AccessId))
            {
                Int16.TryParse(e.Response.Component6.AccessId, out retArr[55]);
                var componentModelArr = Util.StringToAbIntArray(e.Response.Component6.ModelNumber);
                Array.Copy(componentModelArr, 0, retArr, 56, componentModelArr.Length);
            }
            // add quantity
            if (!Int32.TryParse(e.Response.Quantity, out int result))
            {
                result = 0;
            }
            if (result > 9999)
            {
                var small = result - 10000;
                var big = result - small;
                retArr[65] = (byte)big;
                retArr[66] = (byte)small;
            }
            else
            {
                retArr[66] = (byte)result;
            }

            Int16.TryParse(e.Response.Acknowledge, out retArr[67]);

            Int16.TryParse(e.Response.ErrorCode, out retArr[68]);
            var s = new PlcWriter();
            s.LogixResponse(e.SenderIp, retArr, e.OutTagName);

            if (e.ProcessIndicator == 4)
                EipPlcModelSetupResponse(e.SenderIp, e);

        }

        public void EipPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = e.PlcModelSetup;
            //for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            //{
            //    short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            //}

            var s = new PlcWriter();
            s.LogixResponse(ipAddress, retArr, "N241[0]");
        }

        public void EipLoginResponse(LoginEventArgs e)
        {
            var retArr = new short[34];
            e.OutTagName = e.InTagName;
            retArr[20] = e.SuccessIndicator;
            retArr[21] = e.FaultCode;

            var s = new PlcWriter();

            s.LogixResponse(e.SenderIp, retArr, e.OutTagName);

        }

        public void EipSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247[0]";
            var itemArr = Util.StringToAbIntArray(e.ItemId);
            Array.Copy(itemArr, 0, retArr, 0, itemArr.Length);
            var s = new PlcWriter();

            s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
        }

        public void EipFinalPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);
            var s = new PlcWriter();

            s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
        }
    }
}
