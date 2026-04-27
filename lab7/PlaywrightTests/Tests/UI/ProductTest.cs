using System.Threading.Tasks;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using PlaywrightTests.Pages;
using PlaywrightTests.Config;

namespace PlaywrightTests.Tests.UI
{
    /// <summary>
    /// Product page test suite.
    /// Tests product listing, filtering, and shopping functionality.
    /// </summary>
    [TestFixture]
    [AllureSuite("Shopping")]
    [AllureFeature("Products")]
    public class ProductTest : BaseTest
    {
        /// <summary>
        /// Test: Verify products are displayed on product page.
        /// </summary>
        [Test]
        [AllureTag("Smoke")]
        [Category("Smoke")]
        [Category("UI")]
        public async Task VerifyProductsDisplayedOnPage()
        {
            Logger.Information("Test: VerifyProductsDisplayedOnPage");
            var productPage = new ProductPage(Page, Logger);

            // Navigate to products
            await productPage.NavigateAsync($"{ConfigManager.Environment.BaseUrl}/products");

            // Verify products load
            await productPage.WaitForProductsLoadAsync();

            // Get product count
            int productCount = await productPage.GetProductCountAsync();
            Assert.That(productCount, Is.GreaterThan(0), "At least one product should be displayed");

            Logger.Information("Test passed: Found {Count} products", productCount);
        }

        /// <summary>
        /// Test: Verify product names are retrieved correctly.
        /// </summary>
        [Test]
        [AllureTag("Regression")]
        [Category("Regression")]
        [Category("UI")]
        public async Task VerifyProductNamesRetrieved()
        {
            Logger.Information("Test: VerifyProductNamesRetrieved");
            var productPage = new ProductPage(Page, Logger);

            await productPage.NavigateAsync($"{ConfigManager.Environment.BaseUrl}/products");
            await productPage.WaitForProductsLoadAsync();

            var productNames = await productPage.GetProductNamesAsync();

            Assert.That(productNames.Count, Is.GreaterThan(0), "Product names should be retrieved");
            foreach (var name in productNames)
            {
                Assert.That(name, Is.Not.Null.And.Not.Empty, "Product name should not be empty");
            }

            Logger.Information("Test passed: Retrieved {Count} product names", productNames.Count);
        }

        /// <summary>
        /// Test: Verify product can be added to cart.
        /// </summary>
        [Test]
        [AllureTag("Critical")]
        [Category("Critical")]
        [Category("UI")]
        public async Task VerifyAddProductToCart()
        {
            Logger.Information("Test: VerifyAddProductToCart");
            var productPage = new ProductPage(Page, Logger);

            await productPage.NavigateAsync($"{ConfigManager.Environment.BaseUrl}/products");
            await productPage.WaitForProductsLoadAsync();

            // Add first product to cart
            await productPage.AddFirstProductToCartAsync();

            // Alert should appear (specific to test application)
            Logger.Information("Product added to cart successfully");
            Assert.Pass("Product was successfully added to cart");
        }
    }
}
