using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class LoginPage : BasePage
    {
        private readonly string _loginModalButton = "#login2";
        private readonly string _usernameInput = "#loginusername";
        private readonly string _passwordInput = "#loginpassword";
        private readonly string _loginSubmitButton = "button[onclick='logIn()']";
        private readonly string _welcomeMessage = "#nameofuser";

        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task OpenLoginModalAsync()
        {
            await ClickElementAsync(_loginModalButton);
        }

        public async Task LoginAsync(string username, string password)
        {
            await FillTextAsync(_usernameInput, username);
            await FillTextAsync(_passwordInput, password);
            await ClickElementAsync(_loginSubmitButton);
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            await _page.Locator(_welcomeMessage).WaitForAsync();
            return await _page.Locator(_welcomeMessage).InnerTextAsync();
        }
    }
}