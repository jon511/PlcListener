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
        internal ListenerType listenerType { get; set; }
        public string InTagName { get; set; }
        public string OutTagName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public System.Net.Sockets.TcpClient Client { get; set; }
        public string SenderIp { get; set; }
        public string CellId { get; set; }
        public string ItemId { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public string FailureCode { get; set; }

    }
}
