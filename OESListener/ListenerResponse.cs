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

        public void LabelPrintResponse(LabelPrintEventArgs e)
        {
            var p = new LabelPrinter(e);
            p.UseFile = true;
            var result = p.PrintLabel();
            e.Response = result.ToString();
            
            switch (e.listenerType)
            {
                case ListenerType.TCP:
                    TcpPrintResponse(e);
                    break;
                case ListenerType.EIP:
                    EipPrintResponse(e);
                    break;
                case ListenerType.PCCC:
                    PcccPrintResponse(e);
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
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append(e.CellId);
                sb.Append(",");
                sb.Append(e.ItemId);
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

        public void TcpSetupResponse(SetupEventArgs e)
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
                sb.Append(e.Request);
                sb.Append(",");
                sb.Append(e.Status);
                sb.Append(",");
                sb.Append(e.FailureCode);
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
                sb.Append(e.ItemID);
                responseString = sb.ToString();
            }

            var stream = e.Client.GetStream();
            var outData = Encoding.ASCII.GetBytes(responseString);
            stream.Write(outData, 0, outData.Length);
        }

        public void TcpPrintResponse(LabelPrintEventArgs e)
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

            retArr[18] = Convert.ToInt16(e.Request);
            retArr[19] = Convert.ToInt16(e.Status);
            retArr[20] = 0;
            retArr[21] = Convert.ToInt16(e.FailureCode);

            var phArr = new List<short>();
            foreach (var item in e.ProcessHistoryValues)
            {
                var fp = Convert.ToDouble(item);
                var wh = (short)Math.Truncate(fp);
                var d = (fp - wh) * 100;
                var dec = (short)(Math.Round(d));
                phArr.Add(wh);
                phArr.Add(dec);
            }

            Array.Copy(phArr.ToArray(), 0, retArr, 22, phArr.Count());
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();
            s.SlcResponse(targetIp, retArr, e.OutTagName);
        }
        
        public void PcccSetupResponse(SetupEventArgs e)
        {
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
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();
            s.SlcResponse(targetIp, retArr, e.OutTagName);

            if (e.Request == "4")
                PcccPlcModelSetupResponse(targetIp, e);

        }

        public void PcccPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = new short[34];
            for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            {
                short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            }

            var s = new PlcWriter();
            s.SlcResponse(ipAddress, retArr, "N241:0");
        }

        public void PcccLoginResponse(LoginEventArgs e)
        {
            var retArr = new short[34];
            e.OutTagName = e.InTagName;

            short.TryParse(e.Status, out retArr[20]);
            short.TryParse(e.FailureCode, out retArr[21]);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.SlcResponse(targetIp, retArr, e.OutTagName);

        }

        public void PcccSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247:0";
            var itemArr = Util.StringToAbIntArray(e.ItemID);
            Array.Copy(itemArr, 0, retArr, 0, itemArr.Length);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.SlcResponse(targetIp, retArr, e.OutTagName);
        }

        public void PcccPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.SlcResponse(targetIp, retArr, e.OutTagName);
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

            retArr[18] = Convert.ToInt16(e.Request);
            retArr[19] = Convert.ToInt16(e.Status);
            retArr[20] = 0;
            retArr[21] = Convert.ToInt16(e.FailureCode);

            var phArr = new List<short>();
            foreach (var item in e.ProcessHistoryValues)
            {
                var fp = Convert.ToDouble(item);
                var wh = (short)Math.Truncate(fp);
                var d = (fp - wh) * 100;
                var dec = (short)(Math.Round(d));
                phArr.Add(wh);
                phArr.Add(dec);
            }

            Array.Copy(phArr.ToArray(), 0, retArr, 22, phArr.Count());
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();
            s.LogixResponse(targetIp, retArr, e.OutTagName);
        }

        public void EipSetupResponse(SetupEventArgs e)
        {
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
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();
            s.LogixResponse(targetIp, retArr, e.OutTagName);

            if (e.Request == "4")
                EipPlcModelSetupResponse(targetIp, e);

        }

        public void EipPlcModelSetupResponse(string ipAddress, SetupEventArgs e)
        {
            var retArr = new short[34];
            for (var i = 0; i < e.Response.PlcModelSetup.Length; i++)
            {
                short.TryParse(e.Response.PlcModelSetup[i], out retArr[i]);
            }

            var s = new PlcWriter();
            s.LogixResponse(ipAddress, retArr, "N241[0]");
        }

        public void EipLoginResponse(LoginEventArgs e)
        {
            e.OutTagName = "N228[0]";

            var retArr = new short[34];
            e.OutTagName = e.InTagName;

            short.TryParse(e.Status, out retArr[20]);
            short.TryParse(e.FailureCode, out retArr[21]);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.LogixResponse(targetIp, retArr, e.OutTagName);

        }

        public void EipSerialRequestRespone(SerialRequestEventArgs e)
        {
            var retArr = new short[20];
            e.OutTagName = "N247[0]";
            var itemArr = Util.StringToAbIntArray(e.ItemID);
            Array.Copy(itemArr, 0, retArr, 0, itemArr.Length);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.LogixResponse(targetIp, retArr, e.OutTagName);
        }

        public void EipPrintResponse(LabelPrintEventArgs e)
        {
            var retArr = new short[10];
            e.OutTagName = e.InTagName;
            short.TryParse(e.Response, out retArr[0]);
            var targetIp = ((IPEndPoint)e.Client.Client.RemoteEndPoint).Address.ToString();
            var s = new PlcWriter();

            s.LogixResponse(targetIp, retArr, e.OutTagName);
        }
    }
}
