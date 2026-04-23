using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Pages;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// Page object for login functionality.
    /// Encapsulates login-related interactions and locators.
    /// </summary>
    public class LoginPage : BasePage
    {
        private readonly string _loginModalButton = "#login2";
        private readonly string _usernameInput = "#loginusername";
        private readonly string _passwordInput = "#loginpassword";
        private readonly string _loginSubmitButton = "button[onclick='logIn()']";
        private readonly string _welcomeMessage = "#nameofuser";

        public LoginPage(IPage page, ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Opens the login modal dialog.
        /// </summary>
        public async Task OpenLoginModalAsync()
        {
            _logger.Information("Opening login modal");
            await ClickAsync(_loginModalButton);
        }

        /// <summary>
        /// Performs login with provided credentials.
        /// </summary>
        public async Task LoginAsync(string username, string password)
        {
            _logger.Information("Attempting login for user: {Username}", username);
            await FillAsync(_usernameInput, username);
            await FillAsync(_passwordInput, password);
            await ClickAsync(_loginSubmitButton);
            _logger.Information("Login form submitted");
        }

        /// <summary>
        /// Gets the welcome message displayed after successful login.
        /// </summary>
        public async Task<string> GetWelcomeMessageAsync()
        {
            _logger.Information("Retrieving welcome message");
            await WaitForElementVisibleAsync(_welcomeMessage);
            string message = await GetTextAsync(_welcomeMessage);
            _logger.Information("Welcome message: {Message}", message);
            return message;
        }
    }
}
