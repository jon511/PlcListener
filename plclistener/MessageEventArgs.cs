using System;
using System.Collections.Generic;
using System.Text;

namespace PlcListener
{
    public class MessageEventArgs : EventArgs
    {
        public string IPSender { get; }
        public string IPAddressNIC { get; }
        public string ItemName { get; }
        public object Value { get; }
        public int Length { get; }
        public TypeCode NetType { get; }
        public DateTime Timestamp { get; }
        

        public MessageEventArgs(string ipSender, string ipAddressNic, string itemName, object value, int length,
            TypeCode netType, DateTime timestamp)
        {
            IPSender = ipSender;
            IPAddressNIC = ipAddressNic;
            ItemName = itemName;
            Value = value;
            Length = length;
            NetType = netType;
            Timestamp = timestamp;
        }

        public string CellID()
        {

            var dataArray = (Int16[])Value;
            var bytes = new List<byte>();
            for (int i = 0; i < 5; i++)
            {
                var b1 = (byte) ((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte) (dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);
                
                if (b2 != 0)
                    bytes.Add(b2);
            }

            return System.Text.Encoding.Default.GetString(bytes.ToArray());
        }

        public string SerialNumber()
        {
            var dataArray = (Int16[])Value;
            var bytes = new List<byte>();
            for (var i = 5; i < 11; i++)
            {
                var b1 = (byte) ((dataArray[i] & 0xff00) >> 8);
                var b2 = (byte) (dataArray[i] & 0x00ff);
                if (b1 != 0)
                    bytes.Add(b1);
                
                if (b2 != 0)
                    bytes.Add(b2);
            }

            return System.Text.Encoding.Default.GetString(bytes.ToArray());
        }
        
    }
}