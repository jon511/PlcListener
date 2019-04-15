using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OESListener
{
    public class EipListener : AbPlcAsyncListener
    {
        protected static readonly ManualResetEvent allDone = new ManualResetEvent(false);

        public EipListener(string ipAddress) : base()
        {
            myIPAddress = ipAddress;
            Port = 44818;
        }

        public void Listen()
        {
            Thread myNewThread = new Thread(() => StartListening());
            myNewThread.Start();
        }

        protected void StartListening()
        {
            var ipAddressForFamily = System.Net.IPAddress.Parse(myIPAddress);
            var ipEndPoint = new IPEndPoint(ipAddressForFamily, Port);
            System.Net.Sockets.Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Bind(ipEndPoint);
                server.Listen(100);

                while (true)
                {
                    allDone.Reset();

                    if (Logger.Enabled)
                        Logger.Log("Ready for connection...");

                    server.BeginAccept(new AsyncCallback(AcceptReceiveDataCallback), server);
                    if (Logger.Enabled)
                        Logger.Log("Connected!");

                    allDone.WaitOne();

                }
            }
            catch (SocketException e)
            {
                if (Logger.Enabled)
                    Logger.Log(string.Format("SocketException: {0}", e));
            }
            finally
            {
                //Stop listening for new clients
                server.Dispose();
            }
        }

        protected void AcceptReceiveDataCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();

            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }
        

        protected static void Send(Socket handler, byte[] data)
        {
            handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallBack), handler);
        }

        protected static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception ex)
            {
                if (Logger.Enabled)
                    Logger.Log(ex.ToString());
            }
        }


        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            var remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            var senderIp = remoteIpEndPoint.Address.ToString();
            var plcID = new byte[4];
            var bytes = new byte[1024];

            try
            {

                int bytesRead = handler.EndReceive(ar);


                if (bytesRead > 0)
                {
                    var receivedBytes = new byte[bytesRead];

                    Array.Copy(state.buffer, 0, receivedBytes, 0, receivedBytes.Length);
                    
                    if (receivedBytes[0] == 0x04)
                    {
                        ListServices(handler, receivedBytes);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                    }
                    else if (receivedBytes[0] == 0x65)
                    {
                        RegisterSesision(handler, receivedBytes);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                    }
                    else if (receivedBytes[0] == 0x6f)
                    {
                        // value 0x54 in element 40 indicates Foward Open. Data will be sent seperately after connection is
                        // established.
                        // value 0x52 indicates unconnected message. Data is appending to this transaction.
                        if (receivedBytes[40] == 0x54)
                        {
                            //test adding plcId to state object
                            Array.Copy(receivedBytes, 52, state.plcId, 0, 4);
                            Array.Copy(receivedBytes, 48, state.conId, 0, 4);
                            state.orgSn[0] = receivedBytes[56];
                            state.orgSn[1] = receivedBytes[57];


                            FowardOpen(handler, receivedBytes, state, senderIp);
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                        }
                        else if (receivedBytes[40] == 0x52)
                        {
                            UnconnectedMessage(handler, receivedBytes, senderIp);
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                        }
                        else
                        {
                            return;
                        }
                    }

                    else if (receivedBytes[0] == 0x70)
                    {
                        SendUnitData(handler, receivedBytes, state);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                    }
                    else
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }

                }
                else
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception ex)
            {
                if (Logger.Enabled)
                    Logger.Log(ex.ToString());

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            
        }

        private void ListServices(Socket handler, byte[] bytes)
        {
            bytes[2] = 0x1a;
            byte[] resArr = { 0x01, 0x00, 0x00, 0x01, 0x14, 0x00, 0x01, 0x00, 0x20, 0x00 };
            byte[] nameOfService = { 0x43, 0x6f, 0x6d, 0x6d, 0x75, 0x6e, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x73, 0x00, 0x00 };

            var retArr = new byte[bytes.Length + resArr.Length + nameOfService.Length];

            Buffer.BlockCopy(bytes, 0, retArr, 0, bytes.Length);
            Buffer.BlockCopy(resArr, 0, retArr, bytes.Length, resArr.Length);
            Buffer.BlockCopy(nameOfService, 0, retArr, bytes.Length + resArr.Length, nameOfService.Length);

            Send(handler, retArr);
        }
        private void RegisterSesision(Socket handler, byte[] bytes)
        {
            var rnd = new Random();
            var sessionInt = rnd.Next(1, 63000);

            var sessionHandle = Util.ConvertIntToFourBytes(sessionInt);
            Buffer.BlockCopy(sessionHandle, 0, bytes, 4, 4);

            Send(handler, bytes);
        }
        private void UnconnectedMessage(Socket handler, byte[] bytes, string senderIP)
        {
            var retArr = new byte[44];
            Buffer.BlockCopy(bytes, 0, retArr, 0, retArr.Length);

            retArr[2] = 0x14;
            retArr[3] = 0x00;
            retArr[38] = 0x04;
            retArr[39] = 0x00;
            retArr[40] = 0xcd;
            retArr[41] = 0x00;
            retArr[42] = 0x00;
            retArr[43] = 0x00;

            Send(handler, retArr);

            var writeReqLength = Convert.ToInt32(bytes[48]);

            var dataArray = new byte[writeReqLength];
            Buffer.BlockCopy(bytes, 50, dataArray, 0, writeReqLength);

            var remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            var senderIp = remoteIpEndPoint.Address.ToString();

            ParseLogixRecievedData(senderIp, dataArray, senderIP);

        }

        private void FowardOpen(Socket handler, byte[] bytes, StateObject state, string senderIP)
        {
            var retArr = new byte[70];
            Buffer.BlockCopy(bytes, 0, retArr, 0, retArr.Length);
            //Buffer.BlockCopy(bytes, 52, plcID, 0, 4);

            retArr[2] = 0x2e;
            retArr[3] = 0x00;

            retArr[38] = 0x1e;
            retArr[39] = 0x00;
            retArr[40] = 0xd4;
            retArr[41] = 0x00;
            retArr[42] = 0x00;
            retArr[43] = 0x00;
            retArr[44] = state.conId[0]; //0x07
            retArr[45] = state.conId[1]; //0x01
            retArr[46] = state.conId[2]; //0x00
            retArr[47] = state.conId[3]; //0x8a
            retArr[48] = state.plcId[0]; //0x00
            retArr[49] = state.plcId[1]; //0x07
            retArr[50] = state.plcId[2]; //0x00
            retArr[51] = state.plcId[3]; //0x11
            retArr[52] = state.orgSn[0];//0x11;
            retArr[53] = state.orgSn[1];//0x00;
            retArr[54] = 0x01;
            retArr[55] = 0x00;
            retArr[56] = 0x02; //0x8a
            retArr[57] = 0xbb; //0x76
            retArr[58] = 0x47; //0x1e
            retArr[59] = 0x60; //0xbc
            retArr[60] = 0xe0; //0x40
            retArr[61] = 0x70; //0x82
            retArr[62] = 0x72; //0x1f
            retArr[63] = 0x00; //0x00
            retArr[64] = 0x01;
            retArr[65] = 0x00;
            retArr[66] = 0x00;
            retArr[67] = 0x00;
            retArr[68] = 0x00;
            retArr[69] = 0x00;

            Send(handler, retArr);



        }

        private void SendUnitData(Socket handler, byte[] bytes, StateObject state)
        {
            var rArr = new byte[61];
            Buffer.BlockCopy(bytes, 0, rArr, 0, rArr.Length);

            rArr[2] = 0x25;

            rArr[32] = 0xa1;
            rArr[33] = 0x00;
            rArr[34] = 0x04;
            rArr[35] = 0x00;

            rArr[36] = state.plcId[0];
            rArr[37] = state.plcId[1];
            rArr[38] = state.plcId[2];
            rArr[39] = state.plcId[3];

            rArr[42] = 0x11;
            rArr[43] = 0x00;
            rArr[44] = bytes[44];
            rArr[45] = bytes[45]; //0x00;
            rArr[46] = 0xcb;
            rArr[47] = 0x00;
            rArr[48] = 0x00;
            rArr[49] = 0x00;
            rArr[50] = bytes[52];// 0x07;
            rArr[51] = bytes[53];// 0x01;
            rArr[52] = bytes[54];// 0x00;
            rArr[53] = bytes[55];
            rArr[54] = bytes[56];
            rArr[55] = bytes[57];
            rArr[56] = bytes[58];
            rArr[57] = 0x4f;
            rArr[58] = bytes[60];
            rArr[59] = bytes[61];
            rArr[60] = bytes[62];

            Send(handler, rArr);

            var copyLenth = bytes[64] + 5;

            var bytesToParse = new byte[copyLenth];

            Buffer.BlockCopy(bytes, 64, bytesToParse, 0, bytesToParse.Length);

            var remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            var senderIp = remoteIpEndPoint.Address.ToString();

            ParseSlcRecievedData(senderIp, bytesToParse, true);
        }

        private void ParseLogixRecievedData(string senderIp,byte[] bytes, string senderIP)
        {
            var tagNameLen = (int)bytes[3];
            var tagNameArr = new byte[tagNameLen];
            Buffer.BlockCopy(bytes, 4, tagNameArr, 0, tagNameLen);
            var tagName = System.Text.Encoding.Default.GetString(tagNameArr) + "[0]";
            var dataLength = 3 + tagNameLen + 5;
            var startingPoint = 3 + tagNameLen + 5 + 2;

            var arrLen = bytes[dataLength] * 2;

            var tempArr = new byte[arrLen];
            Buffer.BlockCopy(bytes, startingPoint, tempArr, 0, arrLen);

            var dataArray = new Int16[50];

            for (int i = 0; i < tempArr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var c = Convert.ToInt16(Util.Convert2BytesToInteger(tempArr[i], tempArr[i + 1]));
                    dataArray[i / 2] = c;
                }
            }

            switch (dataArray[18])
            {
                case 0: //pre process
                case 1:
                    ParseProductionTransaction(senderIp, dataArray, tagName);
                    break;
                case 2:
                case 3:
                case 19:
                    ParseLoginTransaction(senderIp, dataArray, tagName);
                    break;
                case 4:
                case 5:
                case 6:
                case 14:
                case 15:
                case 16:
                    ParseSetupTransaction(senderIp, dataArray, tagName);
                    break;
                case 21: //serial request
                    ParseRequestSerialTransaction(senderIp, dataArray, tagName);
                    break;
                case 31:
                    // print final label
                    ParsePrintFinalLabelTransaction(senderIp, dataArray, tagName);
                    break;
                case 32:
                    // print interim label

                    break;
                default:
                    if (tagName == "N247[20]")
                        ParseRequestSerialTransaction(senderIp, dataArray, tagName);

                    break;
            }

        }

    }
}
