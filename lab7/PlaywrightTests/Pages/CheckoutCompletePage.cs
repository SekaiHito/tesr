using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Core.Pages;
using Serilog;

namespace PlaywrightTests.Pages
{
    public class CheckoutCompletePage : BasePage
    {
        private const string Container = "#checkout_complete_container";
        private const string CompleteHeader = ".complete-header";
        private const string BackHome = "[data-test='back-to-products']";

        public CheckoutCompletePage(IPage page, ILogger logger) : base(page, logger) { }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(Container);
        }

        public async Task<string> GetCompleteHeaderAsync()
        {
            await WaitForLoadedAsync();
            return (await GetTextAsync(CompleteHeader)).Trim();
        }

        public async Task BackHomeAsync()
        {
            await ClickAsync(BackHome);
            await WaitForPageLoadAsync();
        }
    }
}

