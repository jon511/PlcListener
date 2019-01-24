using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OESListener
{
    internal class LabelPrinter
    {
        LabelPrintEventArgs e;
        public bool UseFile { get; set; }
        public string FinalPrintStorageFolder { get; set; }
        public string InterimPrintStorageFolder { get; set; }

        public int Port = 9100;
        public LabelPrinter(LabelPrintEventArgs args)
        {
            e = args;
            FinalPrintStorageFolder = @"\PrintCode\Final\";
            InterimPrintStorageFolder = @"\Printcode\Interim";

            CheckFolders();
        }

        public LabelPrinter(LabelPrintEventArgs args, string finalLabelFolder, string interimLabelFolder)
        {
            e = args;
            FinalPrintStorageFolder = finalLabelFolder;
            InterimPrintStorageFolder = interimLabelFolder;

            CheckFolders();
        }

        private void CheckFolders()
        {
            if (!Directory.Exists(FinalPrintStorageFolder))
                Directory.CreateDirectory(FinalPrintStorageFolder);

            if (!Directory.Exists(InterimPrintStorageFolder))
                Directory.CreateDirectory(InterimPrintStorageFolder);
        }

        public int PrintLabel()
        {
            if (UseFile)
                e.PrintCode = GetPrintcode();

            if (string.IsNullOrEmpty(e.PrintCode))
                return 2;

            CreatePrintString();
            var result = SendToPrinter();
            if (result != 0)
                return result;

            Console.WriteLine(e.PrintCode);

            return 0;
        }

        private string GetPrintcode()
        {
            
            string file = FinalPrintStorageFolder + e.AlphaCode + ".txt";
            if (File.Exists(file))
                return File.ReadAllText(file);
            else
                return "";
        }

        private void CreatePrintString()
        {
            string yearString = DateTime.Now.Year.ToString();
            string monthString = DateTime.Now.Month.ToString();
            string monthString2 = (monthString.Length == 1) ? "0" + monthString : monthString;

            e.PrintCode = e.PrintCode.Replace("_SerialNumber_", e.ItemId);
            e.PrintCode = e.PrintCode.Replace("_Weight_", e.Weight);
            e.PrintCode = e.PrintCode.Replace("_Year_", yearString);
            e.PrintCode = e.PrintCode.Replace("_Month_", monthString);
            e.PrintCode = e.PrintCode.Replace("_Month02_", monthString2);
            e.PrintCode = e.PrintCode.Replace("_RevLevel_", e.RevLevel);
        }

        private int SendToPrinter()
        {
            try
            {
                // Open connection
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(e.PrinterIpAddress, Port);

                // Write ZPL String to connection
                System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
                writer.Write(e.PrintCode);
                writer.Flush();

                // Close Connection
                writer.Close();
                client.Close();
                return 0;
            }
            catch (Exception e)
            {
                return 3;
            }

        }

    }
}
