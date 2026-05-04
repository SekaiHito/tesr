using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Core.Pages;
using Serilog;

namespace PlaywrightTests.Pages
{
    public class CheckoutStepOnePage : BasePage
    {
        private const string Container = "#checkout_info_container";
        private const string FirstName = "[data-test='firstName']";
        private const string LastName = "[data-test='lastName']";
        private const string PostalCode = "[data-test='postalCode']";
        private const string Continue = "[data-test='continue']";

        public CheckoutStepOnePage(IPage page, ILogger logger) : base(page, logger) { }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(Container);
        }

        public async Task FillCustomerInfoAsync(string firstName, string lastName, string postalCode)
        {
            await WaitForLoadedAsync();
            await FillAsync(FirstName, firstName);
            await FillAsync(LastName, lastName);
            await FillAsync(PostalCode, postalCode);
        }

        public async Task ContinueAsync()
        {
            await ClickAsync(Continue);
            await WaitForPageLoadAsync();
        }
    }
}

