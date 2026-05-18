using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [Test]
    public async Task GetStartedLink()
    {
        await page.GotoAsync("https://www.saucedemo.com/");
        await page.Locator("[data-test=\"username\"]").ClickAsync();
        await page.Locator("[data-test=\"username\"]").FillAsync("standard_user");
        await page.Locator("[data-test=\"password\"]").ClickAsync();
        await page.Locator("[data-test=\"password\"]").FillAsync("secret_sauce");
        await page.Locator("[data-test=\"login-button\"]").ClickAsync();
        await page.Locator("[data-test=\"add-to-cart-sauce-labs-backpack\"]").ClickAsync();
        await page.Locator("[data-test=\"add-to-cart-sauce-labs-bike-light\"]").ClickAsync();
        await page.Locator("[data-test=\"shopping-cart-link\"]").ClickAsync();
        await page.Locator("[data-test=\"checkout\"]").ClickAsync();
        await page.Locator("[data-test=\"cancel\"]").ClickAsync();
        await page.Locator("[data-test=\"remove-sauce-labs-backpack\"]").ClickAsync();
        await page.Locator("[data-test=\"remove-sauce-labs-bike-light\"]").ClickAsync();

    } 
}