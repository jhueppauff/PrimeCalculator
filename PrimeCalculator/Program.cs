//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="https://github.com/jhueppauff/PrimeCalculator">
// Copyright 2018 Jhueppauff
// MIT License 
// For licence details visit https://github.com/jhueppauff/PrimeCalculator/blob/master/LICENSE
// </copyright>
//-----------------------------------------------------------------------

namespace PrimeCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///  Main Program Class
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Program : IDisposable
    {
        /// <summary>
        /// Log4Net Logger
        /// </summary>
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The file path
        /// </summary>
        private const string FilePath = @".\output.txt";

        /// <summary>
        /// The file stream
        /// </summary>
        private static FileStream fileStream;

        /// <summary>
        /// Message Queue of the type Data.Message.
        /// </summary>
        private static Queue<string> queue = new Queue<string>();

        /// <summary>
        /// Message Trigger
        /// </summary>
        private static AutoResetEvent trigger = new AutoResetEvent(false);

        private static Int64 counterCalculated = 0;
        private static Int64 counterWritten = 0;

        /// <summary>
        /// The disposed value to detect redundant calls
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Finalizes an instance of the <see cref="Program"/> class.
        /// </summary>
        ~Program()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    Program.fileStream.Dispose();
                }

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Main(string[])"/> class./>
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            // Clear up previous run
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            fileStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            
            // Thread creation
            Thread queueThread = new Thread(new ThreadStart(ProcessQueue))
            {
                IsBackground = true
            };
            queueThread.Start();

            Thread calcThread = new Thread(new ThreadStart(Calculate))
            {
                IsBackground = true
            };
            calcThread.Start();


            Thread consoleUpdate = new Thread(new ThreadStart(ConsoleUpdate))
            {
                IsBackground = true
            };
            consoleUpdate.Start();

            Console.ReadLine();
        }

        private static void ConsoleUpdate()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Primes found : {counterCalculated}");
                Console.WriteLine($"Primes in queue : {queue.Count}");
                Console.WriteLine($"Primes written: {counterWritten}");
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Calculates the prime numbers
        /// </summary>
        private static void Calculate()
        {
            double j, i;
            double positiveInfinity = double.PositiveInfinity;

            for (i = 2; i < positiveInfinity; i++)
            {
                for (j = 2; j < positiveInfinity; j++)
                {
                    if (i % j == 0)
                    {
                        break;
                    }
                }

                if (i == j)
                {
                    lock (queue)
                    { 
                        counterCalculated++;
                        queue.Enqueue(i.ToString(CultureInfo.CurrentCulture) + ",");
                    }
                }
            }
        }

        /// <summary>
        /// Processes the queue.
        /// </summary>
        private static void ProcessQueue()
        {
            while (true)
            {
                trigger.WaitOne(5000);
                string[] stringArray = null;

                lock (queue)
                {
                    stringArray = queue.ToArray();
                }

                foreach (string item in stringArray)
                {
                    counterWritten++;
                    ProcessWrite(item);
                    queue.Dequeue();
                }
            }
        }

        /// <summary>
        /// Writes the text asynchronous.
        /// </summary>
        /// <param name="text">The text to write into the file.</param>
        /// <returns>Returns <see cref="Task"/></returns>
        private static async Task WriteTextAsync(string text)
        {
            try
            {
                byte[] encodedText = Encoding.Unicode.GetBytes(text);

                await fileStream.WriteAsync(encodedText, 0, encodedText.Length).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error("Error while writiing to File!", ex);
            }
        }

        /// <summary>
        /// async write handler.
        /// </summary>
        /// <param name="text">The text to write into the file.</param>
        /// <returns>Returns <see cref="Task"/></returns>
        private static Task ProcessWrite(string text)
        {
            return WriteTextAsync(text);
        } 
    }
}
