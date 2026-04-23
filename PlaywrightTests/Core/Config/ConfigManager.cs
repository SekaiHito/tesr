namespace PlaywrightTests.Core.Config;

using Microsoft.Extensions.Configuration;

public class FrameworkConfig
{
    public PlaywrightConfig Playwright { get; set; } = new();
    public EnvironmentsConfig Environments { get; set; } = new();
    public LoggingConfig Logging { get; set; } = new();
    public AllureConfig Allure { get; set; } = new();
    public TestDataConfig TestData { get; set; } = new();
}

public class PlaywrightConfig
{
    public string BrowserType { get; set; } = "chromium";
    public bool HeadlessMode { get; set; } = true;
    public int Timeout { get; set; } = 30000;
    public int SlowMoDelay { get; set; } = 0;
    public bool FullPage { get; set; } = false;
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;
    public List<string> LaunchArgs { get; set; } = new();
}

public class EnvironmentsConfig
{
    public string Default { get; set; } = "dev";
    public Dictionary<string, string> Urls { get; set; } = new();
}

public class LoggingConfig
{
    public string MinimumLevel { get; set; } = "Information";
    public List<string> WriteTo { get; set; } = new() { "Console", "File" };
    public string FilePath { get; set; } = "Logs/test-{Date:yyyy-MM-dd}.log";
    public string OutputTemplate { get; set; } = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
}

public class AllureConfig
{
    public bool Enabled { get; set; } = true;
    public string ResultsDirectory { get; set; } = "allure-results";
    public bool CleanBeforeRun { get; set; } = true;
}

public class TestDataConfig
{
    public int DefaultTimeout { get; set; } = 5;
    public int RetryAttempts { get; set; } = 3;
    public bool ScreenshotsOnFailure { get; set; } = true;
    public bool VideosOnFailure { get; set; } = false;
}

public class EnvironmentConfig
{
    public string Environment { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public CredentialsConfig Credentials { get; set; } = new();
    public FeaturesConfig Features { get; set; } = new();
}

public class CredentialsConfig
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class FeaturesConfig
{
    public bool DebugMode { get; set; } = false;
    public bool SlowMode { get; set; } = false;
    public bool ScreenshotsOnFailure { get; set; } = true;
    public bool VideosOnFailure { get; set; } = false;
}

public class ConfigManager
{
    private static ConfigManager? _instance;
    private readonly IConfiguration _configuration;
    private readonly FrameworkConfig _frameworkConfig;
    private readonly EnvironmentConfig _environmentConfig;
    private readonly string _currentEnvironment;

    public FrameworkConfig Framework => _frameworkConfig;
    public EnvironmentConfig Current => _environmentConfig;
    public string CurrentEnvironment => _currentEnvironment;

    private ConfigManager()
    {
        _currentEnvironment = Environment.GetEnvironmentVariable("TEST_ENV") ?? "dev";

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config/Environments/{_currentEnvironment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        _configuration = configBuilder.Build();
        _frameworkConfig = _configuration.Get<FrameworkConfig>() ?? new FrameworkConfig();
        _environmentConfig = _configuration.Get<EnvironmentConfig>() ?? new EnvironmentConfig();
    }

    /// <summary>
    /// Gets singleton instance of ConfigManager
    /// </summary>
    public static ConfigManager Instance => _instance ??= new ConfigManager();

    /// <summary>
    /// Gets base URL for current environment
    /// </summary>
    public string GetBaseUrl()
    {
        return _environmentConfig.BaseUrl ?? _frameworkConfig.Environments.Urls.GetValueOrDefault(_currentEnvironment, "");
    }

    /// <summary>
    /// Gets API URL for current environment
    /// </summary>
    public string GetApiUrl()
    {
        return _environmentConfig.ApiUrl ?? "";
    }

    /// <summary>
    /// Resets singleton instance (useful for testing)
    /// </summary>
    public static void Reset()
    {
        _instance = null;
    }

    /// <summary>
    /// Loads configuration for specific environment
    /// </summary>
    public void LoadEnvironment(string environment)
    {
        Environment.SetEnvironmentVariable("TEST_ENV", environment);
        Reset();
    }
}
