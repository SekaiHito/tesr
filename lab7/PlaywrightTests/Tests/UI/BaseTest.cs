using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Config;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Tests.UI
{
    public class BaseTest
    {
        protected IPlaywright PlaywrightInstance;
        protected IBrowser Browser;
        protected IPage Page;

        [SetUp]
        public async Task Setup()
        {
            Logger.Info("Ініціалізація тестового середовища...");
            PlaywrightInstance = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions 
            { 
                Headless = ConfigManager.Settings.Headless 
            };

            Browser = ConfigManager.Settings.Browser.ToLower() switch
            {
                "firefox" => await PlaywrightInstance.Firefox.LaunchAsync(launchOptions),
                "webkit" => await PlaywrightInstance.Webkit.LaunchAsync(launchOptions),
                _ => await PlaywrightInstance.Chromium.LaunchAsync(launchOptions)
            };

            Page = await Browser.NewPageAsync();
            Page.SetDefaultTimeout(ConfigManager.Settings.Timeout);
        }

        [TearDown]
        public async Task Teardown()
        {
            Logger.Info("Завершення тесту і закриття браузера...");
            if (Browser != null)
            {
                await Browser.CloseAsync();
            }
            PlaywrightInstance?.Dispose();
        }
    }
}