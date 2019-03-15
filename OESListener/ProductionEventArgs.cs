using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    public class ProductionEventArgs : OesEventArgs
    {
        //public string ItemId { get; set; }
        public string GeneratedBarcode { get; set; }
        public string[] ProcessHistoryValues {
            get
            {
                return new string[] { (ResponseArray[22] + (ResponseArray[23] * 0.01)).ToString(), (ResponseArray[24] + (ResponseArray[25] * 0.01)).ToString(), (ResponseArray[26] + (ResponseArray[27] * 0.01)).ToString(),
                    (ResponseArray[28] + (ResponseArray[29] * 0.01)).ToString(), (ResponseArray[30] + (ResponseArray[31] * 0.01)).ToString(), (ResponseArray[32] + (ResponseArray[33] * 0.01)).ToString(),
                    (ResponseArray[34] + (ResponseArray[35] * 0.01)).ToString(),(ResponseArray[36] + (ResponseArray[37] * 0.01)).ToString(),(ResponseArray[38] + (ResponseArray[39] * 0.01)).ToString(),
                    (ResponseArray[40] + (ResponseArray[41] * 0.01)).ToString(),(ResponseArray[42] + (ResponseArray[43] * 0.01)).ToString(),(ResponseArray[44] + (ResponseArray[45] * 0.01)).ToString(),
                    (ResponseArray[46] + (ResponseArray[47] * 0.01)).ToString(),(ResponseArray[48] + (ResponseArray[49] * 0.01)).ToString()};
            }
            set
            {
                ProcessHistoryValues = value;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_1 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_3 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_4 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_5 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_6 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_7 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_8 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_9 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_10 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_11 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_12 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_13 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_14 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_15 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_16 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_17 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_18 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_19 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_20 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_21 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_22 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_23 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_24 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_25 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_26 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_27 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short P_Val_28 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_0 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_1 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_3 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_4 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_5 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_6 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_7 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_8 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_9 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_10 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public short In_Word_11 { get; set; }



        public ProductionEventArgs(System.Net.Sockets.TcpClient client)
        {
            Client = client;
            ResponseArray = new short[50];
        }

        public ProductionEventArgs(string senderIp)
        {
            SenderIp = senderIp;
            ResponseArray = new short[50];
        }
        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, short requestType, short status, short failureCode, short[] data)
        {
            short z = 0;
            Client = client;
            CellId = cellID;
            ItemId = itemID;
            GeneratedBarcode = "";
            ProcessIndicator = requestType;
            SuccessIndicator = status;
            FaultCode = failureCode;
            StatusCode = 0;
            P_Val_1 = (data.Length > 0) ? data[0] : z;
            P_Val_2 = (data.Length > 1) ? data[1] : z;
            P_Val_3 = (data.Length > 2) ? data[2] : z;
            P_Val_4 = (data.Length > 3) ? data[3] : z;
            P_Val_5 = (data.Length > 4) ? data[4] : z;
            P_Val_6 = (data.Length > 5) ? data[5] : z;
            P_Val_7 = (data.Length > 6) ? data[6] : z;
            P_Val_8 = (data.Length > 7) ? data[7] : z;
            P_Val_9 = (data.Length > 8) ? data[8] : z;
            P_Val_10 = (data.Length > 9) ? data[9] : z;
            P_Val_11 = (data.Length > 10) ? data[10] : z;
            P_Val_12 = (data.Length > 11) ? data[11] : z;
            P_Val_13 = (data.Length > 12) ? data[12] : z;
            P_Val_14 = (data.Length > 13) ? data[13] : z;
            P_Val_15 = (data.Length > 14) ? data[14] : z;
            P_Val_16 = (data.Length > 15) ? data[15] : z;
            P_Val_17 = (data.Length > 16) ? data[16] : z;
            P_Val_18 = (data.Length > 17) ? data[17] : z;
            P_Val_19 = (data.Length > 18) ? data[18] : z;
            P_Val_20 = (data.Length > 19) ? data[19] : z;
            P_Val_21 = (data.Length > 20) ? data[20] : z;
            P_Val_22 = (data.Length > 21) ? data[21] : z;
            P_Val_23 = (data.Length > 22) ? data[22] : z;
            P_Val_24 = (data.Length > 23) ? data[23] : z;
            P_Val_25 = (data.Length > 24) ? data[24] : z;
            P_Val_26 = (data.Length > 25) ? data[25] : z;
            P_Val_27 = (data.Length > 26) ? data[26] : z;
            P_Val_28 = (data.Length > 27) ? data[27] : z;

            ResponseArray = new short[50];

        }

        public ProductionEventArgs(System.Net.Sockets.TcpClient client, string cellID, string itemID, string generatedBarcode, short requestType, short status, short failureCode, short[] data)
        {
            short z = 0;
            Client = client;
            CellId = cellID;
            ItemId = itemID;
            GeneratedBarcode = generatedBarcode;
            ProcessIndicator = requestType;
            SuccessIndicator = status;
            FaultCode = failureCode;
            StatusCode = 0;
            P_Val_1 = (data.Length > 0) ? data[0] : z;
            P_Val_2 = (data.Length > 1) ? data[1] : z;
            P_Val_3 = (data.Length > 2) ? data[2] : z;
            P_Val_4 = (data.Length > 3) ? data[3] : z;
            P_Val_5 = (data.Length > 4) ? data[4] : z;
            P_Val_6 = (data.Length > 5) ? data[5] : z;
            P_Val_7 = (data.Length > 6) ? data[6] : z;
            P_Val_8 = (data.Length > 7) ? data[7] : z;
            P_Val_9 = (data.Length > 8) ? data[8] : z;
            P_Val_10 = (data.Length > 9) ? data[9] : z;
            P_Val_11 = (data.Length > 10) ? data[10] : z;
            P_Val_12 = (data.Length > 11) ? data[11] : z;
            P_Val_13 = (data.Length > 12) ? data[12] : z;
            P_Val_14 = (data.Length > 13) ? data[13] : z;
            P_Val_15 = (data.Length > 14) ? data[14] : z;
            P_Val_16 = (data.Length > 15) ? data[15] : z;
            P_Val_17 = (data.Length > 16) ? data[16] : z;
            P_Val_18 = (data.Length > 17) ? data[17] : z;
            P_Val_19 = (data.Length > 18) ? data[18] : z;
            P_Val_20 = (data.Length > 19) ? data[19] : z;
            P_Val_21 = (data.Length > 20) ? data[20] : z;
            P_Val_22 = (data.Length > 21) ? data[21] : z;
            P_Val_23 = (data.Length > 22) ? data[22] : z;
            P_Val_24 = (data.Length > 23) ? data[23] : z;
            P_Val_25 = (data.Length > 24) ? data[24] : z;
            P_Val_26 = (data.Length > 25) ? data[25] : z;
            P_Val_27 = (data.Length > 26) ? data[26] : z;
            P_Val_28 = (data.Length > 27) ? data[27] : z;

            ResponseArray = new short[50];
        }
    }
}
