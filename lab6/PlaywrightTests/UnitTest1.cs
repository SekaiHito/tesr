using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task MyTest()
    {
        await Page.GotoAsync("https://shop.polymer-project.org/");
        await Page.Locator("#tabContainer").GetByRole(AriaRole.Link, new() { Name = "Ladies Outerwear" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Men's T-Shirts" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "YouTube Organic Cotton T-" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add this item to cart" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Shopping cart: 1 item" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Shopping cart: 1 item" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete item YouTube Organic" }).ClickAsync();
    }
}
