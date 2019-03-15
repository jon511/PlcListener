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

        private static readonly object balanceLock = new object();



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

            while (true)
            {
                while (itemsToWrite.TryDequeue(out string item))
                {
                    var s = string.Format("{0} {1} - {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), item);
                    
                    if (logToConsole)
                        Console.WriteLine(s);

                    if (logToFile)
                    {
                        var fileout = new StreamWriter(string.Format("{0}{3}_{1}_{2}_{4:HH}_Log.txt", logPath, DateTime.Now.Month.ToString(), 
                            DateTime.Now.Day.ToString(), DateTime.Now.Year.ToString(), DateTime.Now.Hour.ToString()), true);
                        fileout.WriteLine(s);
                        fileout.Close();
                    }
                }
                Thread.Sleep(10);
            }
        }

        //adds messages to the queue to print to the file
        public static void Log(string message)
        {
            lock (balanceLock)
            {
                if (!active)
                    StartLogger();
            }

            itemsToWrite.Enqueue(message);

        }


    }
}
