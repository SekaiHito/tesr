using System;
using System.Threading.Tasks;
using Allure.NUnit;
using NUnit.Framework;
using PlaywrightTests.Config;
using PlaywrightTests.Pages;

[assembly: Parallelizable(ParallelScope.Children)]
[assembly: LevelOfParallelism(3)]

namespace PlaywrightTests.Tests.UI
{
    [TestFixture]
    [Category("UI")]
    [AllureNUnit]
    public class SauceDemoBusinessFlowsTest : BaseTest
    {
        private async Task LoginAsStandardUserAsync()
        {
            var loginPage = new LoginPage(Page, Logger);
            await loginPage.NavigateAsync(ConfigManager.Environment.BaseUrl);
            await loginPage.WaitForLoadedAsync();

            var username = ConfigManager.Environment.Credentials.GetValueOrDefault("Username", "standard_user");
            var password = ConfigManager.Environment.Credentials.GetValueOrDefault("Password", "secret_sauce");
            await loginPage.LoginAsync(username, password);

            var inventoryPage = new InventoryPage(Page, Logger);
            await inventoryPage.WaitForLoadedAsync();
            Assert.That(Page.Url, Does.Contain("inventory.html"), "After login user should land on inventory page.");
        }

        [Test]
        [Category("Smoke")]
        public async Task Flow01_Login_Inventory_Loads_And_Logout()
        {
            await LoginAsStandardUserAsync();

            var inventoryPage = new InventoryPage(Page, Logger);
            Assert.That(await inventoryPage.GetInventoryCountAsync(), Is.GreaterThan(0), "Inventory should list products.");

            await inventoryPage.LogoutAsync();

            var loginPage = new LoginPage(Page, Logger);
            await loginPage.WaitForLoadedAsync();
            Assert.That(Page.Url, Does.Not.Contain("inventory.html"), "After logout user should no longer be on inventory page.");
        }

        [Test]
        [Category("Regression")]
        public async Task Flow02_Login_Fails_With_Invalid_Credentials_Shows_Error()
        {
            var loginPage = new LoginPage(Page, Logger);
            await loginPage.NavigateAsync(ConfigManager.Environment.BaseUrl);
            await loginPage.WaitForLoadedAsync();

            await loginPage.LoginAsync("invalid_user", "invalid_password");

            var error = await loginPage.GetErrorMessageAsync();
            Assert.That(error, Is.Not.Null.And.Not.Empty, "Login error should be displayed.");
            Assert.That(error, Does.Contain("Epic sadface"), "SauceDemo returns an 'Epic sadface' error for invalid login.");
        }

        [Test]
        [Category("Regression")]
        public async Task Flow03_Add_To_Cart_Then_Remove_From_Cart()
        {
            await LoginAsStandardUserAsync();

            var inventoryPage = new InventoryPage(Page, Logger);
            await inventoryPage.AddFirstItemToCartAsync();

            Assert.That(await inventoryPage.GetCartBadgeTextAsync(), Is.EqualTo("1"), "Cart badge should show 1 item.");

            await inventoryPage.OpenCartAsync();
            var cartPage = new CartPage(Page, Logger);
            Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(1), "Cart should contain 1 item.");

            await cartPage.RemoveFirstItemAsync();
            Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(0), "Cart should be empty after removing item.");
        }

        [Test]
        [Category("Critical")]
        public async Task Flow04_Checkout_Single_Item_To_Completion()
        {
            await LoginAsStandardUserAsync();

            var inventoryPage = new InventoryPage(Page, Logger);
            var firstItemName = await inventoryPage.GetFirstItemNameAsync();
            await inventoryPage.AddItemToCartByNameAsync(firstItemName);
            await inventoryPage.OpenCartAsync();

            var cartPage = new CartPage(Page, Logger);
            Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(1), "Cart should contain selected item before checkout.");
            await cartPage.CheckoutAsync();

            var checkout1 = new CheckoutStepOnePage(Page, Logger);
            await checkout1.FillCustomerInfoAsync("Test", "User", "12345");
            await checkout1.ContinueAsync();

            var checkout2 = new CheckoutStepTwoPage(Page, Logger);
            await checkout2.FinishAsync();

            var complete = new CheckoutCompletePage(Page, Logger);
            var header = await complete.GetCompleteHeaderAsync();
            Assert.That(header, Is.EqualTo("Thank you for your order!"));
        }
    }
}

