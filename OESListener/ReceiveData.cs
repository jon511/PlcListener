using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class ReceiveData
    {
        public string Command { get; set; }
        public string CellId { get; set; }
        public string ItemId { get; set; }
        public string GeneratedBarcode { get; set; }
        public string RequestCode { get; set; }
        public string Status { get; set; }
        public string FailureCode { get; set; }
        public string OperatorID { get; set; }
        public string Component { get; set; }
        public string AccessId { get; set; }
        public string ModelNumber { get; set; }
        public string OpNumber { get; set; }
        public string[] ProcessHistoryValues { get; set; }
        public string Weight { get; set; }
        public string PrinterIpAddress { get; set; }
        public string RevLevel { get; set; }
        public string PrintType { get; set; }
        public string InterimFile { get; set; }
    }
}
