using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;

namespace PlaywrightTests.Core.Helpers
{
    /// <summary>
    /// Provides wait helper methods for common Playwright wait scenarios.
    /// Centralizes wait logic with consistent timeout handling and logging.
    /// </summary>
    public class WaitHelpers
    {
        private readonly IPage _page;
        private readonly ILogger _logger;
        private readonly int _defaultTimeoutMs;

        public WaitHelpers(IPage page, ILogger logger, int defaultTimeoutMs = 30000)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultTimeoutMs = defaultTimeoutMs;
        }

        /// <summary>
        /// Waits for an element to exist in the DOM.
        /// </summary>
        public async Task WaitForElementAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for element: {Locator} (timeout: {Timeout}ms)", locator, timeout);
                await _page.Locator(locator).WaitForAsync(new LocatorWaitForOptions { Timeout = timeout });
                _logger.Debug("Element found: {Locator}", locator);
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for element: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Waits for an element to be visible (displayed and not hidden).
        /// </summary>
        public async Task WaitForVisibleAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for visible element: {Locator} (timeout: {Timeout}ms)", locator, timeout);
                await _page.Locator(locator).WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeout
                });
                _logger.Debug("Element visible: {Locator}", locator);
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for visible element: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Waits for an element to be hidden (not visible or removed from DOM).
        /// </summary>
        public async Task WaitForHiddenAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for hidden element: {Locator} (timeout: {Timeout}ms)", locator, timeout);
                await _page.Locator(locator).WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = timeout
                });
                _logger.Debug("Element hidden: {Locator}", locator);
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for hidden element: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Waits for an element to be in a clickable state.
        /// </summary>
        public async Task WaitForClickableAsync(string locator, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for clickable element: {Locator} (timeout: {Timeout}ms)", locator, timeout);
                var locatorElement = _page.Locator(locator);
                await locatorElement.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeout
                });
                _logger.Debug("Element clickable: {Locator}", locator);
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for clickable element: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Waits for navigation to complete.
        /// </summary>
        public async Task WaitForNavigationAsync(int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for navigation (timeout: {Timeout}ms)", timeout);
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeout });
                _logger.Debug("Navigation completed");
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for navigation");
                throw;
            }
        }

        /// <summary>
        /// Waits for page to load.
        /// </summary>
        public async Task WaitForPageLoadAsync(int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for page load (timeout: {Timeout}ms)", timeout);
                await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = timeout });
                _logger.Debug("Page loaded");
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for page load");
                throw;
            }
        }

        /// <summary>
        /// Waits for a function to return true.
        /// </summary>
        public async Task WaitForFunctionAsync(string script, int timeoutMs = 0)
        {
            int timeout = timeoutMs > 0 ? timeoutMs : _defaultTimeoutMs;
            try
            {
                _logger.Information("Waiting for function (timeout: {Timeout}ms)", timeout);
                await _page.WaitForFunctionAsync(script, new PageWaitForFunctionOptions { Timeout = timeout });
                _logger.Debug("Function returned true");
            }
            catch (PlaywrightException ex)
            {
                _logger.Error(ex, "Timeout waiting for function");
                throw;
            }
        }

        /// <summary>
        /// Retries an async action with specified number of attempts.
        /// </summary>
        public async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxAttempts, int delayMs = 1000)
        {
            int attempt = 0;
            while (attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    _logger.Debug("Retry attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                    return await action();
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    _logger.Warning(ex, "Attempt {Attempt} failed, retrying...", attempt);
                    await Task.Delay(delayMs);
                }
            }
            throw new InvalidOperationException($"Action failed after {maxAttempts} attempts");
        }

        /// <summary>
        /// Retries an async action with specified number of attempts.
        /// </summary>
        public async Task RetryAsync(Func<Task> action, int maxAttempts, int delayMs = 1000)
        {
            int attempt = 0;
            while (attempt < maxAttempts)
            {
                try
                {
                    attempt++;
                    _logger.Debug("Retry attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                    await action();
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    _logger.Warning(ex, "Attempt {Attempt} failed, retrying...", attempt);
                    await Task.Delay(delayMs);
                }
            }
            throw new InvalidOperationException($"Action failed after {maxAttempts} attempts");
        }
    }
}
