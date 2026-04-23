using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PlaywrightTests.Config
{
    /// <summary>
    /// Configuration settings for Playwright browser.
    /// </summary>
    public class PlaywrightSettings
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

    /// <summary>
    /// Environment-specific configuration.
    /// </summary>
    public class EnvironmentSettings
    {
        public string Environment { get; set; } = "dev";
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public Dictionary<string, string> Credentials { get; set; } = new();
        public Dictionary<string, bool> Features { get; set; } = new();
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; } = true;
        public int Timeout { get; set; } = 30000;
    }

    /// <summary>
    /// Logging configuration.
    /// </summary>
    public class LoggingSettings
    {
        public string MinimumLevel { get; set; } = "Information";
        public List<string> WriteTo { get; set; } = new();
        public string FilePath { get; set; } = "Logs/test-{Date:yyyy-MM-dd}.log";
        public string OutputTemplate { get; set; } = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    }

    /// <summary>
    /// Allure reporting configuration.
    /// </summary>
    public class AllureSettings
    {
        public bool Enabled { get; set; } = true;
        public string ResultsDirectory { get; set; } = "allure-results";
        public bool CleanBeforeRun { get; set; } = true;
    }

    /// <summary>
    /// Test data configuration.
    /// </summary>
    public class TestDataSettings
    {
        public int DefaultTimeout { get; set; } = 5;
        public int RetryAttempts { get; set; } = 3;
        public bool Screenshots { get; set; } = true;
        public bool Videos { get; set; } = false;
    }

    /// <summary>
    /// Manages configuration loading from multiple sources and environment variables.
    /// Provides centralized access to all framework configuration with caching.
    /// </summary>
    public static class ConfigManager
    {
        private static IConfigurationRoot _configuration;
        private static PlaywrightSettings _playwrightSettings;
        private static EnvironmentSettings _environmentSettings;
        private static LoggingSettings _loggingSettings;
        private static AllureSettings _allureSettings;
        private static TestDataSettings _testDataSettings;

        static ConfigManager()
        {
            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            string environment = System.Environment.GetEnvironmentVariable("TEST_ENV") ?? "dev";
            string basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Config/Environments/appsettings.{environment}.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            // Bind configuration sections
            _playwrightSettings = new PlaywrightSettings();
            _configuration.GetSection("Playwright").Bind(_playwrightSettings);

            _environmentSettings = new EnvironmentSettings();
            _configuration.Bind(_environmentSettings);

            _loggingSettings = new LoggingSettings();
            _configuration.GetSection("Logging").Bind(_loggingSettings);

            _allureSettings = new AllureSettings();
            _configuration.GetSection("Allure").Bind(_allureSettings);

            _testDataSettings = new TestDataSettings();
            _configuration.GetSection("TestData").Bind(_testDataSettings);
        }

        public static PlaywrightSettings Playwright => _playwrightSettings;
        public static EnvironmentSettings Environment => _environmentSettings;
        public static LoggingSettings Logging => _loggingSettings;
        public static AllureSettings Allure => _allureSettings;
        public static TestDataSettings TestData => _testDataSettings;
        public static IConfigurationRoot Configuration => _configuration;

        /// <summary>
        /// Gets a configuration value by key with optional default.
        /// </summary>
        public static string GetSetting(string key, string defaultValue = "")
        {
            return _configuration[key] ?? defaultValue;
        }

        /// <summary>
        /// Reloads configuration (useful for switching environments at runtime).
        /// </summary>
        public static void Reload()
        {
            LoadConfiguration();
        }
    }
}
