using System;
using NUnit.Framework;

namespace PlaywrightTests.Utils
{
    public static class Logger
    {
        public static void Info(string message)
        {
            string logMessage = $"[INFO] {DateTime.Now:HH:mm:ss} - {message}";
            TestContext.Progress.WriteLine(logMessage);
        }

        public static void Error(string message)
        {
            string logMessage = $"[ERROR] {DateTime.Now:HH:mm:ss} - {message}";
            TestContext.Progress.WriteLine(logMessage);
        }
    }
}