namespace PrimeCalculator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program : IDisposable
    {
        /// <summary>
        /// The file path
        /// </summary>
        const string filePath = @".\output.txt";

        /// <summary>
        /// The file stream
        /// </summary>
        static FileStream fileStream;

        /// <summary>
        /// Message Queue of the type Data.Message.
        /// </summary>
        private static Queue<string> queue = new Queue<string>();

        /// <summary>
        /// Message Trigger
        /// </summary>
        private static AutoResetEvent messageTrigger = new AutoResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="Main(string[])"/> class./>
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            // Clear up previous run
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            
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

            Console.ReadLine();
        }

        /// <summary>
        /// Calculates the prime numbers
        /// </summary>
        private static void Calculate()
        {
            double j, i;
            double positiveInfinity = Double.PositiveInfinity;

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
                    // Add Console Output if you like, but it will slow down the process
#pragma warning disable S125 // Sections of code should not be "commented out"
                    // Console.WriteLine(i.ToString());
#pragma warning restore S125 // Sections of code should not be "commented out"

                    lock (queue)
                    {
                        queue.Enqueue(i.ToString() + ",");
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
                messageTrigger.WaitOne(5000);
                string[] stringArray = null;

                lock (queue)
                {
                    stringArray = queue.ToArray();
                }

                foreach (string item in stringArray)
                {
                    ProcessWrite(item);
                }
            }
        }

        /// <summary>
        /// Writes the text asynchronous.
        /// </summary>
        /// <param name="text">The text to write into the file.</param>
        /// <returns></returns>
        private static async Task WriteTextAsync(string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

        /// <summary>
        /// async write handler.
        /// </summary>
        /// <param name="text">The text to write into the file.</param>
        /// <returns></returns>
        private static Task ProcessWrite(string text)
        {
            return WriteTextAsync(text);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fileStream.Dispose();
                }

                disposedValue = true;
            }
        }

        ~Program()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
