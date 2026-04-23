namespace PlaywrightTests.Core.Helpers;

using Microsoft.Playwright;
using PlaywrightTests.Core.Managers;

public class WaitHelpers
{
    private readonly LoggerManager _logger = LoggerManager.Instance;
    private readonly IPage _page;

    public WaitHelpers(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Waits for element with retry logic
    /// </summary>
    public async Task<bool> WaitForElementWithRetryAsync(string locator, int maxRetries = 3, int delayMs = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _logger.Debug("Attempt {Attempt}/{MaxRetries} to find element: {Locator}", i + 1, maxRetries, locator);
                await _page.Locator(locator).WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                }).ConfigureAwait(false);
                return true;
            }
            catch
            {
                if (i < maxRetries - 1)
                {
                    _logger.Debug("Retry waiting for element in {DelayMs}ms", delayMs);
                    await Task.Delay(delayMs).ConfigureAwait(false);
                }
            }
        }

        _logger.Error("Element not found after {MaxRetries} retries: {Locator}", maxRetries, locator);
        return false;
    }

    /// <summary>
    /// Waits for specific text to appear
    /// </summary>
    public async Task WaitForTextAsync(string locator, string expectedText, int timeoutMs = 30000)
    {
        _logger.Debug("Waiting for text '{Text}' in: {Locator}", expectedText, locator);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < timeoutMs)
        {
            try
            {
                var text = await _page.Locator(locator).TextContentAsync().ConfigureAwait(false);
                if (text?.Contains(expectedText) == true)
                {
                    _logger.Debug("Found expected text: {Text}", expectedText);
                    return;
                }
            }
            catch { }

            await Task.Delay(500).ConfigureAwait(false);
        }

        throw new TimeoutException($"Text '{expectedText}' not found in {locator} within {timeoutMs}ms");
    }

    /// <summary>
    /// Waits for page to load completely
    /// </summary>
    public async Task WaitForPageLoadAsync()
    {
        _logger.Debug("Waiting for page to load");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for element count to match expected
    /// </summary>
    public async Task WaitForElementCountAsync(string locator, int expectedCount, int timeoutMs = 30000)
    {
        _logger.Debug("Waiting for element count {Count} in: {Locator}", expectedCount, locator);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < timeoutMs)
        {
            var count = await _page.Locator(locator).CountAsync().ConfigureAwait(false);
            if (count == expectedCount)
            {
                _logger.Debug("Found expected element count: {Count}", expectedCount);
                return;
            }

            await Task.Delay(500).ConfigureAwait(false);
        }

        throw new TimeoutException($"Expected {expectedCount} elements in {locator}, but timeout after {timeoutMs}ms");
    }

    /// <summary>
    /// Waits for element to be enabled
    /// </summary>
    public async Task WaitForElementEnabledAsync(string locator, int timeoutMs = 30000)
    {
        _logger.Debug("Waiting for element to be enabled: {Locator}", locator);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < timeoutMs)
        {
            var isEnabled = await _page.Locator(locator).IsEnabledAsync().ConfigureAwait(false);
            if (isEnabled)
            {
                _logger.Debug("Element is enabled: {Locator}", locator);
                return;
            }

            await Task.Delay(500).ConfigureAwait(false);
        }

        throw new TimeoutException($"Element {locator} did not become enabled within {timeoutMs}ms");
    }
}

public class ScreenshotHelpers
{
    private readonly LoggerManager _logger = LoggerManager.Instance;
    private readonly BrowserManager _browserManager = BrowserManager.Instance;

    /// <summary>
    /// Takes full page screenshot
    /// </summary>
    public async Task<string> TakeFullPageScreenshotAsync(string name)
    {
        _logger.Info("Taking full page screenshot: {Name}", name);
        var screenshotsPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
        if (!Directory.Exists(screenshotsPath))
        {
            Directory.CreateDirectory(screenshotsPath);
        }

        var fileName = $"{name}_fullpage_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}.png";
        var filePath = Path.Combine(screenshotsPath, fileName);

        await _browserManager.Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = filePath,
            FullPage = true
        }).ConfigureAwait(false);

        return filePath;
    }

    /// <summary>
    /// Takes screenshot of specific element
    /// </summary>
    public async Task<string> TakeElementScreenshotAsync(string locator, string name)
    {
        _logger.Info("Taking element screenshot: {Name}", name);
        var screenshotsPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
        if (!Directory.Exists(screenshotsPath))
        {
            Directory.CreateDirectory(screenshotsPath);
        }

        var fileName = $"{name}_element_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}.png";
        var filePath = Path.Combine(screenshotsPath, fileName);

        await _browserManager.Page.Locator(locator).ScreenshotAsync(new LocatorScreenshotOptions
        {
            Path = filePath
        }).ConfigureAwait(false);

        return filePath;
    }

    /// <summary>
    /// Takes screenshot and compares with baseline
    /// </summary>
    public async Task<bool> CompareScreenshotAsync(string locator, string baselineName)
    {
        _logger.Info("Comparing screenshot with baseline: {Baseline}", baselineName);
        try
        {
            var baselineScreenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots", "Baselines", $"{baselineName}.png");
            if (!File.Exists(baselineScreenshotPath))
            {
                _logger.Warning("Baseline screenshot not found: {Path}", baselineScreenshotPath);
                return false;
            }

            // Compare logic - this is a simplified version
            var currentScreenshot = await _browserManager.Page.Locator(locator).ScreenshotAsync().ConfigureAwait(false);
            var baselineScreenshot = await File.ReadAllBytesAsync(baselineScreenshotPath).ConfigureAwait(false);

            return currentScreenshot.SequenceEqual(baselineScreenshot);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error comparing screenshots");
            return false;
        }
    }
}

public class DataHelpers
{
    private readonly LoggerManager _logger = LoggerManager.Instance;

    /// <summary>
    /// Generates random string
    /// </summary>
    public string GenerateRandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
        _logger.Debug("Generated random string: {Length} characters", length);
        return result;
    }

    /// <summary>
    /// Generates random email
    /// </summary>
    public string GenerateRandomEmail()
    {
        var timestamp = DateTime.UtcNow.Ticks;
        var email = $"test.{timestamp}@example.com";
        _logger.Debug("Generated random email: {Email}", email);
        return email;
    }

    /// <summary>
    /// Generates random phone number
    /// </summary>
    public string GenerateRandomPhoneNumber()
    {
        var random = new Random();
        var number = $"+1{random.Next(100, 999)}{random.Next(100, 999)}{random.Next(1000, 9999)}";
        _logger.Debug("Generated random phone number: {Number}", number);
        return number;
    }

    /// <summary>
    /// Gets test data from configuration
    /// </summary>
    public Dictionary<string, string> GetTestData(string key)
    {
        _logger.Debug("Retrieving test data for key: {Key}", key);
        // This would typically load from a JSON or CSV file
        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates URL format
    /// </summary>
    public bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
