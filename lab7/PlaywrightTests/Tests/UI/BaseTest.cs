using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using Serilog;
using PlaywrightTests.Config;
using PlaywrightTests.Core.Managers;

namespace PlaywrightTests.Tests.UI
{
    /// <summary>
    /// Base test class for all UI tests.
    /// Handles browser initialization, page setup, and teardown with logging.
    /// </summary>
    public class BaseTest
    {
        protected IPage Page;
        protected BrowserManager BrowserManager;
        protected ILogger Logger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Initialize logger once for all tests
            LoggerManager.Initialize(ConfigManager.Logging.MinimumLevel);
            Logger = LoggerManager.Logger;

            // Initialize Allure reporting
            AllureHelper.Initialize(Logger);

            Logger.Information("==== Test Execution Started ====");
            Logger.Information("Environment: {Environment}", System.Environment.GetEnvironmentVariable("TEST_ENV") ?? "dev");
            Logger.Information("Browser: {Browser}", ConfigManager.Environment.Browser);
            Logger.Information("Headless: {Headless}", ConfigManager.Environment.Headless);
        }

        [SetUp]
        public async Task Setup()
        {
            Logger.Information("Test Setup: {TestName}", TestContext.CurrentContext.Test.Name);

            try
            {
                // Initialize browser manager
                var launchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = ConfigManager.Environment.Headless,
                    Args = ConfigManager.Playwright.LaunchArgs.ToArray()
                };

                BrowserManager = new BrowserManager(Logger, launchOptions);

                // Parse browser type
                var browserType = ConfigManager.Environment.Browser.ToLower() switch
                {
                    "firefox" => PlaywrightTests.Core.Managers.BrowserType.Firefox,
                    "webkit" => PlaywrightTests.Core.Managers.BrowserType.WebKit,
                    _ => PlaywrightTests.Core.Managers.BrowserType.Chromium
                };

                await BrowserManager.InitializeBrowserAsync(browserType);
                Page = BrowserManager.Page;

                // Set page timeout
                Page.SetDefaultTimeout(ConfigManager.Playwright.Timeout);

                Logger.Information("Test Setup completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Test Setup failed");
                throw;
            }
        }

        [TearDown]
        public async Task Teardown()
        {
            Logger.Information("Test Teardown: {TestName}", TestContext.CurrentContext.Test.Name);

            try
            {
                // Check test result
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    Logger.Error("Test Failed: {TestName}", TestContext.CurrentContext.Test.Name);

                    // Take screenshot on failure
                    if (ConfigManager.TestData.Screenshots && Page != null)
                    {
                        string screenshotDir = "Screenshots";
                        if (!System.IO.Directory.Exists(screenshotDir))
                            System.IO.Directory.CreateDirectory(screenshotDir);

                        string fileName = $"{screenshotDir}/{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fff}.png";
                        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = fileName, FullPage = true });
                        Logger.Warning("Screenshot saved: {FileName}", fileName);
                    }
                }
                else
                {
                    Logger.Information("Test Passed: {TestName}", TestContext.CurrentContext.Test.Name);
                }

                // Close browser
                if (BrowserManager != null)
                {
                    await BrowserManager.CloseBrowserAsync();
                    await BrowserManager.DisposeAsync();
                }

                Logger.Information("Test Teardown completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error during test teardown");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            Logger.Information("==== Test Execution Completed ====");
            LoggerManager.Shutdown();
        }
    }
}
