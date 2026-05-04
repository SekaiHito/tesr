using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Pages;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// Page object для сторінки логіну Swag Labs.
    /// Використовує data-test атрибути для стабільності селекторів.
    /// </summary>
    public class LoginPage : BasePage
    {
        // Селектори на основі наданого HTML
        private readonly string _usernameInput = "[data-test='username']";
        private readonly string _passwordInput = "[data-test='password']";
        private readonly string _loginButton = "[data-test='login-button']";
        private readonly string _errorMessage = "[data-test='error']"; 

        public LoginPage(IPage page, ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Перехід на сторінку (якщо потрібно для фреймворку)
        /// </summary>
        public async Task NavigateAsync(string url)
        {
            _logger.Information("Navigating to: {Url}", url);
            await _page.GotoAsync(url);
        }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(_loginButton);
        }

        /// <summary>
        /// Виконує вхід у систему.
        /// </summary>
        public async Task LoginAsync(string username, string password)
        {
            _logger.Information("Attempting login with username: {Username}", username);
            
            await FillAsync(_usernameInput, username);
            await FillAsync(_passwordInput, password);
            await ClickAsync(_loginButton);
            
            _logger.Information("Login button clicked");
        }

        /// <summary>
        /// Перевіряє, чи відображається помилка при некоректних даних.
        /// </summary>
        public async Task<string?> GetErrorMessageAsync()
        {
            if (await IsVisibleAsync(_errorMessage))
            {
                var text = await GetTextAsync(_errorMessage);
                _logger.Warning("Login error detected: {Error}", text);
                return text;
            }
            return null;
        }
    }
}