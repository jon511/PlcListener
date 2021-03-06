﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OESListener
{
    class Util
    {
        internal static Dictionary<int, string> SlcErrorCode = new Dictionary<int, string> {
            { 0x00, "GOOD" },
            { 0x01, "DST node is out of buffer space" },
            { 0x02, "Cannot guarantee delivery: link layer (The remote node specified does not ACK command.)" },
            { 0x03, "Duplicate token holder detected" },
            { 0x04, "Local port is disconnected" },
            { 0x05, "Application layer timed out waiting for a response" },
            { 0x06, "Duplicate node detected" },
            { 0x07, "Station is offline" },
            { 0x08, "Hardware fault" },
            { 0x10, "Illegal command or format" },
            { 0x20, "Host has a problem and will not communicate" },
            { 0x30, "Remote node host is missing, disconnected, or shut down" },
            { 0x40, "Host could not complete function due to hardware fault" },
            { 0x50, "Addressing problem or memory protect rungs" },
            { 0x60, "Function not allowed due to command protection selection" },
            { 0x70, "Processor is in Program mode" },
            { 0x80, "Compatibility mode file missing or communication zone problem" },
            { 0x90, "Remote node cannot buffer command" },
            { 0xa0, "Wait ACK (1775KA buffer full)"},
            { 0xb0, "Remote node problem due to download" },
            { 0xc0, "Wait ACK (1775KA buffer full)" },
            { 0xf0, "Error code in the EXT STS byte" }
        };

        internal static Dictionary<int, string> SlcExtErrorCode = new Dictionary<int, string> {
            { 0x00, "GOOD" },
            { 0x01, "A field has an illegal value" },
            { 0x02, "Less levels specified in address than minimum for any address" },
            { 0x03, "More levels specified in address than system supports" },
            { 0x04, "Symbol not found" },
            { 0x05, "Symbol is of improper format" },
            { 0x06, "Address doesn't point to something usable" },
            { 0x07, "File is wrong size" },
            { 0x08, "Cannot complete request, situation has changed since the start of the command" },
            { 0x09, "Data or file is too large" },
            { 0x0a, "Transaction size plus word address is too large" },
            { 0x0b, "Access denied, improper privilege" },
            { 0x0c, "Condition cannot be generated  resource is not available" },
            { 0x0d, "Condition already exists  resource is already available" },
            { 0x0e, "Command cannot be executed" },
            { 0x0f, "Histogram overflow" },
            { 0x10, "No access" },
            { 0x11, "Host has a problem and will not communicate" },
            { 0x12, "" },
            { 0x13, "" },
            { 0x14, "" },
            { 0x15, "" },
            { 0x16, "" },
            { 0x17, "" },
            { 0x18, "" },
            { 0x19, "" },
            { 0x22, "" },
            { 0x23, "" },
            { 0x24, "" },
            { 0x1a, "" },
            { 0x1b, "" },
            { 0x1c, "" },
            { 0x1d, "" },
            { 0x1e, "" },
            { 0x1f, "" },
        };

        internal static Dictionary<int, string> LogixErrorCode = new Dictionary<int, string>
        {
            { 0x00, "GOOD" },
            { 0x04, "A syntax error was detected decoding the Request Path" },
            { 0x05, "Request Path destination unknown: Probably instance number is not present"},
            { 0x2101, "Device state conflict: keyswitch position: The requestor is attempting to change force information in HARD RUN mode" },
            { 0x2802, "Device state conflict: Safety Status: The controller is in a state in which Safety. Memory cannot be modified." },
            { 0x13, "Insufficient Request Data: Data too short for expected parameters." },
            { 0x26, "The Request Path Size received was shorter or longer than expected." },
            { 0x2104, "General Error: Offset is beyond end of the requested tag." },
            { 0x2105, "General Error: Offset plus Number of Elements extends beyond the end of the requested tag." },
            { 0x2107, "General Error: Data type used in request does not match target tag’s data type." }
        };
        internal static byte[] ConvertIntToFourBytes(int val)
        {
            var b = new byte[4];
            b[0] = Convert.ToByte(val & 0x000000ff);
            b[1] = Convert.ToByte((val & 0x0000ff00) >> 8);
            b[2] = Convert.ToByte((val & 0x00ff0000) >> 16);
            b[3] = Convert.ToByte((val & 0xff000000) >> 24);

            return b;
        }

        internal static byte[] ConvertIntToTwoBytes(int val)
        {
            var b = new byte[2];
            b[0] = Convert.ToByte(val & 0x000000ff);
            b[1] = Convert.ToByte((val & 0x0000ff00) >> 8);

            return b;
        }

        internal static int Convert2BytesToInteger(byte b1, byte b2)
        {
            return (short)((b2 << 8) | b1);
            //return Convert.ToInt16(b2 << 8) + Convert.ToInt16(b1);
        }

        internal static short[] ConvertStringToByteArray(string s)
        {
            var bc = (char)0;
            if (s.Length % 2 == 1)
                s += bc;

            var len = s.Length;

            var retArr = new List<short>();
            for (var i = 0; i < len; i++)
            {
                if (i % 2 == 0)
                {
                    retArr.Add(Convert.ToInt16((s[i] << 8) + s[i + 1]));
                }
            }

            return retArr.ToArray();
        }

        internal static short[] StringToAbIntArray(string s)
        {
            if (s.Count() % 2 == 1)
            {
                s += char.MinValue;
            }

            var arr = new List<short>();

            for (var i = 0; i < s.Count(); i++)
            {
                if (i % 2 == 0)
                {
                    arr.Add((short)((s[i] * 256) + s[i + 1]));
                }
            }

            return arr.ToArray();

        }
        internal static string AbIntArrayToString(short[] data)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                var s1 = data[i] & 0xff00;
                s1 = s1 >> 8;
                var s2 = data[i] & 0xff;
                
                sb.Append(Convert.ToChar(s1));
                sb.Append(Convert.ToChar(s2));
            }

            return sb.ToString();
        }

        internal static byte[] ConvertToByteArray(short[] arr)
        {
            var retArr = new List<byte>();
            foreach (var item in arr)   
            {
                retArr.Add((byte)(item & 0xff));
                retArr.Add((byte)((item & 0xff00) >> 8));
            }

            return retArr.ToArray();
        }

        internal static string GetSlcDataType(byte val)
        {
            // file types from slc
            //80-􏰀83 hex: reserved •84 hex: status
            // •85 hex: bit
            // •86 hex: timer
            // •87 hex: counter
            // •88 hex: control
            // •89 hex: integer
            // •8A hex: floating point •8D hex: string
            // •8E hex: ASCII
            switch (val)
            {
                case 0x85:
                    return "B";
                case 0x86:
                    return "T";
                case 0x87:
                    return "C";
                case 0x88:
                    return "R";
                case 0x89:
                    return "N";
                case 0x8a:
                    return "F";
                case 0x8d:
                    return "S";
                case 0x8e:
                    return "A";
                default:
                    return "";
            }

        }

        internal static byte GetSlcDataTypeCode(char str)
        {
            switch (str)
            {
                case 'B':
                    return 0x85;
                case 'T':
                    return 0x86;
                case 'C':
                    return 0x87;
                case 'R':
                    return 0x88;
                case 'N':
                    return 0x89;
                case 'F':
                    return 0x8a;
                case 'S':
                    return 0x8d;
                case 'A':
                    return 0x8e;
                default:
                    return 0;
            }
        }

        internal static byte[] DesturctureSlcTag(string tag)
        {
            var dataType = GetSlcDataTypeCode(tag.First());
            var addressArr = tag.Substring(1).Split(':');
            Byte.TryParse(addressArr[0], out byte addr);
            Byte.TryParse(addressArr[1], out byte ele);

            return new byte[]{ addr, dataType, ele, 0 };
        }

        internal static byte[] ForwardOpenPacket(byte[] sessionHandle, byte processorSlot)
        {
            var fwdOpen = buildCIPForwardOpen(processorSlot);
            var rrDataHeader = new List<byte>();
            rrDataHeader.AddRange(buildEIPSendDataHeader(sessionHandle, fwdOpen.Length));
            rrDataHeader.AddRange(fwdOpen);
            return rrDataHeader.ToArray();
        }

        internal static byte[] ForwardClosePacket(byte[] sessionHandel, byte processorSlot, int connectionSerialNumber)
        {
            var fwdClose = buildCipFowardClose(processorSlot, connectionSerialNumber);
            var rrDataHeader = new List<byte>();
            rrDataHeader.AddRange(buildEIPSendDataHeader(sessionHandel, fwdClose.Length));
            rrDataHeader.AddRange(fwdClose);
            return rrDataHeader.ToArray();
        }

        private static int connectionSerial = 1;
        internal static byte[] buildCIPForwardOpen(byte processorSlot)
        {

            
            
            if (connectionSerial > 1000 || connectionSerial < 1)
                connectionSerial = 1;

            connectionSerial++;
            var nB = Util.ConvertIntToTwoBytes(connectionSerial);

            ///////////////////////
            return new byte[] {0x54, 0x02, 0x20, 0x06, 0x24, 0x01, 0x0a, 0xff, 0x02, 0x00, 0x00, 0x20, 0x01, 0x00, 0x00, 0x20, nB[0], nB[1], 0x37,
                0x13, 0x42, 0x00, 0x00, 0x00, 0x02/*0x03*/, 0x00, 0x00, 0x00, 0x34, 0x12, 0x20, 0x00, 0xf4, 0x43, 0x01, 0x40, 0x20, 0x00, 0xf4, 0x43, 0xa3, 0x04/*0x03*/,
                0x01, processorSlot, 0x20, 0x02, 0x24, 0x01, 0x2c, 0x01 };

        }

        internal static byte[] buildCipFowardClose(byte processorSlot, int connectionSerialNumber)
        {
            var nB = Util.ConvertIntToTwoBytes(connectionSerial);
            return new byte[] { 0x4e, 0x02, 0x20, 0x06, 0x24, 0x01, 0xa, 0x0e, nB[0], nB[1], 0x37, 0x13, 0x42, 0x00, 0x00, 0x00, 0x03, 0x00, 0x01, processorSlot, 0x20, 0x02, 0x24, 0x01 };
        }

        internal static byte[] buildEIPSendDataHeader(byte[] sessionHandle, int frameLen)
        {
            byte[] eipLength = Util.ConvertIntToTwoBytes(16 + frameLen);
            byte[] eipItem2Length = Util.ConvertIntToTwoBytes(frameLen);
            return new byte[] {0x6f, 0x00, eipLength[0], eipLength[1], sessionHandle[0], sessionHandle[1], sessionHandle[2], sessionHandle[3],
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xb2, 0x00, eipItem2Length[0], eipItem2Length[1]};
        }

        internal static byte[] EipPcccWrapper(byte[] sessionHandle, int dataLen)
        {

            var eipCommand = new byte[] { 0x6f, 0x00 };
            var eipLength = Util.ConvertIntToTwoBytes(dataLen);
            var eipStatus = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var senderContext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var eipOptions = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            var eipInterfaceHandle = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var eipTimeOut = new byte[] { 0x00, 0x00 };
            var eipItemCount = new byte[] { 0x02, 0x00 };

            var retList = new List<byte>();
            retList.AddRange(eipCommand);
            retList.AddRange(eipLength);
            retList.AddRange(sessionHandle);
            retList.AddRange(eipStatus);
            retList.AddRange(senderContext);
            retList.AddRange(eipOptions);
            retList.AddRange(eipInterfaceHandle);
            retList.AddRange(eipTimeOut);
            retList.AddRange(eipItemCount);

            return retList.ToArray();
        }

        internal static byte[] Unconnected_Send(byte[] tagIOI, byte[] sessionHandle)
        {
            var eipCommand = new byte[] { 0x6f, 0x00 };
            var eipLength = Util.ConvertIntToTwoBytes(tagIOI.Length + 54); // implement after getting data
            //var eipLength = Util.ConvertIntToTwoBytes(tagIOI.Length + 50); // implement after getting data
            //var thisSessionhandel = sessionHandle;
            var eipStatus = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var senderContext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var eipOptions = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            var eipInterfaceHandle = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var eipTimeOut = new byte[] { 0x00, 0x00 };
            var eipItemCount = new byte[] { 0x02, 0x00 };

            //1st item
            var eipNullAddressTypeId = new byte[] { 0x00, 0x00 };
            var eipNullAdressLength = new byte[] { 0x00, 0x00 };

            //2nd item
            var eipUnconnectedTypeId = new byte[] { 0xb2, 0x00 };
            var eipUnconnectedLen = Util.ConvertIntToTwoBytes(tagIOI.Length + 14); // size of tag write data + 14 bytes

            var eipUnconnectedServiceCode = new byte[] { 0x52 };
            var UnconnectedRequestPathSize = new byte[] { 0x02 };
            var unconnectedOptions = new byte[] { 0x20, 0x05, 0x24, 0x01, 0x07, 0xe9 };

            var messageRequestSize = Util.ConvertIntToTwoBytes(tagIOI.Length); //size of tag write data

            //add message data

            var retList = new List<byte>();
            retList.AddRange(eipCommand);
            retList.AddRange(eipLength);
            retList.AddRange(sessionHandle);
            retList.AddRange(eipStatus);
            retList.AddRange(senderContext);
            retList.AddRange(eipOptions);
            retList.AddRange(eipInterfaceHandle);
            retList.AddRange(eipTimeOut);
            retList.AddRange(eipItemCount);
            retList.AddRange(eipNullAddressTypeId);
            retList.AddRange(eipNullAdressLength);
            retList.AddRange(eipUnconnectedTypeId);
            retList.AddRange(eipUnconnectedLen);
            retList.AddRange(eipUnconnectedServiceCode);
            retList.AddRange(UnconnectedRequestPathSize);
            retList.AddRange(unconnectedOptions);
            retList.AddRange(messageRequestSize);
            retList.AddRange(tagIOI);

            retList.AddRange(new byte[] { 0x01, 0x00, 0x01, 0x00 });

            return retList.ToArray();




        }

        private static int sequenceCounter = 0;
        internal static byte[] Build_EIP_CIP_Header(byte[] tagIOI, byte[] sessionHandle, byte[] connectionId)
        {
            var eipConnectedDataLength = tagIOI.Length + 2;

            var eipCommand = new List<byte>() { 0x70, 0x00 };
            var eipLength = Util.ConvertIntToTwoBytes(22 + tagIOI.Length);
            //var eipSessionHandle = s;
            var eipStatus = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var eipContext = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var eipOptions = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var eipInterfaceHandle = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var eipTimeout = new byte[] { 0x00, 0x00 };
            var eipItemCount = new byte[] { 0x02, 0x00 };
            var eipItem1ID = new byte[] { 0xa1, 0x00 };
            var eipItem1Length = new byte[] { 0x04, 0x00 };
            var eipItem1 = connectionId; //connectionId
            var eipItem2ID = new byte[] { 0xb1, 0x00 };
            var eipItem2Length = new byte[]{(byte)eipConnectedDataLength, 0x00};
            var eipSequence = Util.ConvertIntToTwoBytes(sequenceCounter);

            sequenceCounter++;
            if (sequenceCounter > 32767)
                sequenceCounter = 0;

            eipCommand.AddRange(eipLength);
            eipCommand.AddRange(sessionHandle);
            eipCommand.AddRange(eipStatus);
            eipCommand.AddRange(eipContext);
            eipCommand.AddRange(eipOptions);
            eipCommand.AddRange(eipInterfaceHandle);
            eipCommand.AddRange(eipTimeout);
            eipCommand.AddRange(eipItemCount);
            eipCommand.AddRange(eipItem1ID);
            eipCommand.AddRange(eipItem1Length);
            eipCommand.AddRange(eipItem1);
            eipCommand.AddRange(eipItem2ID);
            eipCommand.AddRange(eipItem2Length);
            eipCommand.AddRange(eipSequence);
            eipCommand.AddRange(tagIOI);

            return eipCommand.ToArray();
        }

        internal static byte[] createWriteRequest(string tagName, short[] data)
        {
            var tagNameArr = tagName.Split('[');
            string TagName;
            byte[] element = null;
            if (tagNameArr.Length > 1)
            {
                var eleString = tagNameArr[1].Substring(0, tagNameArr[1].Length - 1);
                byte.TryParse(eleString, out byte ele);
                if (ele > 0)
                    element = new byte[] { 0x28, ele };
            }
                

            TagName = tagNameArr[0];

            var requestService = new List<byte>() { 0x4D };
            //var requestService = new List<byte>() { 0x53 };
            var requestPath = new List<byte>() { 0x91, (byte)TagName.Length };
            requestPath.AddRange(Encoding.ASCII.GetBytes(TagName));

            if (TagName.Length % 2 != 0)
                requestPath.Add(0x00);

            if (element != null)
            {
                requestPath.AddRange(element);
            }

            var requestPathSize = requestPath.Count() / 2;
            var requestDataType = Util.ConvertIntToTwoBytes(0xc3);
            var requestDataLen = Util.ConvertIntToTwoBytes(data.Length);

            var writeValue = new List<byte>();
            foreach (var item in data)
            {
                writeValue.AddRange(Util.ConvertIntToTwoBytes(item));
            }

            requestService.Add((byte)requestPathSize);
            requestService.AddRange(requestPath);
            requestService.AddRange(requestDataType);
            requestService.AddRange(requestDataLen);
            requestService.AddRange(writeValue);

            return requestService.ToArray();
        }

        internal static void DisplayHexValues(byte[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                Console.Write(string.Format("{1} : 0x{0:x2}, ", arr[i], i));
            }
        }
        

        
    }
}
