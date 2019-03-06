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
        public short RequestCode { get; set; }
        public short Status { get; set; }
        public short FailureCode { get; set; }
        public string OperatorID { get; set; }
        public string Component { get; set; }
        public short AccessId { get; set; }
        public string ModelNumber { get; set; }
        public string OpNumber { get; set; }
        public string[] ProcessHistoryValues { get; set; }
        public string Weight { get; set; }
        public string PrinterIpAddress { get; set; }
        public string RevLevel { get; set; }
    }
}
