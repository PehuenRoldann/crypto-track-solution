using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.db
{
    public class Logger
    {

        private static readonly object lockObj = new object();
        private static readonly string logFilePath = "error.log";



        public static async Task LogErrorAsync(string message, Exception exception)
        {
            await Task.Run(() => 
            {
                lock(lockObj)
                {
                    try
                    {
                        using ( var writer = new System.IO.StreamWriter(logFilePath, true))
                        {
                            writer.WriteLine($"[ERROR] {DateTime.Now}: {message}");
                            writer.WriteLine($"Exception: {exception.Message}");
                            writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                            writer.WriteLine();
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine($"Error while writting in the log file: {error.Message}");
                    }
                }
            });
        }
        
    }
}