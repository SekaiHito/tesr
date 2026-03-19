using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTests.Pages;
using PlaywrightTests.Config;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Tests.UI
{
    [TestFixture]
    public class LoginTest : BaseTest
    {
        [Test]
        public async Task UserCanLoginSuccessfully()
        {
            Logger.Info("Початок тесту: UserCanLoginSuccessfully");

            var loginPage = new LoginPage(Page);

            await loginPage.NavigateAsync(ConfigManager.Settings.BaseUrl);
            await loginPage.OpenLoginModalAsync();
            
            await loginPage.LoginAsync("test", "test");
            string welcomeText = await loginPage.GetWelcomeMessageAsync();
            Assert.That(welcomeText, Does.Contain("test"), "Користувач не залогінився!");
            await Page.WaitForTimeoutAsync(3000);    
            Logger.Info("Тест успішно пройдено!");
        }
    }
}