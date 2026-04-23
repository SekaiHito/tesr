namespace PlaywrightTests.Tests.UI;

using Allure.Net.Commons;
using PlaywrightTests.Core.Pages;
using PlaywrightTests.Tests;

[TestFixture]
[Category("Smoke")]
[Category("UI")]
[AllureFeature("Authentication")]
public class LoginTests : BaseTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public override async Task TestSetUpAsync()
    {
        await base.TestSetUpAsync().ConfigureAwait(false);
        _loginPage = new LoginPage();
    }

    [Test]
    [Category("Smoke")]
    [Retry(1)]
    [AllureSeverity(SeverityLevel.Blocker)]
    [AllureStory("User login with valid credentials")]
    [AllureLink("https://example.com/test-case-001")]
    public async Task UserLogin_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        AddAllureDescription("This test validates that a user can successfully login with valid credentials");
        AddAllureSeverity(SeverityLevel.Blocker);
        Logger.Info("Test: User Login with Valid Credentials");

        var baseUrl = Config.GetBaseUrl();
        var username = Config.Current.Credentials.Username;
        var password = Config.Current.Credentials.Password;

        // Act
        Logger.Info("Navigating to base URL: {BaseUrl}", baseUrl);
        await _loginPage.NavigateAsync(baseUrl).ConfigureAwait(false);

        Logger.Info("Performing login");
        await _loginPage.LoginAsync(username, password).ConfigureAwait(false);

        // Assert
        Logger.Info("Verifying welcome message");
        var welcomeMessage = await _loginPage.GetWelcomeMessageAsync().ConfigureAwait(false);
        Assert.That(welcomeMessage, Does.Contain(username),
            $"Expected welcome message to contain '{username}', but got '{welcomeMessage}'");

        Logger.Info("Test passed - User successfully logged in");
        await _loginPage.TakeFailureScreenshot("LoginSuccess");
    }

    [Test]
    [Category("UI")]
    [AllureSeverity(SeverityLevel.Normal)]
    [AllureStory("Login modal opens on click")]
    public async Task LoginModal_OnLoginLinkClick_ShouldOpen()
    {
        // Arrange
        AddAllureDescription("This test validates that the login modal opens when clicking the login link");
        Logger.Info("Test: Login Modal Opens");

        var baseUrl = Config.GetBaseUrl();

        // Act
        Logger.Info("Navigating to base URL");
        await _loginPage.NavigateAsync(baseUrl).ConfigureAwait(false);

        Logger.Info("Clicking login link");
        const string loginLinkSelector = "a:has-text('Log in')";
        await _loginPage.ClickAsync(loginLinkSelector).ConfigureAwait(false);

        // Assert
        Logger.Info("Verifying login modal is visible");
        var isModalVisible = await _loginPage.IsLoginModalVisibleAsync().ConfigureAwait(false);
        Assert.That(isModalVisible, Is.True, "Login modal should be visible");

        Logger.Info("Test passed - Login modal opened successfully");
    }

    [Test]
    [Category("UI")]
    [AllureSeverity(SeverityLevel.Normal)]
    [AllureStory("Page navigation")]
    public async Task PageNavigation_ToBaseUrl_ShouldLoadSuccessfully()
    {
        // Arrange
        AddAllureDescription("This test validates that the page loads successfully");
        Logger.Info("Test: Page Navigation");

        var baseUrl = Config.GetBaseUrl();

        // Act
        Logger.Info("Navigating to: {BaseUrl}", baseUrl);
        await _loginPage.NavigateAsync(baseUrl).ConfigureAwait(false);

        // Assert
        Logger.Info("Verifying page title");
        var pageTitle = await BrowserManager.Page.TitleAsync().ConfigureAwait(false);
        Assert.That(pageTitle, Is.Not.Empty, "Page title should not be empty");

        Logger.Info("Page title: {PageTitle}", pageTitle);
        Logger.Info("Test passed - Page loaded successfully");
    }

    [Test]
    [Category("Smoke")]
    [AllureSeverity(SeverityLevel.Critical)]
    [AllureStory("Browser initialization")]
    public async Task Browser_AfterInitialization_ShouldBeReady()
    {
        // Arrange
        AddAllureDescription("This test validates that the browser is properly initialized");
        Logger.Info("Test: Browser Initialization");

        // Act & Assert
        Assert.That(BrowserManager.Page, Is.Not.Null, "Browser page should be initialized");
        Assert.That(BrowserManager.Browser, Is.Not.Null, "Browser instance should be initialized");
        Assert.That(BrowserManager.Context, Is.Not.Null, "Browser context should be initialized");

        var config = Config.Framework.Playwright;
        Logger.Info("Browser type: {BrowserType}", config.BrowserType);
        Logger.Info("Headless mode: {Headless}", config.HeadlessMode);

        Logger.Info("Test passed - Browser is ready");
    }
}
