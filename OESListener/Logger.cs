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

        public static bool Enabled = false;

        private static bool logToConsole;

        public static bool LogToConsole
        {
            get { return logToConsole; }
            set {
                logToConsole = value;
                Enabled = (logToConsole || LogToFile) ? true : false;
            }
        }

        private static bool logToFile;

        public static bool LogToFile
        {
            get { return logToFile; }
            set {
                logToFile = value;
                Enabled = (logToConsole || LogToFile) ? true : false;
            }
        }



        static ConcurrentQueue<string> itemsToWrite = new ConcurrentQueue<string>();
        static bool done = false;
        
        static Logger()
        {
            Task consumerTask = new Task(ConsumerMethod);
            consumerTask.Start();
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
                        if (logToFile)
                            fileout.WriteLine(itemToWrite);

                        if (logToConsole)
                            Console.WriteLine(itemToWrite);
                    }

                    //Thread.Sleep(10);  //sleep for 10 milliseconds
                }
            }
        }

        //adds messages to the queue to print to the file
        public static void Log(string message)
        {
            itemsToWrite.Enqueue(message);
        }


    }
}
