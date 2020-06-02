using System;
using System.Collections.Generic;
using System.Text;

namespace OESListener
{
    public class ListenerResponse
    {
        public string ProductionResponse(ProductionEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    return TcpProductionResponse(e);
                case ListenerType.EIP:
                    return EipProductionResponse(e);
                case ListenerType.PCCC:
                    return PcccProductionResponse(e);
                default:
                    return "";
            }
        }

        public string SetupResponse(SetupEventArgs e)
        {
            
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    return TcpSetupResponse(e);
                case ListenerType.EIP:
                    return EipSetupResponse(e);
                case ListenerType.PCCC:
                    return PcccSetupResponse(e);
                default:
                    return "";
            }
        }

        public string LoginResponse(LoginEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    return TcpLoginResponse(e);
                case ListenerType.EIP:
                    return EipLoginResponse(e);
                case ListenerType.PCCC:
                    return PcccLoginResponse(e);
                default:
                    return "";
            }
        }

        public string SerialRequestResponse(SerialRequestEventArgs e)
        {
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    return TcpSerialRequestRespone(e);
                case ListenerType.EIP:
                    return EipSerialRequestRespone(e);
                case ListenerType.PCCC:
                    return PcccSerialRequestRespone(e);
                default:
                    return "";
            }
        }

        public string FinalPrintResponse(LabelPrintEventArgs e)
        {
            
            //var p = new LabelPrinter(e);
            //p.UseFile = false;
            //var result = p.PrintLabel();
            //e.Response = result.ToString();

            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    return TcpFinalPrintResponse(e);
                case ListenerType.EIP:
                    return EipFinalPrintResponse(e);
                case ListenerType.PCCC:
                    return PcccFinalPrintResponse(e);
                default:
                    return "";
            }
        }

        public string TcpProductionResponse(ProductionEventArgs e)
        {
            string responseString;
            if (e.UseJson)
            {
                e.ProcessIndicator = e.ResponseArray[18];
                e.SuccessIndicator = e.ResponseArray[19];
                e.FaultCode = e.ResponseArray[20];
                e.StatusCode = e.ResponseArray[21];
                var tempList = new List<string>();
                for (var i = 22; i < e.ResponseArray.Length; i++)
                {
                    tempList.Add(e.ResponseArray[i].ToString());
                }
                e.ProcessHistoryValues = tempList.ToArray();

                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellId + ",");
                sb.Append(e.ItemId + ",");
                sb.Append(e.GeneratedBarcode);

                for (var i = 18; i < e.ResponseArray.Length; i++)
                {
                    sb.Append(",");
                    sb.Append(e.ResponseArray[i].ToString());
                }

                responseString = sb.ToString();
            }
            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);

            if (Logger.Enabled)
                Logger.Log(string.Format("Sending: {0}", responseString));

            stream.Write(outData, 0, outData.Length);
            return "GOOD";
        }

        public string TcpSetupResponse(SetupEventArgs e)
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
                sb.Append(e.Response.Component1.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component1.ModelNumber.Trim()); ;
                sb.Append(",");
                sb.Append(e.Response.Component2.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component2.ModelNumber.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component3.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component3.ModelNumber.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component4.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component4.ModelNumber.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component5.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component5.ModelNumber.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component6.AccessId.Trim());
                sb.Append(",");
                sb.Append(e.Response.Component6.ModelNumber.Trim());
                sb.Append(",");
                sb.Append(e.Response.Quantity.Trim());
                sb.Append(",");
                sb.Append(e.Response.Acknowledge.Trim());
                sb.Append(",");
                sb.Append(e.Response.ErrorCode.Trim());
                sb.Append(",");
                sb.Append(string.Join(",", e.Response.PlcModelSetup));

                responseString = sb.ToString();
            }
            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
            return "GOOD";
        }

        public string TcpLoginResponse(LoginEventArgs e)
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

            return "GOOD";
        }

        public string TcpSerialRequestRespone(SerialRequestEventArgs e)
        {
            string responseString;
            e.ItemId = Util.AbIntArrayToString(e.ResponseArray);
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
            return "GOOD";
        }

        public string TcpFinalPrintResponse(LabelPrintEventArgs e)
        {
            string responseString;
            if (e.UseJson)
                responseString = "{\"response\":\"" + e.Response + "\"}";
            else
                responseString = e.Response;

            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);

            return "GOOD";
        }

        public string PcccProductionResponse(ProductionEventArgs e)
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
            
            var s = new PlcWriter();
            
            if (e.UsePlcFive)
            {
                return s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                return s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else 
            {
                return s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            }

            
        }
        
        public string PcccSetupResponse(SetupEventArgs e)
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
                var big = (result - small) * 0.0001;
                retArr[66] = (short)big;
                retArr[65] = (short)small;
            }
            else
            {
                retArr[65] = (short)result;
            }

            Int16.TryParse(e.Response.Acknowledge, out retArr[67]);

            Int16.TryParse(e.Response.ErrorCode, out retArr[68]);
            var s = new PlcWriter();

            var status = "";
            if (e.UsePlcFive)
            {
                status = s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                status = s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else
            {
                status = s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            }

            if (e.ProcessIndicator == 4)
            {
                var pushDownStatus = PcccPlcModelSetupResponse(e.SenderIp, e);
                if (pushDownStatus != "GOOD")
                {
                    status = string.Format("PLC Model Download - {0}", pushDownStatus);
                }
            }

            return status;


        }

        public string PcccPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = e.PlcModelSetup;
            //for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            //{
            //    short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            //}

            var s = new PlcWriter();
            if (e.UsePlcFive)
            {
                return s.PlcResponse(e.SenderIp, retArr, "N241:0");
            }
            else if (e.UsePlcMicrologix)
            {
                return s.MicroLogixResponse(e.SenderIp, retArr, "N241:0");
            }
            else
            {
                return s.SlcResponse(e.SenderIp, retArr, "N241:0");
            }
        }

        public string PcccLoginResponse(LoginEventArgs e)
        {
            var retArr = new short[34];

            if (e.InTagName == "N227:0")
                e.OutTagName = "N228:0";
            else
                e.OutTagName = e.InTagName;

            retArr[20] = e.SuccessIndicator;
            retArr[21] = e.FaultCode;

            var s = new PlcWriter();
            if (e.UsePlcFive)
            {
                return s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                return s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else
            {
                return s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            }

        }

        public string PcccSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247:0";
            Array.Copy(e.ResponseArray, 0, retArr, 0, e.ResponseArray.Length);

            var s = new PlcWriter();
            if (e.UsePlcFive)
            {
                return s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                return s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else
            {
                return s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
        }

        public string PcccFinalPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);

            var s = new PlcWriter();
            if (e.UsePlcFive)
            {
                return s.PlcResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else if (e.UsePlcMicrologix)
            {
                return s.MicroLogixResponse(e.SenderIp, retArr, e.OutTagName);
            }
            else
            {
                return s.SlcResponse(e.SenderIp, retArr, e.OutTagName);
            };
        }

        public string EipProductionResponse(ProductionEventArgs e)
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


            var s = new PlcWriter();
            var status = s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
            if (Logger.Enabled)
                Logger.Log("listener complete");

            return status;
        }

        public string EipSetupResponse(SetupEventArgs e)
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
                var big = (result - small) * 0.0001;
                retArr[66] = (short)big;
                retArr[65] = (short)small;
            }
            else
            {
                retArr[65] = (short)result;
            }

            Int16.TryParse(e.Response.Acknowledge, out retArr[67]);

            Int16.TryParse(e.Response.ErrorCode, out retArr[68]);
            var s = new PlcWriter();
            var status = s.LogixResponse(e.SenderIp, retArr, e.OutTagName);

            if (e.ProcessIndicator == 4)
            {
                var pushDownStatus = EipPlcModelSetupResponse(e.SenderIp, e);
                if (pushDownStatus != "GOOD")
                {
                    status = string.Format("PLC Model Download - {0}", pushDownStatus);
                }
            }

            return status;

        }

        public string EipPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = e.PlcModelSetup;
            //for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            //{
            //    short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            //}

            var s = new PlcWriter();
            return s.LogixResponse(ipAddress, retArr, "N241[0]");
        }

        public string EipLoginResponse(LoginEventArgs e)
        {
            var retArr = new short[34];
            if (e.InTagName == "N227[0]")
                e.OutTagName = "N228[0]";
            else
                e.OutTagName = e.InTagName;

            retArr[20] = e.SuccessIndicator;
            retArr[21] = e.FaultCode;

            var s = new PlcWriter();

            return s.LogixResponse(e.SenderIp, retArr, e.OutTagName);

        }

        public string EipSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247[0]";
            Array.Copy(e.ResponseArray, 0, retArr, 0, e.ResponseArray.Length);
            var s = new PlcWriter();

            return s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
        }

        public string EipFinalPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);
            var s = new PlcWriter();

            return s.LogixResponse(e.SenderIp, retArr, e.OutTagName);
        }
    }
}
