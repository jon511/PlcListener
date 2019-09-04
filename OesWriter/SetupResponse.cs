using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OesWriter
{
    public struct BomModelData
    {
        public string AccessId;
        public string ModelNumber;

    }
    public class SetupResponse
    {
        public BomModelData Component1;
        public BomModelData Component2;
        public BomModelData Component3;
        public BomModelData Component4;
        public BomModelData Component5;
        public BomModelData Component6;
        public string Quantity;
        public string Acknowledge;
        public string ErrorCode;
        [Newtonsoft.Json.JsonIgnore]
        public string ResponseString { get; set; }

        //public string[] PlcModelSetup = new string[34];
    }
}
