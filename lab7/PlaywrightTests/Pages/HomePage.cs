using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Pages;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// Page object for home page functionality.
    /// Encapsulates home page interactions and navigation.
    /// </summary>
    public class HomePage : BasePage
    {
        private readonly string _homeTitle = "h1";
        private readonly string _productLink = "a[href*='product']";
        private readonly string _cartLink = "a[href*='cart']";
        private readonly string _contactLink = "a[href*='contact']";
        private readonly string _aboutLink = "a[href*='about']";

        public HomePage(IPage page, ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Verifies home page is loaded.
        /// </summary>
        public async Task VerifyHomePageLoadedAsync()
        {
            _logger.Information("Verifying home page loaded");
            await WaitForElementVisibleAsync(_homeTitle);
        }

        /// <summary>
        /// Clicks on products link.
        /// </summary>
        public async Task ClickProductsAsync()
        {
            _logger.Information("Clicking products link");
            await ClickAsync(_productLink);
            await WaitForPageLoadAsync();
        }

        /// <summary>
        /// Clicks on cart link.
        /// </summary>
        public async Task ClickCartAsync()
        {
            _logger.Information("Clicking cart link");
            await ClickAsync(_cartLink);
            await WaitForPageLoadAsync();
        }

        /// <summary>
        /// Clicks on contact link.
        /// </summary>
        public async Task ClickContactAsync()
        {
            _logger.Information("Clicking contact link");
            await ClickAsync(_contactLink);
            await WaitForPageLoadAsync();
        }

        /// <summary>
        /// Clicks on about link.
        /// </summary>
        public async Task ClickAboutAsync()
        {
            _logger.Information("Clicking about link");
            await ClickAsync(_aboutLink);
            await WaitForPageLoadAsync();
        }

        /// <summary>
        /// Gets home page title.
        /// </summary>
        public async Task<string> GetPageTitleAsync()
        {
            _logger.Information("Getting home page title");
            return await GetTextAsync(_homeTitle);
        }
    }
}
