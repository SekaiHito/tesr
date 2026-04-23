namespace PlaywrightTests.Core.Steps;

using PlaywrightTests.Core.Config;
using PlaywrightTests.Core.Managers;
using PlaywrightTests.Core.Pages;

public class BaseSteps
{
    protected readonly BrowserManager BrowserManager = BrowserManager.Instance;
    protected readonly LoggerManager Logger = LoggerManager.Instance;
    protected readonly ConfigManager Config = ConfigManager.Instance;

    /// <summary>
    /// Navigate to base URL
    /// </summary>
    public virtual async Task GivenUserNavigatesToBaseUrlAsync()
    {
        Logger.Info("Step: Navigate to base URL");
        await BrowserManager.NavigateAsync(Config.GetBaseUrl()).ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false); // Wait for page load
    }

    /// <summary>
    /// Navigate to specific URL
    /// </summary>
    public virtual async Task GivenUserNavigatesToUrlAsync(string url)
    {
        Logger.Info("Step: Navigate to URL - {Url}", url);
        await BrowserManager.NavigateAsync(url).ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Wait for specified seconds
    /// </summary>
    public virtual async Task WaitAsync(int seconds)
    {
        Logger.Info("Step: Wait for {Seconds} seconds", seconds);
        await Task.Delay(seconds * 1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Takes screenshot
    /// </summary>
    public virtual async Task ThenTakeScreenshotAsync(string name)
    {
        Logger.Info("Step: Take screenshot - {Name}", name);
        await BrowserManager.TakeScreenshotAsync(name).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies page title
    /// </summary>
    public virtual async Task ThenPageTitleShouldBeAsync(string expectedTitle)
    {
        Logger.Info("Step: Verify page title is '{Title}'", expectedTitle);
        var actualTitle = await BrowserManager.Page.TitleAsync().ConfigureAwait(false);
        Assert.That(actualTitle, Does.Contain(expectedTitle),
            $"Expected title to contain '{expectedTitle}', but got '{actualTitle}'");
    }

    /// <summary>
    /// Verifies page URL
    /// </summary>
    public virtual async Task ThenPageUrlShouldContainAsync(string expectedUrl)
    {
        Logger.Info("Step: Verify page URL contains '{Url}'", expectedUrl);
        var actualUrl = BrowserManager.Page.Url;
        Assert.That(actualUrl, Does.Contain(expectedUrl),
            $"Expected URL to contain '{expectedUrl}', but got '{actualUrl}'");
    }

    /// <summary>
    /// Refreshes page
    /// </summary>
    public virtual async Task WhenUserRefreshesPageAsync()
    {
        Logger.Info("Step: Refresh page");
        await BrowserManager.Page.ReloadAsync().ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Goes back in history
    /// </summary>
    public virtual async Task WhenUserGoesBackAsync()
    {
        Logger.Info("Step: Go back");
        await BrowserManager.Page.GoBackAsync().ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Goes forward in history
    /// </summary>
    public virtual async Task WhenUserGoesForwardAsync()
    {
        Logger.Info("Step: Go forward");
        await BrowserManager.Page.GoForwardAsync().ConfigureAwait(false);
        await Task.Delay(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes JavaScript
    /// </summary>
    public virtual async Task<object?> ExecuteJavaScriptAsync(string script)
    {
        Logger.Info("Step: Execute JavaScript");
        return await BrowserManager.Page.EvaluateAsync(script).ConfigureAwait(false);
    }

    /// <summary>
    /// Scrolls to element
    /// </summary>
    public virtual async Task ScrollToElementAsync(string locator)
    {
        Logger.Info("Step: Scroll to element - {Locator}", locator);
        await BrowserManager.Page.Locator(locator).ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Hovers over element
    /// </summary>
    public virtual async Task HoverOverElementAsync(string locator)
    {
        Logger.Info("Step: Hover over element - {Locator}", locator);
        await BrowserManager.Page.Locator(locator).HoverAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Double clicks element
    /// </summary>
    public virtual async Task DoubleClickElementAsync(string locator)
    {
        Logger.Info("Step: Double click element - {Locator}", locator);
        await BrowserManager.Page.Locator(locator).DblClickAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Right clicks element
    /// </summary>
    public virtual async Task RightClickElementAsync(string locator)
    {
        Logger.Info("Step: Right click element - {Locator}", locator);
        await BrowserManager.Page.Locator(locator).ClickAsync(new LocatorClickOptions { Button = MouseButton.Right }).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets text from multiple elements
    /// </summary>
    public virtual async Task<List<string>> GetAllTextsAsync(string locator)
    {
        Logger.Info("Step: Get all texts from - {Locator}", locator);
        var count = await BrowserManager.Page.Locator(locator).CountAsync().ConfigureAwait(false);
        var texts = new List<string>();

        for (int i = 0; i < count; i++)
        {
            var text = await BrowserManager.Page.Locator($"({locator})[{i + 1}]").TextContentAsync().ConfigureAwait(false);
            if (text != null)
            {
                texts.Add(text.Trim());
            }
        }

        return texts;
    }

    /// <summary>
    /// Waits for loader/spinner to disappear
    /// </summary>
    public virtual async Task WaitForLoaderToDisappearAsync(string loaderLocator, int timeoutSecond = 30)
    {
        Logger.Info("Step: Wait for loader to disappear");
        var timeout = timeoutSecond * 1000;

        try
        {
            await BrowserManager.Page.Locator(loaderLocator).WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Hidden,
                Timeout = timeout
            }).ConfigureAwait(false);
        }
        catch
        {
            Logger.Warning("Loader did not disappear within {Timeout}ms", timeout);
        }
    }

    /// <summary>
    /// Switches to iframe
    /// </summary>
    public virtual async Task<IFrameLocator> SwitchToIframeAsync(string iframeLocator)
    {
        Logger.Info("Step: Switch to iframe - {Locator}", iframeLocator);
        return BrowserManager.Page.FrameLocator(iframeLocator);
    }

    /// <summary>
    /// Accepts alert dialog
    /// </summary>
    public virtual async Task AcceptAlertDialogAsync()
    {
        Logger.Info("Step: Accept alert dialog");
        await BrowserManager.Page.WaitForEventAsync(PageEvent.Dialog, async () =>
        {
            var dialog = BrowserManager.Page.Context.Pages[0];
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Dismisses alert dialog
    /// </summary>
    public virtual async Task DismissAlertDialogAsync()
    {
        Logger.Info("Step: Dismiss alert dialog");
        await BrowserManager.Page.WaitForEventAsync(PageEvent.Dialog).ConfigureAwait(false);
    }
}
