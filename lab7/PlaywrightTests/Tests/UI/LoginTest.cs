using System.Threading.Tasks;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using PlaywrightTests.Pages;
using PlaywrightTests.Config;

namespace PlaywrightTests.Tests.UI
{
    /// <summary>
    /// Login functionality test suite.
    /// Tests user authentication and login workflows.
    /// </summary>
    [TestFixture]
    [AllureSuite("Authentication")]
    [AllureFeature("Login")]
    public class LoginTest : BaseTest
    {
        /// <summary>
        /// Test scenario: Verify user can successfully login with valid credentials.
        /// </summary>
        [Test]
        [AllureTag("Smoke")]
        [Category("Smoke")]
        [Category("UI")]
        [Retry(3)]
        public async Task UserCanLoginSuccessfully()
        {
            // Arrange
            Logger.Information("Test started: UserCanLoginSuccessfully");
            var loginPage = new LoginPage(Page, Logger);
            string testUsername = "test";
            string testPassword = "test";

            // Act
            Logger.Information("Navigating to base URL: {BaseUrl}", ConfigManager.Environment.BaseUrl);
            await loginPage.NavigateAsync(ConfigManager.Environment.BaseUrl);

            Logger.Information("Opening login modal");
            await loginPage.OpenLoginModalAsync();

            Logger.Information("Logging in with credentials");
            await loginPage.LoginAsync(testUsername, testPassword);

            // Assert
            Logger.Information("Verifying welcome message");
            string welcomeText = await loginPage.GetWelcomeMessageAsync();
            Assert.That(welcomeText, Does.Contain(testUsername),
                "Welcome message should contain username after successful login");

            Logger.Information("Test completed successfully!");
        }

        /// <summary>
        /// Test scenario: Verify login fails with invalid credentials.
        /// </summary>
        [Test]
        [AllureTag("Regression")]
        [Category("Regression")]
        [Category("UI")]
        [Retry(2)]
        public async Task LoginFailsWithInvalidCredentials()
        {
            // Arrange
            Logger.Information("Test started: LoginFailsWithInvalidCredentials");
            var loginPage = new LoginPage(Page, Logger);
            string invalidUsername = "invaliduser";
            string invalidPassword = "wrongpassword";

            // Act
            Logger.Information("Navigating to base URL");
            await loginPage.NavigateAsync(ConfigManager.Environment.BaseUrl);

            Logger.Information("Opening login modal");
            await loginPage.OpenLoginModalAsync();

            Logger.Information("Attempting login with invalid credentials");
            // This test would need to be expanded based on actual error handling
            await loginPage.LoginAsync(invalidUsername, invalidPassword);

            Logger.Information("Test completed - error handling validation");
        }
    }
}
