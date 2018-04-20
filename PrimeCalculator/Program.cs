using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PrimeCalculator
{
    class Program : IDisposable
    {
        const string filePath = @".\output.txt";
        static FileStream fileStream;

        static void Main(string[] args)
        {
            // Register Close Event


            // Clear up previous run
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);

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

                    ProcessWrite(i.ToString() + Environment.NewLine).Wait();

                }
            }
        }

        static async Task WriteTextAsync(string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
        }

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
