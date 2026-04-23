using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Async;

namespace PlaywrightTests.Core.Managers
{
    /// <summary>
    /// Manages Serilog logger initialization and configuration.
    /// Provides centralized logging with file and console output with asynchronous processing.
    /// </summary>
    public static class LoggerManager
    {
        private static ILogger _logger;
        private static bool _initialized;

        /// <summary>
        /// Gets the configured logger instance.
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("Logger not initialized. Call Initialize() first.");
                }
                return _logger;
            }
        }

        /// <summary>
        /// Initializes the logger with the specified minimum level.
        /// </summary>
        public static void Initialize(string minimumLevel = "Information")
        {
            if (_initialized)
                return;

            try
            {
                // Ensure Logs directory exists
                string logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                LogEventLevel logLevel = ParseLogLevel(minimumLevel);

                string logFilePath = Path.Combine(
                    logsDirectory,
                    $"test-{DateTime.Now:yyyy-MM-dd}.log"
                );

                _logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithThreadId()
                    .Enrich.WithMachineName()
                    .WriteTo.Async(a =>
                    {
                        a.Console(
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}");

                        a.File(
                            logFilePath,
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{MachineName}] [{EnvironmentUserName}] [{ThreadId}] {Message:lj}{NewLine}{Exception}",
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 10,
                            fileSizeLimitBytes: 10_000_000,
                            rollOnFileSizeLimit: true);
                    })
                    .CreateLogger();

                _initialized = true;
                _logger.Information("Logger initialized successfully");
                _logger.Information("Log file: {LogFilePath}", logFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize logger: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        public static void LogInfo(string message, params object[] args)
        {
            Logger.Information(message, args);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public static void LogDebug(string message, params object[] args)
        {
            Logger.Debug(message, args);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void LogWarning(string message, params object[] args)
        {
            Logger.Warning(message, args);
        }

        /// <summary>
        /// Logs an error message with optional exception.
        /// </summary>
        public static void LogError(string message, Exception ex = null, params object[] args)
        {
            if (ex != null)
                Logger.Error(ex, message, args);
            else
                Logger.Error(message, args);
        }

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        public static void LogFatal(string message, Exception ex = null)
        {
            if (ex != null)
                Logger.Fatal(ex, message);
            else
                Logger.Fatal(message);
        }

        /// <summary>
        /// Flushes and closes the logger.
        /// </summary>
        public static void Shutdown()
        {
            if (_initialized)
            {
                _logger.Information("Shutting down logger");
                Log.CloseAndFlush();
                _initialized = false;
            }
        }

        private static LogEventLevel ParseLogLevel(string level)
        {
            return level?.ToLower() switch
            {
                "debug" => LogEventLevel.Debug,
                "information" or "info" => LogEventLevel.Information,
                "warning" or "warn" => LogEventLevel.Warning,
                "error" => LogEventLevel.Error,
                "fatal" or "critical" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }
    }
}
