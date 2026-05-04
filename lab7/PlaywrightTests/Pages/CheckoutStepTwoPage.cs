using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Core.Pages;
using Serilog;

namespace PlaywrightTests.Pages
{
    public class CheckoutStepTwoPage : BasePage
    {
        private const string Container = "#checkout_summary_container";
        private const string Finish = "[data-test='finish']";

        public CheckoutStepTwoPage(IPage page, ILogger logger) : base(page, logger) { }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(Container);
        }

        public async Task FinishAsync()
        {
            await WaitForLoadedAsync();
            await ClickAsync(Finish);
            await WaitForPageLoadAsync();
        }
    }
}

