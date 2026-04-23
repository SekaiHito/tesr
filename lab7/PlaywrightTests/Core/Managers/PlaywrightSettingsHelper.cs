using System;
using System.Collections.Generic;
using Microsoft.Playwright;
using PlaywrightTests.Config;

namespace PlaywrightTests.Core.Managers
{
    /// <summary>
    /// Helper class for converting configuration to Playwright launch options.
    /// </summary>
    public static class PlaywrightSettingsHelper
    {
        /// <summary>
        /// Creates BrowserTypeLaunchOptions from configuration.
        /// </summary>
        public static BrowserTypeLaunchOptions CreateLaunchOptions()
        {
            var settings = ConfigManager.Playwright;
            var args = new List<string>();

            if (settings.LaunchArgs != null)
            {
                args.AddRange(settings.LaunchArgs);
            }

            return new BrowserTypeLaunchOptions
            {
                Headless = settings.HeadlessMode,
                Args = args,
                SlowMo = settings.SlowMoDelay
            };
        }

        /// <summary>
        /// Creates ViewPort from configuration.
        /// </summary>
        public static ViewportSize CreateViewport()
        {
            return new ViewportSize
            {
                Width = ConfigManager.Playwright.ViewportWidth,
                Height = ConfigManager.Playwright.ViewportHeight
            };
        }

        /// <summary>
        /// Creates ScreenshotOptions from configuration.
        /// </summary>
        public static PageScreenshotOptions CreateScreenshotOptions(string path)
        {
            return new PageScreenshotOptions
            {
                Path = path,
                FullPage = ConfigManager.Playwright.FullPage
            };
        }

        /// <summary>
        /// Parses browser type string to BrowserManager.BrowserType enum.
        /// </summary>
        public static BrowserType ParseBrowserType(string browserName)
        {
            return browserName?.ToLower() switch
            {
                "firefox" => BrowserType.Firefox,
                "webkit" => BrowserType.WebKit,
                _ => BrowserType.Chromium
            };
        }
    }
}
