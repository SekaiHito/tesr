using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;

namespace PlaywrightTests.Core.Managers
{
    /// <summary>
    /// Manages Playwright browser instances and page contexts for tests.
    /// Handles browser lifecycle, page management, and context isolation.
    /// </summary>
    public class BrowserManager : IAsyncDisposable
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private readonly ILogger _logger;
        private readonly BrowserTypeLaunchOptions _launchOptions;
        private readonly Dictionary<string, object> _contextData;

        public BrowserManager(ILogger logger, BrowserTypeLaunchOptions launchOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _launchOptions = launchOptions ?? throw new ArgumentNullException(nameof(launchOptions));
            _contextData = new Dictionary<string, object>();
        }

        public IPage Page
        {
            get => _page ?? throw new InvalidOperationException("Browser is not initialized. Call InitializeBrowserAsync first.");
            private set => _page = value;
        }

        public IBrowserContext Context
        {
            get => _context ?? throw new InvalidOperationException("Browser context is not initialized.");
            private set => _context = value;
        }

        /// <summary>
        /// Initializes the Playwright browser instance and page.
        /// </summary>
        public async Task InitializeBrowserAsync(BrowserType browserType = BrowserType.Chromium)
        {
            try
            {
                _logger.Information("Initializing Playwright browser: {BrowserType}", browserType);

                _playwright = await Playwright.CreateAsync();

                _browser = browserType switch
                {
                    BrowserType.Firefox => await _playwright.Firefox.LaunchAsync(_launchOptions),
                    BrowserType.WebKit => await _playwright.Webkit.LaunchAsync(_launchOptions),
                    _ => await _playwright.Chromium.LaunchAsync(_launchOptions)
                };

                _context = await _browser.NewContextAsync();
                Page = await _context.NewPageAsync();

                _logger.Information("Browser initialized successfully. Headless: {Headless}", _launchOptions.Headless);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to initialize browser");
                throw;
            }
        }

        /// <summary>
        /// Creates a new page in the current context.
        /// </summary>
        public async Task<IPage> CreateNewPageAsync()
        {
            _logger.Information("Creating new page");
            return await _context.NewPageAsync();
        }

        /// <summary>
        /// Takes a screenshot and saves it to the specified path.
        /// </summary>
        public async Task TakeScreenshotAsync(string path)
        {
            try
            {
                _logger.Information("Taking screenshot: {Path}", path);
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to take screenshot");
            }
        }

        /// <summary>
        /// Stores context data for test execution.
        /// </summary>
        public void SetContextData(string key, object value)
        {
            _contextData[key] = value;
            _logger.Debug("Context data set: {Key}", key);
        }

        /// <summary>
        /// Retrieves context data.
        /// </summary>
        public object GetContextData(string key)
        {
            return _contextData.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Closes all pages and contexts gracefully.
        /// </summary>
        public async Task CloseBrowserAsync()
        {
            try
            {
                _logger.Information("Closing browser");
                if (_page != null)
                    await _page.CloseAsync();

                if (_context != null)
                    await _context.CloseAsync();

                if (_browser != null)
                    await _browser.CloseAsync();

                _playwright?.Dispose();
                _logger.Information("Browser closed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error closing browser");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseBrowserAsync();
        }
    }

    public enum BrowserType
    {
        Chromium,
        Firefox,
        WebKit
    }
}
