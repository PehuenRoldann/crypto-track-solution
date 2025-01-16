using System;
using System.IO;

namespace CryptoTrackApp.src.utils
{
    public class Logger
    {
        private readonly object _lockObj = new object();
        private readonly string _logDirectoryPath = "./logs/";
        private readonly string _logFilePath;

        public Logger()
        {
            _logFilePath = Path.Combine(_logDirectoryPath, $"{DateTime.Now:yyyy-MM-dd}.log");

            // Crear el directorio si no existe
            if (!Directory.Exists(_logDirectoryPath))
            {
                Directory.CreateDirectory(_logDirectoryPath);
            }
        }

        public void Log(string message)
        {
            lock (_lockObj)
            {
                try
                {
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                    Console.WriteLine(logEntry);

                    // Crear el archivo si no existe
                    if (!File.Exists(_logFilePath))
                    {
                        using (File.Create(_logFilePath)) { }
                    }

                    // Escribir el mensaje en el archivo
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ERROR] No se pudo escribir en el log: {ex.Message}");
                }
            }
        }

        /* public async Task LogErrorAsync(string message, Exception exception)
        {
            await Task.Run(() => 
            {
                lock(_lockObj)
                {
                    try
                    {
                        using ( var writer = new System.IO.StreamWriter(_logFilePath, true))
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
        } */
        
    }
}