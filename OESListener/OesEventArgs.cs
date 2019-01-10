using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class OesEventArgs : EventArgs
    {
        internal bool UseJson { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public System.Net.Sockets.TcpClient Client { get; set; }
        public string CellID { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public string FailureCode { get; set; }

    }
}
