﻿using System.Net.Sockets;
using System.Text;

namespace OESListener
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public byte[] plcId = new byte[4];
        public byte[] conId = new byte[4];
        public byte[] orgSn = new byte[2];
    }
}
