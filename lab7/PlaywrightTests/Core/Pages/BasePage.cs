using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Helpers;
using PlaywrightTests.Config;

namespace PlaywrightTests.Core.Pages
{
    /// <summary>
    /// Base class for all page objects using Page Object Model pattern.
    /// Provides common methods for element interaction with wait, retry, and logging.
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IPage _page;
        protected readonly ILogger _logger;
        protected readonly WaitHelpers _waitHelpers;
        protected readonly int _defaultTimeout;

        protected BasePage(IPage page, ILogger logger)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultTimeout = ConfigManager.Playwright.Timeout;
            _waitHelpers = new WaitHelpers(page, logger, _defaultTimeout);
        }

        /// <summary>
        /// Clicks an element with wait and logging.
        /// </summary>
        public virtual async Task ClickAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Information("Clicking element: {Locator}", locator);
                await _waitHelpers.WaitForClickableAsync(locator, timeout);
                await _page.Locator(locator).ClickAsync();
                _logger.Debug("Element clicked successfully: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to click element: {Locator}", locator);
                await TakeScreenshotOnErrorAsync("click_error");
                throw;
            }
        }

        /// <summary>
        /// Fills a text field with error handling.
        /// </summary>
        public virtual async Task FillAsync(string locator, string text, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Information("Filling text in: {Locator}", locator);
                await _waitHelpers.WaitForVisibleAsync(locator, timeout);
                await _page.Locator(locator).ClearAsync();
                await _page.Locator(locator).FillAsync(text);
                _logger.Debug("Text filled successfully in: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fill text in: {Locator}", locator);
                await TakeScreenshotOnErrorAsync("fill_error");
                throw;
            }
        }

        /// <summary>
        /// Types text character by character with delay.
        /// </summary>
        public virtual async Task TypeAsync(string locator, string text, int delayMs = 50, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Information("Typing text in: {Locator}", locator);
                await _waitHelpers.WaitForVisibleAsync(locator, timeout);
                await _page.Locator(locator).TypeAsync(text, new LocatorTypeOptions { Delay = delayMs });
                _logger.Debug("Text typed successfully in: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to type text in: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Navigates to a URL.
        /// </summary>
        public virtual async Task NavigateAsync(string url, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Information("Navigating to: {Url}", url);
                await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = timeout });
                _logger.Information("Navigation completed to: {Url}", url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to navigate to: {Url}", url);
                throw;
            }
        }

        /// <summary>
        /// Gets text from an element.
        /// </summary>
        public virtual async Task<string> GetTextAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Debug("Getting text from: {Locator}", locator);
                await _waitHelpers.WaitForVisibleAsync(locator, timeout);
                string text = await _page.Locator(locator).TextContentAsync();
                _logger.Debug("Text retrieved: {Text}", text);
                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get text from: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Gets attribute value from an element.
        /// </summary>
        public virtual async Task<string> GetAttributeAsync(string locator, string attributeName, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeout;
            try
            {
                _logger.Debug("Getting attribute {Attribute} from: {Locator}", attributeName, locator);
                await _waitHelpers.WaitForElementAsync(locator, timeout);
                string value = await _page.Locator(locator).GetAttributeAsync(attributeName);
                _logger.Debug("Attribute {Attribute} value: {Value}", attributeName, value);
                return value ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get attribute {Attribute} from: {Locator}", attributeName, locator);
                throw;
            }
        }

        /// <summary>
        /// Checks if an element is visible.
        /// </summary>
        public virtual async Task<bool> IsVisibleAsync(string locator)
        {
            try
            {
                bool isVisible = await _page.Locator(locator).IsVisibleAsync();
                _logger.Debug("Element visibility check: {Locator} = {IsVisible}", locator, isVisible);
                return isVisible;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to check visibility of: {Locator}", locator);
                return false;
            }
        }

        /// <summary>
        /// Waits for an element to be visible with fluent API.
        /// </summary>
        public virtual async Task WaitForElementVisibleAsync(string locator, int timeoutMs = 0)
        {
            await _waitHelpers.WaitForVisibleAsync(locator, timeoutMs);
        }

        /// <summary>
        /// Takes a screenshot on error for debugging.
        /// </summary>
        protected virtual async Task TakeScreenshotOnErrorAsync(string screenshotName = "error_screenshot")
        {
            try
            {
                if (!ConfigManager.TestData.Screenshots)
                    return;

                string directory = "Screenshots";
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                string fileName = $"{directory}/{screenshotName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = fileName, FullPage = true });
                _logger.Warning("Screenshot saved: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to take screenshot");
            }
        }

        /// <summary>
        /// Executes JavaScript in page context.
        /// </summary>
        public virtual async Task<object> ExecuteScriptAsync(string script, object args = null)
        {
            try
            {
                _logger.Debug("Executing script");
                return await _page.EvaluateAsync(script, args);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to execute script");
                throw;
            }
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        public virtual async Task<string> GetPageTitleAsync()
        {
            string title = _page.Url;
            _logger.Debug("Current page title/URL: {Title}", title);
            return title;
        }

        /// <summary>
        /// Waits for page load state.
        /// </summary>
        public virtual async Task WaitForPageLoadAsync()
        {
            await _waitHelpers.WaitForPageLoadAsync();
        }

        /// <summary>
        /// Performs an action with retry.
        /// </summary>
        public virtual async Task RetryActionAsync(Func<Task> action, int maxAttempts = 3, int delayMs = 1000)
        {
            await _waitHelpers.RetryAsync(action, maxAttempts, delayMs);
        }
    }
}
