using System;
using System.Diagnostics;
using System.IO;

namespace ElementalSpirit.Services
{
    internal static class ErrorLogger
    {
        public static void LogDiagnostic(string message)
        {
            string entry = $"[{DateTimeOffset.Now:O}] {message}{Environment.NewLine}";
            Debug.Write(entry);

            try
            {
                string logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ElementalSpirit",
                    "Logs");
                Directory.CreateDirectory(logDirectory);
                File.AppendAllText(Path.Combine(logDirectory, "diagnostic.log"), entry);
            }
            catch (Exception loggingException)
            {
                Debug.WriteLine($"[ErrorLogger] Could not write diagnostic log: {loggingException}");
            }
        }

        public static void Log(string context, Exception exception)
        {
            string message = $"[{DateTimeOffset.Now:O}] {context}{Environment.NewLine}{exception}";
            Debug.WriteLine(message);

            try
            {
                string logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ElementalSpirit",
                    "Logs");
                Directory.CreateDirectory(logDirectory);
                File.AppendAllText(Path.Combine(logDirectory, "application.log"), message + Environment.NewLine);
            }
            catch (Exception loggingException)
            {
                Debug.WriteLine($"[ErrorLogger] Could not write the application log: {loggingException}");
            }
        }
    }
}
