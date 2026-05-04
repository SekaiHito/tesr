using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Core.Pages;
using Serilog;

namespace PlaywrightTests.Pages
{
    public class CartPage : BasePage
    {
        private const string CartContainer = "#cart_contents_container";
        private const string CartItem = ".cart_item";
        private const string CheckoutButton = "[data-test='checkout']";
        private const string ContinueShoppingButton = "[data-test='continue-shopping']";

        public CartPage(IPage page, ILogger logger) : base(page, logger) { }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(CartContainer);
        }

        public async Task<int> GetCartItemCountAsync()
        {
            await WaitForLoadedAsync();
            return await _page.Locator(CartItem).CountAsync();
        }

        public async Task RemoveFirstItemAsync()
        {
            await WaitForLoadedAsync();
            await _page.Locator(CartItem).First.Locator("button[data-test^='remove-']").ClickAsync();
        }

        public async Task CheckoutAsync()
        {
            await WaitForLoadedAsync();
            await ClickAsync(CheckoutButton);
            await WaitForPageLoadAsync();
        }

        public async Task ContinueShoppingAsync()
        {
            await WaitForLoadedAsync();
            await ClickAsync(ContinueShoppingButton);
            await WaitForPageLoadAsync();
        }
    }
}

