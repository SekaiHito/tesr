using System;
using System.IO;
using Serilog;
using PlaywrightTests.Config;

namespace PlaywrightTests.Core.Managers
{
    /// <summary>
    /// Helper class for Allure reporting setup and management.
    /// Handles Allure results directory initialization and configuration.
    /// </summary>
    public static class AllureHelper
    {
        private static ILogger _logger;

        /// <summary>
        /// Initializes Allure environment and clears previous results if configured.
        /// </summary>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (!ConfigManager.Allure.Enabled)
            {
                _logger.Information("Allure reporting is disabled");
                return;
            }

            try
            {
                string resultsDirectory = ConfigManager.Allure.ResultsDirectory;

                // Create results directory if it doesn't exist
                if (!Directory.Exists(resultsDirectory))
                {
                    Directory.CreateDirectory(resultsDirectory);
                    _logger.Information("Allure results directory created: {Directory}", resultsDirectory);
                }

                // Clean results before run if configured
                if (ConfigManager.Allure.CleanBeforeRun && Directory.Exists(resultsDirectory))
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(resultsDirectory))
                        {
                            File.Delete(file);
                        }
                        _logger.Information("Allure results directory cleaned");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Failed to clean Allure results directory");
                    }
                }

                _logger.Information("Allure initialized successfully. Results directory: {Directory}", resultsDirectory);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize Allure");
                throw;
            }
        }

        /// <summary>
        /// Gets the Allure results directory path.
        /// </summary>
        public static string GetResultsDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), ConfigManager.Allure.ResultsDirectory);
        }

        /// <summary>
        /// Gets the Allure report directory path.
        /// </summary>
        public static string GetReportDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "allure-report");
        }

        /// <summary>
        /// Attachs a file to Allure report.
        /// </summary>
        public static void AttachFile(string filePath, string name, string mimeType = "application/octet-stream")
        {
            _logger?.Debug("Attaching file to Allure: {Name}", name);

            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    // Note: Actual attachment would be handled by test framework
                    _logger?.Debug("File attachment prepared: {Name}", name);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Failed to attach file: {Name}", name);
            }
        }

        /// <summary>
        /// Creates Allure test details.
        /// </summary>
        public static void LogTestDetails(string feature, string story, string testName)
        {
            _logger?.Information("[ALLURE] Feature: {Feature}, Story: {Story}, Test: {TestName}",
                feature, story, testName);
        }
    }
}
