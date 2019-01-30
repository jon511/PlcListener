using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace OESListener
{
    public class PcccListener : AbPlcAsyncListener
    {
        protected static readonly ManualResetEvent allDone = new ManualResetEvent(false);

        public PcccListener(string ipAddress) : base()
        {
            myIPAddress = ipAddress;
            Port = 2222;
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
                    if (receivedBytes[0] == 0x01 && receivedBytes[1] == 0x01)
                    {
                        if (Logger.Enabled)
                            Logger.Log("Connection Received");

                        var tempArr = new byte[receivedBytes.Length];
                        Buffer.BlockCopy(receivedBytes, 0, tempArr, 0, tempArr.Length);
                        tempArr[0] = 0x02;
                        tempArr[15] = 0x05;
                        Send(handler, tempArr);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                    }

                    if (receivedBytes[0] == 0x01 && receivedBytes[1] == 0x07)
                    {
                        var retArray = new byte[36];
                        Buffer.BlockCopy(receivedBytes, 0, retArray, 0, retArray.Length);
                        retArray[0] = 0x02;
                        retArray[3] = 0x08;
                        retArray[32] = 0x4f;
                        Send(handler, retArray);

                        var dataLen = receivedBytes.Length - 37;
                        var tempArr = new byte[dataLen];
                        Array.Copy(receivedBytes, 37, tempArr, 0, tempArr.Length);
                        
                        ParseSlcRecievedData(senderIp, tempArr);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                    }

                    if (receivedBytes[1] != 0x01 && receivedBytes[1] != 0x07)
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                if (Logger.Enabled)
                    Logger.Log(ex.ToString());

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }


    }


}
