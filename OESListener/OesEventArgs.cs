using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class OesEventArgs : EventArgs
    {
        public bool UseJson { get; set; }
        public System.Net.Sockets.TcpClient Client { get; set; }
        public string CellID { get; set; }

    }
}
