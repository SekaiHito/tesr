using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Helpers;
using PlaywrightTests.Config;

namespace PlaywrightTests.Core.Steps
{
    /// <summary>
    /// Base class for test steps using BDD pattern.
    /// Provides common step methods following Given-When-Then structure.
    /// </summary>
    public abstract class BaseSteps
    {
        protected readonly IPage _page;
        protected readonly ILogger _logger;
        protected readonly WaitHelpers _waitHelpers;
        protected readonly DataHelpers _dataHelpers;
        protected readonly int _defaultTimeout;

        protected BaseSteps(IPage page, ILogger logger)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultTimeout = ConfigManager.Playwright.Timeout;
            _waitHelpers = new WaitHelpers(page, logger, _defaultTimeout);
            _dataHelpers = new DataHelpers(logger);
        }

        /// <summary>
        /// Given: Navigates to a base URL
        /// </summary>
        public virtual async Task GivenUserNavigatesToAsync(string url)
        {
            _logger.Information("Given: User navigates to {Url}", url);
            try
            {
                await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
                _logger.Information("Navigation successful to: {Url}", url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Navigation failed to: {Url}", url);
                throw;
            }
        }

        /// <summary>
        /// Given: Waits for element to be visible
        /// </summary>
        public virtual async Task GivenElementIsVisibleAsync(string locator)
        {
            _logger.Information("Given: Element is visible {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForVisibleAsync(locator, _defaultTimeout);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Element not visible: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// When: Clicks an element
        /// </summary>
        public virtual async Task WhenUserClicksAsync(string locator)
        {
            _logger.Information("When: User clicks element {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForClickableAsync(locator, _defaultTimeout);
                await _page.Locator(locator).ClickAsync();
                _logger.Information("Element clicked: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to click element: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// When: Enters text in a field
        /// </summary>
        public virtual async Task WhenUserEntersTextAsync(string locator, string text)
        {
            _logger.Information("When: User enters text in {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForVisibleAsync(locator, _defaultTimeout);
                await _page.Locator(locator).FillAsync(text);
                _logger.Information("Text entered in: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to enter text in: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// When: Submits a form
        /// </summary>
        public virtual async Task WhenUserSubmitsFormAsync(string formLocator)
        {
            _logger.Information("When: User submits form {FormLocator}", formLocator);
            try
            {
                await _page.Locator(formLocator).EvaluateAsync("form => form.submit()");
                await Task.Delay(500); // Allow page to process submission
                _logger.Information("Form submitted: {FormLocator}", formLocator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to submit form: {FormLocator}", formLocator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies element is present
        /// </summary>
        public virtual async Task ThenElementShouldBePresentAsync(string locator)
        {
            _logger.Information("Then: Element should be present {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForElementAsync(locator, _defaultTimeout);
                _logger.Information("Element is present: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Element not found: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies element is visible
        /// </summary>
        public virtual async Task ThenElementShouldBeVisibleAsync(string locator)
        {
            _logger.Information("Then: Element should be visible {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForVisibleAsync(locator, _defaultTimeout);
                _logger.Information("Element is visible: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Element is not visible: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies element is hidden
        /// </summary>
        public virtual async Task ThenElementShouldBeHiddenAsync(string locator)
        {
            _logger.Information("Then: Element should be hidden {Locator}", locator);
            try
            {
                await _waitHelpers.WaitForHiddenAsync(locator, _defaultTimeout);
                _logger.Information("Element is hidden: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Element is visible but should be hidden: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies text content
        /// </summary>
        public virtual async Task ThenElementTextShouldContainAsync(string locator, string expectedText)
        {
            _logger.Information("Then: Element {Locator} text should contain {ExpectedText}", locator, expectedText);
            try
            {
                string actualText = await _page.Locator(locator).TextContentAsync();
                if (!actualText.Contains(expectedText))
                {
                    throw new AssertionException($"Expected text '{expectedText}' not found in '{actualText}'");
                }
                _logger.Information("Text verification passed: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Text verification failed: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies exact text content
        /// </summary>
        public virtual async Task ThenElementTextShouldBeAsync(string locator, string expectedText)
        {
            _logger.Information("Then: Element {Locator} text should be {ExpectedText}", locator, expectedText);
            try
            {
                string actualText = await _page.Locator(locator).TextContentAsync();
                if (actualText?.Trim() != expectedText?.Trim())
                {
                    throw new AssertionException($"Expected text '{expectedText}' but got '{actualText}'");
                }
                _logger.Information("Text verification passed: {Locator}", locator);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Text verification failed: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Then: Verifies URL contains text
        /// </summary>
        public virtual async Task ThenUrlShouldContainAsync(string expectedUrl)
        {
            _logger.Information("Then: URL should contain {ExpectedUrl}", expectedUrl);
            try
            {
                string currentUrl = _page.Url;
                if (!currentUrl.Contains(expectedUrl))
                {
                    throw new AssertionException($"Expected URL to contain '{expectedUrl}' but got '{currentUrl}'");
                }
                _logger.Information("URL verification passed: {CurrentUrl}", currentUrl);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "URL verification failed");
                throw;
            }
        }

        /// <summary>
        /// When: Waits for navigation
        /// </summary>
        public virtual async Task WhenUserWaitsForNavigationAsync()
        {
            _logger.Information("When: User waits for navigation");
            try
            {
                await _waitHelpers.WaitForNavigationAsync(_defaultTimeout);
                _logger.Information("Navigation completed");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Navigation wait failed");
                throw;
            }
        }

        /// <summary>
        /// Custom step: Performs action with retry
        /// </summary>
        public virtual async Task PerformActionWithRetryAsync(Func<Task> action, int maxAttempts = 3)
        {
            _logger.Information("Performing action with retry: {MaxAttempts} attempts", maxAttempts);
            await _waitHelpers.RetryAsync(action, maxAttempts);
        }
    }

    /// <summary>
    /// Custom assertion exception for step failures.
    /// </summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
