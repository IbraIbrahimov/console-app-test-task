using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = new ReadAndWrite();
            result.Runner();
        }
    }
    public class ReadAndWrite
    {
        private const int ReadTimeSec = 1;
        private const int WriteTimeSec = 2;

        private bool isDataReady;
        private int currentData;

        public void Read()
        {
            while (true)
            {
                lock (this)
                {
                    if (!isDataReady)
                    {
                        Monitor.Wait(this);
                    }
                    Console.WriteLine($"Read: {currentData}");

                    isDataReady = false;

                    Monitor.Pulse(this);
                }

                Thread.Sleep(ReadTimeSec * 1000);
            }
        }

        public void Write()
        {
            while (true)
            {
                lock (this)
                {
                    if (isDataReady)
                    {
                        Monitor.Wait(this);
                    }

                    currentData++;

                    Console.WriteLine($"Write: {currentData}");
                    isDataReady = true;
                    Monitor.Pulse(this);
                }
                Thread.Sleep(WriteTimeSec * 1000);
            }
        }

        public void Runner()
        {
            var readTask = Task.Run(() => Read());
            var writeTask = Task.Run(() => Write());

            Task.WaitAll(readTask, writeTask);
        }
    }
}
