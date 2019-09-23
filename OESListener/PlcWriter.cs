using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OESListener
{
    public class PlcWriter
    {

        public string PlcResponse(string ipAddress, short[] data, string tagName)
        {
            TcpClient c = new TcpClient(ipAddress, 2222);
            var writer = new BinaryWriter(c.GetStream());

            var dataByteLength = (byte)(data.Length * 2);
            var writeLength = (byte)(dataByteLength + 21);
            var byteCountPlusOne = (byte)(dataByteLength + 1);
            var elementCount = (byte)data.Length;

            var status = "";

            byte[] outArr = new byte[] { 1, 7, 0, writeLength, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0xd3, 0, 0, 0, 0, 0, 0x1d, 0x26, 0x26, 0, 0, 0, 0 };

            var tagArr = Util.DesturctureSlcTag(tagName);
            var header = new byte[] { 0, 5, 0, 0, 0x0f, 0, 0x73, 0x46, 0x67, 0, 0, elementCount, 0, 0x07, 0, tagArr[0], tagArr[2], 0x99, 0x09, byteCountPlusOne, 0x42 };
            var dataByteArr = Util.ConvertToByteArray(data);
            var origSize = outArr.Length;
            Array.Resize(ref outArr, header.Length + origSize);
            Array.Copy(header, 0, outArr, origSize, header.Length);
            origSize = outArr.Length;
            Array.Resize(ref outArr, dataByteArr.Length + origSize);
            Array.Copy(dataByteArr, 0, outArr, origSize, dataByteArr.Length);

            Task GetResponse = new Task(() =>
            {

                var reader = new BinaryReader(c.GetStream());

                Byte[] inData = new Byte[1024];
                while (true)
                {
                    var bytes = reader.Read(inData, 0, inData.Length);
                    var byteArr = new byte[bytes];
                    Array.Copy(inData, 0, byteArr, 0, byteArr.Length);

                    if (bytes > 0)
                    {
                        if (byteArr[0] == 0x02 && byteArr[1] == 0x01)
                        {
                            outArr[4] = byteArr[4];
                            outArr[5] = byteArr[5];
                            outArr[6] = byteArr[6];
                            outArr[7] = byteArr[7];
                            writer.Write(outArr);
                        }

                        if (byteArr[0] == 0x02 && byteArr[1] == 0x07)
                        {

                            var errorCode = byteArr[33];
                            var extErrorCode = 0x00;
                            if (errorCode == 0xf0 && byteArr.Length > 36)
                                extErrorCode = byteArr[36];

                            if (errorCode == 0x00)
                            {
                                if (Logger.Enabled)
                                    Logger.Log("plc accecpted response");

                                status = "GOOD";
                            }
                            else
                            {
                                if (Logger.Enabled)
                                    Logger.Log("error writing to plc");

                                if (errorCode == 0xf0)
                                {
                                    if (Util.SlcExtErrorCode.ContainsKey(extErrorCode))
                                        status = string.Format("Error - {0}", Util.SlcExtErrorCode[extErrorCode]);
                                    else
                                        status = "Unknown error writing to plc";
                                }
                                else
                                {
                                    if (Util.SlcErrorCode.ContainsKey(errorCode))
                                        status = Util.SlcErrorCode[errorCode];
                                    else
                                        status = "Unknown error writing to plc";
                                }


                            }

                            break;
                        }
                    }
                    else
                    {
                        break;
                    }


                }

                c.GetStream().Close();
                c.Close();
            });
            GetResponse.Start();

            //open socket connections to target plc on port 2222
            var arr = new byte[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0x28, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            writer.Write(arr);
            GetResponse.Wait();

            return status;

        }
        
        public string SlcResponse(string ipAddress, short[] data, string tagName)
        {
            TcpClient c = new TcpClient(ipAddress, 2222);
            var writer = new BinaryWriter(c.GetStream());

            var dataByteLength = (byte)(data.Length * 2);
            var writeLength = (byte)(dataByteLength + 14);

            var status = "";

            byte[] outArr = new byte[] { 1, 7, 0, writeLength, 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0xd3, 0, 0, 0, 0, 0, 0x1d, 0x26, 0x26, 0, 0, 0, 0 };

            var tagArr = Util.DesturctureSlcTag(tagName);
            var header = new byte[] { 0, 5, 0, 0, 0x0f, 0, 0xe2, 0xc0, 0xaa, dataByteLength, tagArr[0], tagArr[1], tagArr[2], tagArr[3] };
            var dataByteArr = Util.ConvertToByteArray(data);
            var origSize = outArr.Length;
            Array.Resize(ref outArr, header.Length + origSize);
            Array.Copy(header, 0, outArr, origSize, header.Length);
            origSize = outArr.Length;
            Array.Resize(ref outArr, dataByteArr.Length + origSize);
            Array.Copy(dataByteArr, 0, outArr, origSize, dataByteArr.Length);

            Task GetResponse = new Task(() =>
            {
                
                var reader = new BinaryReader(c.GetStream());
                
                Byte[] inData = new Byte[1024];
                while (true)
                {
                    var bytes = reader.Read(inData, 0, inData.Length);
                    var byteArr = new byte[bytes];
                    Array.Copy(inData, 0, byteArr, 0, byteArr.Length);

                    if (bytes > 0)
                    {
                        if (byteArr[0] == 0x02 && byteArr[1] == 0x01)
                        {
                            outArr[4] = byteArr[4];
                            outArr[5] = byteArr[5];
                            outArr[6] = byteArr[6];
                            outArr[7] = byteArr[7];
                            writer.Write(outArr);
                        }

                        if (byteArr[0] == 0x02 && byteArr[1] == 0x07)
                        {
                            var errorCode = byteArr[33];
                            var extErrorCode = 0x00;
                            if (errorCode == 0xf0 && byteArr.Length > 36)
                                extErrorCode = byteArr[36];
                                
                            if (errorCode == 0x00)
                            {
                                if (Logger.Enabled)
                                    Logger.Log("plc accecpted response");

                                status = "GOOD";
                            }
                            else
                            {
                                if (Logger.Enabled)
                                    Logger.Log("error writing to plc");

                                if (errorCode == 0xf0)
                                {
                                    if (Util.SlcExtErrorCode.ContainsKey(extErrorCode))
                                        status = string.Format("Error - {0}", Util.SlcExtErrorCode[extErrorCode]);
                                    else
                                        status = "Unknown error writing to plc";
                                }
                                else
                                {
                                    if (Util.SlcErrorCode.ContainsKey(errorCode))
                                        status = Util.SlcErrorCode[errorCode];
                                    else
                                        status = "Unknown error writing to plc";
                                }
                                 

                            }

                            break;
                        }
                    }
                    else
                    {
                        break;
                    }


                }

                c.GetStream().Close();
                c.Close();
            });
            GetResponse.Start();

            //open socket connections to target plc on port 2222
            var arr = new byte[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0x28, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            writer.Write(arr);
            GetResponse.Wait();

            return status;
        }

        public string MicroLogixResponse(string ipAddress, short[] data, string tagName)
        {
            TcpClient c = new TcpClient(ipAddress, 44818);
            var writer = new BinaryWriter(c.GetStream());
            var senderContext = new byte[8] { 0x24, 0x4f, 0x53, 0x42, 0x4f, 0x52, 0x4e, 0x45 };
            var cId = new byte[4];
            byte[] sessionHandle = new byte[4];
            var status = "";

            Task GetResponse = new Task(() =>
            {

                var reader = new BinaryReader(c.GetStream());
                Byte[] inData = new Byte[1024];
                while (true)
                {
                    int bytes = 0;
                    bool done = false;
                    reader.BaseStream.ReadTimeout = 5000;
                    try
                    {
                        bytes = reader.Read(inData, 0, inData.Length);
                    }
                    catch (Exception ex)
                    {
                        if (Logger.Enabled)
                            Logger.Log(ex.ToString());

                        done = true;
                    }

                    if (done)
                        break;

                    var byteArr = new byte[bytes];
                    Array.Copy(inData, 0, byteArr, 0, byteArr.Length);

                    if (bytes > 0)
                    {

                        if (byteArr[0] == 0x65)
                        {
                            //var sessionHandle = new byte[4];
                            sessionHandle[0] = byteArr[4];
                            sessionHandle[1] = byteArr[5];
                            sessionHandle[2] = byteArr[6];
                            sessionHandle[3] = byteArr[7];

                            byte[] outArr = Util.ForwardOpenPacket(sessionHandle, 0);

                            outArr[12] = senderContext[0];
                            outArr[13] = senderContext[1];
                            outArr[14] = senderContext[2];
                            outArr[15] = senderContext[3];
                            outArr[16] = senderContext[4];
                            outArr[17] = senderContext[5];
                            outArr[18] = senderContext[6];
                            outArr[19] = senderContext[7];

                            writer.Write(outArr);

                        }

                        if (byteArr[0] == 0x6f)
                        {
                            //todo: error handling for errors from plc

                            if (byteArr[40] != 0xd4)
                            {
                                var responsePosition = 0;
                                if (byteArr.Length > 48)
                                {
                                    for (var i = 48; i < byteArr.Length; i++)
                                    {
                                        if (byteArr[i] == 0x4f)
                                        {
                                            responsePosition = i;
                                            break;
                                        }
                                    }
                                }

                                if (byteArr[responsePosition] == 0x4f)
                                {
                                    
                                    if (byteArr.Length > responsePosition + 1)
                                    {
                                        var errorCode = byteArr[responsePosition + 1];
                                        var extErrorCode = (byteArr.Length > responsePosition + 4) ? byteArr[responsePosition + 4] : 0x00;
                                        if (errorCode == 0x00)
                                        {
                                            status = "GOOD";
                                        }
                                        else
                                        {
                                            if (Logger.Enabled)
                                                Logger.Log("error writing to plc");

                                            if (errorCode == 0xf0)
                                            {
                                                if (Util.SlcExtErrorCode.ContainsKey(extErrorCode))
                                                    status = string.Format("Error - {0}", Util.SlcExtErrorCode[extErrorCode]);
                                                else
                                                    status = "Unknown error writing to plc";
                                            }
                                            else
                                            {
                                                if (Util.SlcErrorCode.ContainsKey(errorCode))
                                                    status = Util.SlcErrorCode[errorCode];
                                                else
                                                    status = "Unknown error writing to plc";
                                            }
                                        }
                                    }
                                }

                                break;
                            }

                            var dataByteLength = (byte)(data.Length * 2);
                            var writeLength = Util.ConvertIntToTwoBytes(dataByteLength + 10);
                            //var allLength = dataByteLength + 38;

                            var tagArr = Util.DesturctureSlcTag(tagName);

                            //if (ipAddress.Length % 2 != 0)
                            //    ipAddress += char.MinValue;

                            ipAddress += char.MinValue;

                            var allLength = dataByteLength + 26 + ipAddress.Length;

                            var ipArr = Encoding.Default.GetBytes(ipAddress);
                            var ipLen = Util.ConvertIntToTwoBytes(ipArr.Length);

                            var header = new byte[] { 0x91, 0x00, writeLength[0], writeLength[1], 0x0f, 0, 0xe2, 0xc0, 0xaa, dataByteLength, tagArr[0], tagArr[1], tagArr[2], tagArr[3] };
                            var dataByteArr = Util.ConvertToByteArray(data);

                            sessionHandle[0] = byteArr[4];
                            sessionHandle[1] = byteArr[5];
                            sessionHandle[2] = byteArr[6];
                            sessionHandle[3] = byteArr[7];

                            var eipHeader = Util.EipPcccWrapper(sessionHandle, allLength);

                            var typeId1 = new byte[] { 0x85, 0x00 };

                            var retList = new List<byte>();
                            retList.AddRange(eipHeader);
                            retList.AddRange(typeId1);
                            retList.AddRange(ipLen);
                            retList.AddRange(ipArr);
                            retList.AddRange(header);
                            retList.AddRange(dataByteArr);

                            

                            byte[] outArr = retList.ToArray();

                            writer.Write(outArr);


                            if (Logger.Enabled)
                                Logger.Log("plc accecpted response");
                        }
                        if (byteArr[0] == 0x70)
                        {

                            break;
                        }


                    }
                    else
                    {
                        break;
                    }




                }
                if (c.Connected)
                    c.GetStream().Close();

                c.Close();

            });
            GetResponse.Start();


            var registerSession = new byte[] { 0x65, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            writer.Write(registerSession);
            GetResponse.Wait();

            if (Logger.Enabled)
                Logger.Log("Task Complete");

            return status;
        }

        public string LogixResponse(string ipAddress, short[] data, string tagName)
        {
            TcpClient c = new TcpClient(ipAddress, 44818);

            var writer = new BinaryWriter(c.GetStream());
            var senderContext = new byte[8] { 0x24, 0x4f, 0x53, 0x42, 0x4f, 0x52, 0x4e, 0x45 };
            var cId = new byte[4];
            byte[] sessionHandle = new byte[4];

            var status = "";

            Task GetResponse = new Task(() =>
            {

            var reader = new BinaryReader(c.GetStream());
            Byte[] inData = new Byte[1024];
            while (true)
            {
                int bytes = 0;
                    bool done = false;
                reader.BaseStream.ReadTimeout = 5000;
                try
                {
                    bytes = reader.Read(inData, 0, inData.Length);
                }
                catch (Exception ex)
                {
                    if (Logger.Enabled)
                        Logger.Log(ex.ToString());

                    done = true;
                }

                if (done)
                    break;

                var byteArr = new byte[bytes];
                Array.Copy(inData, 0, byteArr, 0, byteArr.Length);

                if (bytes > 0)
                {
                    //Util.DisplayHexValues(byteArr);

                    if (byteArr[0] == 0x65)
                    {
                        //var sessionHandle = new byte[4];
                        sessionHandle[0] = byteArr[4];
                        sessionHandle[1] = byteArr[5];
                        sessionHandle[2] = byteArr[6];
                        sessionHandle[3] = byteArr[7];

                        byte[] outArr = Util.ForwardOpenPacket(sessionHandle, 0);

                        outArr[12] = senderContext[0];
                        outArr[13] = senderContext[1];
                        outArr[14] = senderContext[2];
                        outArr[15] = senderContext[3];
                        outArr[16] = senderContext[4];
                        outArr[17] = senderContext[5];
                        outArr[18] = senderContext[6];
                        outArr[19] = senderContext[7];

                        writer.Write(outArr);

                    }

                    if (byteArr[0] == 0x6f)
                    {
                        //forward open received
                        if (byteArr[40] == 0xd4)
                        {
                            //forward open successful
                            if (byteArr[42] == 0x00)
                            {
                                var connectionId = new byte[4];
                                connectionId[0] = byteArr[44];
                                connectionId[1] = byteArr[45];
                                connectionId[2] = byteArr[46];
                                connectionId[3] = byteArr[47];
                                cId = connectionId;
                                byte[] outArr = Util.Build_EIP_CIP_Header(Util.createWriteRequest(tagName, data), sessionHandle, connectionId);

                                outArr[12] = senderContext[0];
                                outArr[13] = senderContext[1];
                                outArr[14] = senderContext[2];
                                outArr[15] = senderContext[3];
                                outArr[16] = senderContext[4];
                                outArr[17] = senderContext[5];
                                outArr[18] = senderContext[6];
                                outArr[19] = senderContext[7];

                                writer.Write(outArr);
                            }
                            //forward open connection error, connection already established
                            if (byteArr[42] == 0x01)
                            {
                                sessionHandle[0] = byteArr[4];
                                sessionHandle[1] = byteArr[5];
                                sessionHandle[2] = byteArr[6];
                                sessionHandle[3] = byteArr[7];

                                var connectSerial = Util.Convert2BytesToInteger(byteArr[46], byteArr[47]);

                                byte[] outArr = Util.ForwardClosePacket(sessionHandle, 0, connectSerial);

                                outArr[12] = senderContext[0];
                                outArr[13] = senderContext[1];
                                outArr[14] = senderContext[2];
                                outArr[15] = senderContext[3];
                                outArr[16] = senderContext[4];
                                outArr[17] = senderContext[5];
                                outArr[18] = senderContext[6];
                                outArr[19] = senderContext[7];

                                writer.Write(outArr);
                            }

                        }

                        //forward close received
                        if (byteArr[40] == 0xce)
                        {
                            //forward close successful
                            if (byteArr[42] == 0x00)
                            {
                                break;
                            }

                        }

                        if (Logger.Enabled)
                            Logger.Log("plc accecpted response");
                    }
                        if (byteArr[0] == 0x70)
                        {
                            //Util.DisplayHexValues(byteArr);

                            //write received
                            if (byteArr[46] == 0xcd)
                            {
                                var errorCode = byteArr[48];
                                var extErrorCode = byteArr[49];

                                //write successful
                                if (errorCode == 0x00)
                                {
                                    status = "GOOD";
                                }
                                else
                                {
                                    if (extErrorCode != 0)
                                        errorCode = extErrorCode;

                                    if (Util.LogixErrorCode.ContainsKey(errorCode))
                                        status = string.Format("Error - {0}", Util.LogixErrorCode[errorCode]);
                                    else
                                        status = string.Format("Error {0} - Unknown error writing to plc", errorCode);
                                }


                                sessionHandle[0] = byteArr[4];
                                sessionHandle[1] = byteArr[5];
                                sessionHandle[2] = byteArr[6];
                                sessionHandle[3] = byteArr[7];

                                var connectSeral = Util.Convert2BytesToInteger(byteArr[46], byteArr[47]);

                                byte[] outArr = Util.ForwardClosePacket(sessionHandle, 0, connectSeral);

                                outArr[12] = senderContext[0];
                                outArr[13] = senderContext[1];
                                outArr[14] = senderContext[2];
                                outArr[15] = senderContext[3];
                                outArr[16] = senderContext[4];
                                outArr[17] = senderContext[5];
                                outArr[18] = senderContext[6];
                                outArr[19] = senderContext[7];

                                writer.Write(outArr);

                            }
                        }

                    }
                    else
                    {
                        break;
                    }




                }
                if (c.Connected)
                    c.GetStream().Close();

                c.Close();

            });
            GetResponse.Start();
            
            
            var registerSession = new byte[] { 0x65, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            writer.Write(registerSession);
            GetResponse.Wait();

            

            if (Logger.Enabled)
                Logger.Log("Task Complete");

            return status;
        }

        public string LogixResponse2(string ipAddress, short[] data, string tagName)
        {
            TcpClient c = new TcpClient(ipAddress, 44818);
            var writer = new BinaryWriter(c.GetStream());
            var senderContext = new byte[8] { 0x24, 0x49, 0x4e, 0x47, 0x45, 0x41, 0x52, 0x24 };

            byte[] sessionHandle = new byte[4];

            var status = "";

            Task GetResponse = new Task(() =>
            {

                var reader = new BinaryReader(c.GetStream());
                Byte[] inData = new Byte[1024];
                while (true)
                {
                    int bytes = 0;
                    bool done = false;
                    reader.BaseStream.ReadTimeout = 5000;
                    try
                    {
                        bytes = reader.Read(inData, 0, inData.Length);
                    }
                    catch (Exception ex)
                    {
                        if (Logger.Enabled)
                            Logger.Log(ex.ToString());

                        done = true;
                    }

                    if (done)
                        break;

                    var byteArr = new byte[bytes];
                    Array.Copy(inData, 0, byteArr, 0, byteArr.Length);

                    if (bytes > 0)
                    {
                        if (byteArr[0] == 0x65)
                        {
                            sessionHandle[0] = byteArr[4];
                            sessionHandle[1] = byteArr[5];
                            sessionHandle[2] = byteArr[6];
                            sessionHandle[3] = byteArr[7];

                            byte[] outArr = Util.ForwardOpenPacket(sessionHandle, 0);

                            outArr[12] = senderContext[0];
                            outArr[13] = senderContext[0];
                            outArr[14] = senderContext[0];
                            outArr[15] = senderContext[0];
                            outArr[16] = senderContext[0];
                            outArr[17] = senderContext[0];
                            outArr[18] = senderContext[0];
                            outArr[19] = senderContext[0];

                            writer.Write(outArr);
                            
                        }

                        if (byteArr[0] == 0x6f)
                        {
                            //todo: error handling for errors from plc

                            var wRequest = Util.createWriteRequest(tagName, data);
                            var connectionId = new byte[4];
                            connectionId[0] = byteArr[44];
                            connectionId[1] = byteArr[45];
                            connectionId[2] = byteArr[46];
                            connectionId[3] = byteArr[47];

                            byte[] outArr = Util.Build_EIP_CIP_Header(Util.createWriteRequest(tagName, data), sessionHandle, connectionId);
                            outArr[12] = senderContext[0];
                            outArr[13] = senderContext[0];
                            outArr[14] = senderContext[0];
                            outArr[15] = senderContext[0];
                            outArr[16] = senderContext[0];
                            outArr[17] = senderContext[0];
                            outArr[18] = senderContext[0];
                            outArr[19] = senderContext[0];

                            writer.Write(outArr);
                            
                            if (Logger.Enabled)
                                Logger.Log("plc accecpted response");

                            //break;
                        }
                        if (byteArr[0] == 0x70)
                        {
                            var arr = new byte[] {0x66, 0x00, 0x00, 0x00, sessionHandle[0], sessionHandle[1], sessionHandle[2], sessionHandle[3],
                            0x00, 0x00, 0x00, 0x00, byteArr[12], byteArr[13], byteArr[14], byteArr[15], byteArr[16],
                            byteArr[17], byteArr[18], byteArr[19], 0x00, 0x00, 0x00, 0x00,};

                            arr[12] = senderContext[0];
                            arr[13] = senderContext[0];
                            arr[14] = senderContext[0];
                            arr[15] = senderContext[0];
                            arr[16] = senderContext[0];
                            arr[17] = senderContext[0];
                            arr[18] = senderContext[0];
                            arr[19] = senderContext[0];

                            writer.Write(arr);
                            
                            break;
                        }


                    }
                    else
                    {
                        break;
                    }


                }

                if (c != null)
                    c.GetStream().Flush();

                c.GetStream().Close();
                c.Close();
            });
            GetResponse.Start();


            var registerSession = new byte[] { 0x65, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            writer.Write(registerSession);
            GetResponse.Wait();
            if (Logger.Enabled)
                Logger.Log("Task Complete");

            return status;
        }



    }
}
