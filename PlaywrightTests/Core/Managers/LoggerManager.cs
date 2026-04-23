namespace PlaywrightTests.Core.Managers;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using PlaywrightTests.Core.Config;

public class LoggerManager
{
    private static LoggerManager? _instance;
    private ILogger? _logger;
    private readonly object _lockObject = new();

    private LoggerManager()
    {
        InitializeLogger();
    }

    /// <summary>
    /// Gets singleton instance of LoggerManager
    /// </summary>
    public static LoggerManager Instance => _instance ??= new LoggerManager();

    /// <summary>
    /// Gets the configured Serilog logger
    /// </summary>
    public ILogger Logger => _logger ?? Log.Logger;

    /// <summary>
    /// Initializes Serilog logger with configuration
    /// </summary>
    private void InitializeLogger()
    {
        lock (_lockObject)
        {
            try
            {
                var config = ConfigManager.Instance.Framework.Logging;
                var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

                // Create logs directory if it doesn't exist
                if (!Directory.Exists(logsPath))
                {
                    Directory.CreateDirectory(logsPath);
                }

                // Parse minimum level
                var minimumLevel = Enum.Parse<LogEventLevel>(config.MinimumLevel, ignoreCase: true);

                var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Is(minimumLevel)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", "PlaywrightTests")
                    .Enrich.WithProperty("Environment", ConfigManager.Instance.CurrentEnvironment);

                // Console output
                if (config.WriteTo.Contains("Console", StringComparer.OrdinalIgnoreCase))
                {
                    loggerConfig = loggerConfig
                        .WriteTo.Console(
                            outputTemplate: config.OutputTemplate,
                            theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code);
                }

                // File output
                if (config.WriteTo.Contains("File", StringComparer.OrdinalIgnoreCase))
                {
                    var logPath = Path.Combine(logsPath, config.FilePath.Replace("Logs/", ""));
                    loggerConfig = loggerConfig
                        .WriteTo.Async(a => a.File(
                            logPath,
                            outputTemplate: config.OutputTemplate,
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 30));
                }

                _logger = loggerConfig.CreateLogger();
                Log.Logger = _logger;

                _logger.Information("===== Logger Initialized =====");
                _logger.Information("Environment: {Environment}", ConfigManager.Instance.CurrentEnvironment);
                _logger.Information("Base URL: {BaseUrl}", ConfigManager.Instance.GetBaseUrl());
                _logger.Information("Browser: {Browser}", ConfigManager.Instance.Framework.Playwright.BrowserType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logger initialization failed: {ex}");
                throw;
            }
        }
    }

    /// <summary>
    /// Logs information message
    /// </summary>
    public void Info(string message, params object?[] args)
    {
        Logger.Information(message, args);
    }

    /// <summary>
    /// Logs debug message
    /// </summary>
    public void Debug(string message, params object?[] args)
    {
        Logger.Debug(message, args);
    }

    /// <summary>
    /// Logs warning message
    /// </summary>
    public void Warning(string message, params object?[] args)
    {
        Logger.Warning(message, args);
    }

    /// <summary>
    /// Logs error message
    /// </summary>
    public void Error(string message, params object?[] args)
    {
        Logger.Error(message, args);
    }

    /// <summary>
    /// Logs error message with exception
    /// </summary>
    public void Error(Exception exception, string message, params object?[] args)
    {
        Logger.Error(exception, message, args);
    }

    /// <summary>
    /// Logs fatal error
    /// </summary>
    public void Fatal(string message, params object?[] args)
    {
        Logger.Fatal(message, args);
    }

    /// <summary>
    /// Logs fatal error with exception
    /// </summary>
    public void Fatal(Exception exception, string message, params object?[] args)
    {
        Logger.Fatal(exception, message, args);
    }

    /// <summary>
    /// Closes and flushes logger
    /// </summary>
    public void Close()
    {
        try
        {
            Logger.Information("===== Test Execution Complete =====");
            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error closing logger: {ex}");
        }
    }

    /// <summary>
    /// Resets singleton instance
    /// </summary>
    public static void Reset()
    {
        _instance?.Close();
        _instance = null;
    }
}
