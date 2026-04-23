namespace PlaywrightTests.Core.Pages;

public class LoginPage : BasePage
{
    // Locators
    private const string LoginModalSelector = "[id='logInModal']";
    private const string UsernameInputSelector = "#loginusername";
    private const string PasswordInputSelector = "#loginpassword";
    private const string LoginButtonSelector = "button:has-text('Log in')";
    private const string WelcomeMessageSelector = "#nameofuser";
    private const string CloseButtonSelector = "[data-bs-dismiss='modal']";

    // Page URL
    private const string PageUrl = "/index.html";

    /// <summary>
    /// Opens login modal
    /// </summary>
    public async Task OpenLoginModalAsync()
    {
        Logger.Info("Opening login modal");
        const string loginLinkSelector = "a:has-text('Log in')";
        await ClickAsync(loginLinkSelector).ConfigureAwait(false);
        await WaitForVisibleAsync(LoginModalSelector).ConfigureAwait(false);
        Logger.Info("Login modal opened");
    }

    /// <summary>
    /// Enters username
    /// </summary>
    public async Task EnterUsernameAsync(string username)
    {
        Logger.Info("Entering username");
        await FillAsync(UsernameInputSelector, username).ConfigureAwait(false);
    }

    /// <summary>
    /// Enters password
    /// </summary>
    public async Task EnterPasswordAsync(string password)
    {
        Logger.Info("Entering password");
        await FillAsync(PasswordInputSelector, password).ConfigureAwait(false);
    }

    /// <summary>
    /// Clicks login button
    /// </summary>
    public async Task ClickLoginButtonAsync()
    {
        Logger.Info("Clicking login button");
        await ClickAsync(LoginButtonSelector).ConfigureAwait(false);
        await Task.Delay(2000).ConfigureAwait(false); // Wait for login to process
    }

    /// <summary>
    /// Performs complete login
    /// </summary>
    public async Task LoginAsync(string username, string password)
    {
        Logger.Info("Performing login with username: {Username}", username);
        await OpenLoginModalAsync().ConfigureAwait(false);
        await EnterUsernameAsync(username).ConfigureAwait(false);
        await EnterPasswordAsync(password).ConfigureAwait(false);
        await ClickLoginButtonAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets welcome message
    /// </summary>
    public async Task<string> GetWelcomeMessageAsync()
    {
        Logger.Info("Getting welcome message");
        return await GetTextAsync(WelcomeMessageSelector).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks if login modal is visible
    /// </summary>
    public async Task<bool> IsLoginModalVisibleAsync()
    {
        return await IsVisibleAsync(LoginModalSelector).ConfigureAwait(false);
    }

    /// <summary>
    /// Closes login modal
    /// </summary>
    public async Task CloseLoginModalAsync()
    {
        Logger.Info("Closing login modal");
        await ClickAsync(CloseButtonSelector).ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to login page
    /// </summary>
    public override async Task NavigateAsync(string url = "")
    {
        var fullUrl = string.IsNullOrEmpty(url) ?
            $"{BrowserManager.Page.Url}{PageUrl}" : url;

        Logger.Info("Navigating to login page: {Url}", fullUrl);
        await BrowserManager.NavigateAsync(fullUrl).ConfigureAwait(false);
    }
}
