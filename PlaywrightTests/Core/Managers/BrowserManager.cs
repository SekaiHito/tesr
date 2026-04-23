namespace PlaywrightTests.Core.Managers;

using Microsoft.Playwright;
using PlaywrightTests.Core.Config;

public class BrowserManager
{
    private static BrowserManager? _instance;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private IPlaywright? _playwright;
    private readonly object _lockObject = new();

    public IPage Page => _page ?? throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");
    public IBrowserContext Context => _context ?? throw new InvalidOperationException("Context not initialized. Call InitializeAsync first.");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

    private BrowserManager() { }

    /// <summary>
    /// Gets singleton instance of BrowserManager
    /// </summary>
    public static BrowserManager Instance => _instance ??= new BrowserManager();

    /// <summary>
    /// Initializes Playwright and launches browser
    /// </summary>
    public async Task InitializeAsync()
    {
        lock (_lockObject)
        {
            if (_browser != null || _page != null)
            {
                LoggerManager.Instance.Warning("Browser already initialized");
                return;
            }
        }

        try
        {
            var logger = LoggerManager.Instance;
            var config = ConfigManager.Instance.Framework.Playwright;
            var environmentConfig = ConfigManager.Instance.Current;

            logger.Info("Initializing Playwright...");

            // Install Playwright browsers if needed
            var exitCode = await Microsoft.Playwright.Program.Main(new[] { "install" }).ConfigureAwait(false);
            if (exitCode != 0)
            {
                logger.Warning("Playwright browser installation returned code: {ExitCode}", exitCode);
            }

            _playwright = await Playwright.CreateAsync().ConfigureAwait(false);
            logger.Debug("Playwright instance created");

            // Launch browser
            var browserType = config.BrowserType.ToLower();
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = config.HeadlessMode,
                SlowMo = config.SlowMoDelay > 0 ? config.SlowMoDelay : null,
                Args = config.LaunchArgs.Any() ? config.LaunchArgs : null
            };

            _browser = browserType switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions).ConfigureAwait(false),
                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions).ConfigureAwait(false),
                _ => await _playwright.Chromium.LaunchAsync(launchOptions).ConfigureAwait(false)
            };

            logger.Info("Browser launched: {BrowserType} (Headless: {Headless})", browserType, config.HeadlessMode);

            // Create browser context
            var contextOptions = new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = config.ViewportWidth,
                    Height = config.ViewportHeight
                },
                IgnoreHTTPSErrors = true
            };

            _context = await _browser.NewContextAsync(contextOptions).ConfigureAwait(false);
            logger.Debug("Browser context created with viewport: {Width}x{Height}",
                config.ViewportWidth, config.ViewportHeight);

            // Create page
            _page = await _context.NewPageAsync().ConfigureAwait(false);
            await _page.SetDefaultTimeout(config.Timeout).ConfigureAwait(false);
            await _page.SetDefaultNavigationTimeout(config.Timeout).ConfigureAwait(false);

            logger.Info("Page created with timeout: {Timeout}ms", config.Timeout);

            // Set up event handlers
            _page.Dialog += (_, dialog) =>
            {
                logger.Info("Dialog appeared: {Type} - {Message}", dialog.Type, dialog.Message);
                _ = dialog.AcceptAsync();
            };

            _page.Crash += (_, _) =>
            {
                logger.Error("Page crashed!");
            };

            logger.Info("Browser and page initialized successfully");
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Error(ex, "Failed to initialize browser");
            throw;
        }
    }

    /// <summary>
    /// Navigates to specified URL
    /// </summary>
    public async Task NavigateAsync(string url)
    {
        try
        {
            LoggerManager.Instance.Info("Navigating to: {Url}", url);
            var response = await Page.GotoAsync(url).ConfigureAwait(false);
            LoggerManager.Instance.Debug("Navigation response status: {Status}", response?.Status);
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Error(ex, "Navigation failed to {Url}", url);
            throw;
        }
    }

    /// <summary>
    /// Takes screenshot and saves to file
    /// </summary>
    public async Task<string> TakeScreenshotAsync(string screenshotName)
    {
        try
        {
            var screenshotsPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            if (!Directory.Exists(screenshotsPath))
            {
                Directory.CreateDirectory(screenshotsPath);
            }

            var fileName = $"{screenshotName}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}.png";
            var filePath = Path.Combine(screenshotsPath, fileName);

            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = filePath }).ConfigureAwait(false);
            LoggerManager.Instance.Info("Screenshot saved: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Error(ex, "Failed to take screenshot");
            return string.Empty;
        }
    }

    /// <summary>
    /// Closes page and browser
    /// </summary>
    public async Task CloseAsync()
    {
        try
        {
            if (_page != null)
            {
                await _page.CloseAsync().ConfigureAwait(false);
                LoggerManager.Instance.Debug("Page closed");
            }

            if (_context != null)
            {
                await _context.CloseAsync().ConfigureAwait(false);
                LoggerManager.Instance.Debug("Context closed");
            }

            if (_browser != null)
            {
                await _browser.CloseAsync().ConfigureAwait(false);
                LoggerManager.Instance.Info("Browser closed");
            }

            _playwright?.Dispose();
            LoggerManager.Instance.Debug("Playwright disposed");
        }
        catch (Exception ex)
        {
            LoggerManager.Instance.Error(ex, "Error during browser cleanup");
        }
        finally
        {
            _page = null;
            _context = null;
            _browser = null;
            _playwright = null;
        }
    }

    /// <summary>
    /// Resets singleton instance
    /// </summary>
    public static void Reset()
    {
        _instance = null;
    }
}
