using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class LabelPrintEventArgs : OesEventArgs
    {
        public string Weight { get; set; }
        public string PrinterIpAddress { get; set; }
        public string RevLevel { get; set; }
    }
}
