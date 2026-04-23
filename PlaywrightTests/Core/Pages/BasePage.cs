namespace PlaywrightTests.Core.Pages;

using Microsoft.Playwright;
using PlaywrightTests.Core.Managers;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly LoggerManager Logger = LoggerManager.Instance;
    protected readonly BrowserManager BrowserManager = BrowserManager.Instance;

    protected BasePage()
    {
        Page = BrowserManager.Page;
    }

    /// <summary>
    /// Navigates to specific URL
    /// </summary>
    public virtual async Task NavigateAsync(string url)
    {
        Logger.Info("Navigating to page: {Url}", url);
        await BrowserManager.NavigateAsync(url).ConfigureAwait(false);
    }

    /// <summary>
    /// Clicks element by locator
    /// </summary>
    public virtual async Task ClickAsync(string locator, int timeout = 30000)
    {
        try
        {
            Logger.Debug("Clicking element: {Locator}", locator);
            await Page.Locator(locator).ClickAsync(new LocatorClickOptions { Timeout = timeout }).ConfigureAwait(false);
            Logger.Debug("Successfully clicked: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to click element: {Locator}", locator);
            await TakeFailureScreenshot("ClickFailure");
            throw;
        }
    }

    /// <summary>
    /// Fills text input
    /// </summary>
    public virtual async Task FillAsync(string locator, string text, int timeout = 30000)
    {
        try
        {
            Logger.Debug("Filling text in: {Locator} with text: {Text}", locator, ObfuscatePassword(text));
            await Page.Locator(locator).FillAsync(text, new LocatorFillOptions { Timeout = timeout }).ConfigureAwait(false);
            Logger.Debug("Successfully filled: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to fill text in: {Locator}", locator);
            await TakeFailureScreenshot("FillFailure");
            throw;
        }
    }

    /// <summary>
    /// Types text character by character
    /// </summary>
    public virtual async Task TypeAsync(string locator, string text, int delayMs = 50)
    {
        try
        {
            Logger.Debug("Typing text in: {Locator}", locator);
            await Page.Locator(locator).TypeAsync(text, new LocatorTypeOptions { Delay = delayMs }).ConfigureAwait(false);
            Logger.Debug("Successfully typed in: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to type in: {Locator}", locator);
            await TakeFailureScreenshot("TypeFailure");
            throw;
        }
    }

    /// <summary>
    /// Gets element text
    /// </summary>
    public virtual async Task<string> GetTextAsync(string locator, int timeout = 30000)
    {
        try
        {
            Logger.Debug("Getting text from: {Locator}", locator);
            var text = await Page.Locator(locator).TextContentAsync(new LocatorTextContentOptions { Timeout = timeout }).ConfigureAwait(false);
            Logger.Debug("Retrieved text: {Text}", text);
            return text ?? string.Empty;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to get text from: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Waits for element to be visible
    /// </summary>
    public virtual async Task WaitForVisibleAsync(string locator, int timeout = 30000)
    {
        try
        {
            Logger.Debug("Waiting for element to be visible: {Locator}", locator);
            await Page.Locator(locator).WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            }).ConfigureAwait(false);
            Logger.Debug("Element is visible: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Timeout waiting for element to be visible: {Locator}", locator);
            await TakeFailureScreenshot("WaitForVisibleFailure");
            throw;
        }
    }

    /// <summary>
    /// Waits for element to be hidden
    /// </summary>
    public virtual async Task WaitForHiddenAsync(string locator, int timeout = 30000)
    {
        try
        {
            Logger.Debug("Waiting for element to be hidden: {Locator}", locator);
            await Page.Locator(locator).WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Hidden,
                Timeout = timeout
            }).ConfigureAwait(false);
            Logger.Debug("Element is hidden: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Timeout waiting for element to be hidden: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Checks if element is visible
    /// </summary>
    public virtual async Task<bool> IsVisibleAsync(string locator)
    {
        try
        {
            var isVisible = await Page.Locator(locator).IsVisibleAsync().ConfigureAwait(false);
            Logger.Debug("Element visibility check - {Locator}: {IsVisible}", locator, isVisible);
            return isVisible;
        }
        catch (Exception ex)
        {
            Logger.Debug("Error checking visibility: {Locator}", locator);
            return false;
        }
    }

    /// <summary>
    /// Checks if element exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(string locator)
    {
        try
        {
            var count = await Page.Locator(locator).CountAsync().ConfigureAwait(false);
            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets element count
    /// </summary>
    public virtual async Task<int> GetCountAsync(string locator)
    {
        return await Page.Locator(locator).CountAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Selects option from dropdown
    /// </summary>
    public virtual async Task SelectOptionAsync(string locator, string optionValue)
    {
        try
        {
            Logger.Debug("Selecting option: {Option} from: {Locator}", optionValue, locator);
            await Page.Locator(locator).SelectOptionAsync(optionValue).ConfigureAwait(false);
            Logger.Debug("Successfully selected option: {Option}", optionValue);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to select option: {Option} from: {Locator}", optionValue, locator);
            throw;
        }
    }

    /// <summary>
    /// Checks a checkbox
    /// </summary>
    public virtual async Task CheckAsync(string locator)
    {
        try
        {
            Logger.Debug("Checking checkbox: {Locator}", locator);
            await Page.Locator(locator).CheckAsync(new LocatorCheckOptions { Force = true }).ConfigureAwait(false);
            Logger.Debug("Successfully checked: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to check: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Unchecks a checkbox
    /// </summary>
    public virtual async Task UncheckAsync(string locator)
    {
        try
        {
            Logger.Debug("Unchecking checkbox: {Locator}", locator);
            await Page.Locator(locator).UncheckAsync(new LocatorUncheckOptions { Force = true }).ConfigureAwait(false);
            Logger.Debug("Successfully unchecked: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to uncheck: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Presses keyboard key
    /// </summary>
    public virtual async Task PressAsync(string locator, string key)
    {
        try
        {
            Logger.Debug("Pressing key: {Key} on: {Locator}", key, locator);
            await Page.Locator(locator).PressAsync(key).ConfigureAwait(false);
            Logger.Debug("Successfully pressed key: {Key}", key);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to press key: {Key}", key);
            throw;
        }
    }

    /// <summary>
    /// Waits for navigation to complete
    /// </summary>
    public virtual async Task WaitForNavigationAsync(Func<Task> action)
    {
        try
        {
            Logger.Debug("Waiting for navigation...");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
            await action().ConfigureAwait(false);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);
            Logger.Debug("Navigation completed");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Navigation wait failed");
            throw;
        }
    }

    /// <summary>
    /// Takes screenshot on failure
    /// </summary>
    protected virtual async Task TakeFailureScreenshot(string name)
    {
        try
        {
            if (ConfigManager.Instance.Framework.TestData.ScreenshotsOnFailure)
            {
                await BrowserManager.TakeScreenshotAsync($"{name}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}").ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to take failure screenshot");
        }
    }

    /// <summary>
    /// Obfuscates password for logging
    /// </summary>
    protected static string ObfuscatePassword(string text)
    {
        return text?.Length > 2 ? $"***{text[^1]}" : "***";
    }
}
