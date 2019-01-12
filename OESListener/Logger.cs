using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OESListener
{
    public static class Logger
    {
        private static string logPath = "";

        public static string LogPath
        {
            get { return logPath; }
            set { logPath = value.EndsWith("\\") ? value : value + "\\"; }
        }

        internal static bool Enabled = false;
        

        public static bool EnableDllLogging
        {
            set { Enabled = value; }
        }

        private static bool logToConsole;

        public static bool LogToConsole
        {
            get { return logToConsole; }
            set {
                logToConsole = value;
            }
        }

        private static bool logToFile;

        public static bool LogToFile
        {
            get { return logToFile; }
            set {
                logToFile = value;
            }
        }



        static ConcurrentQueue<string> itemsToWrite = new ConcurrentQueue<string>();
        static bool done = false;
        static bool active = false;

        static void StartLogger()
        {
            Task consumerTask = new Task(ConsumerMethod);
            consumerTask.Start();
            active = true;
        }

        //prints messages to file (almost) as fast as possible
        static void ConsumerMethod()
        {

            using (StreamWriter fileout = File.CreateText(string.Format("{0}Log.txt", logPath)))
            {
                fileout.AutoFlush = true;

                while (done == false)
                {
                    string itemToWrite;
                    while (itemsToWrite.TryDequeue(out itemToWrite))
                    {
                        var s = string.Format("{0} {1} - {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), itemToWrite);
                        if (logToFile)
                            fileout.WriteLine(s);

                        if (logToConsole)
                            Console.WriteLine(s);
                    }

                    Thread.Sleep(10);  //sleep for 10 milliseconds
                }
            }
        }

        //adds messages to the queue to print to the file
        public static void Log(string message)
        {
            if (!active)
                StartLogger();
                    
            itemsToWrite.Enqueue(message);
        }


    }
}
