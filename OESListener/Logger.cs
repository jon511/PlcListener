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
        static ConcurrentQueue<string> itemsToWrite = new ConcurrentQueue<string>();
        //static bool logToConsole = false;
        static bool done = false;
        
        static Logger()
        {
            Task consumerTask = new Task(ConsumerMethod);
            consumerTask.Start();
        }

        //prints messages to file (almost) as fast as possible
        static void ConsumerMethod()
        {
            using (StreamWriter fileout = File.CreateText("Log.txt"))
            {
                fileout.AutoFlush = true;

                while (done == false)
                {
                    string itemToWrite;
                    while (itemsToWrite.TryDequeue(out itemToWrite))
                    {
                        fileout.WriteLine(itemToWrite);
                        Console.WriteLine(itemToWrite);
                    }

                    Thread.Sleep(10);  //sleep for 10 milliseconds
                }
            }
        }

        //adds messages to the queue to print to the file
        public static void Log(string message)
        {
            itemsToWrite.Enqueue(message);

            //int sleepTime = rng.Next(2000, 5000); //2-5 seconds
            //itemsToWrite.Enqueue(DateTime.Now + " Thread " + id + ":  Working on report for " +
            //                     sleepTime + " miliseconds...");

            //Thread.Sleep(sleepTime);

            //itemsToWrite.Enqueue(DateTime.Now + " Thread " + id + ":  Done!");
        }


    }
}
