using System;

namespace OESListener
{
    public enum ListenerType
    {
        TCP,
        EIP,
        PCCC
    }

    public class OesEventArgs : EventArgs
    {
        internal bool UseJson { get; set; }
        internal bool UsePlcLogix { get; set; }
        internal bool UsePlcMicrologix { get; set; }
        internal bool UsePlcSlc { get; set; }
        internal bool UsePlcFive { get; set; }
        internal ListenerType listenerType { get; set; }
        public string InTagName { get; set; }
        public string OutTagName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public System.Net.Sockets.TcpClient Client { get; set; }
        public string SenderIp { get; set; }
        public string CellId { get; set; }
        public string ItemId { get; set; }
        public short ProcessIndicator { get; set; }
        public short SuccessIndicator { get; set; }
        public short FaultCode { get; set; }
        public short StatusCode { get; set; }
        public short[] ResponseArray { get; set; }

    }
}
