using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Pages;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// Page object for product listing and details.
    /// Encapsulates product-related interactions.
    /// </summary>
    public class ProductPage : BasePage
    {
        private readonly string _productListContainer = ".products";
        private readonly string _productItem = ".product-item";
        private readonly string _productName = ".product-name";
        private readonly string _productPrice = ".product-price";
        private readonly string _addToCartButton = "button[class*='add-to-cart']";
        private readonly string _productFilter = "input[placeholder*='filter']";

        public ProductPage(IPage page, ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Waits for products to load.
        /// </summary>
        public async Task WaitForProductsLoadAsync()
        {
            _logger.Information("Waiting for products to load");
            await WaitForElementVisibleAsync(_productListContainer);
        }

        /// <summary>
        /// Gets count of available products.
        /// </summary>
        public async Task<int> GetProductCountAsync()
        {
            _logger.Information("Getting product count");
            await WaitForProductsLoadAsync();
            var productElements = await _page.Locator(_productItem).AllAsync();
            _logger.Information("Found {Count} products", productElements.Count);
            return productElements.Count;
        }

        /// <summary>
        /// Gets list of product names.
        /// </summary>
        public async Task<List<string>> GetProductNamesAsync()
        {
            _logger.Information("Getting product names");
            await WaitForProductsLoadAsync();
            var names = new List<string>();
            var productElements = await _page.Locator(_productItem).AllAsync();

            foreach (var element in productElements)
            {
                var nameElement = element.Locator(_productName);
                var name = await nameElement.TextContentAsync();
                names.Add(name);
            }

            _logger.Information("Retrieved {Count} product names", names.Count);
            return names;
        }

        /// <summary>
        /// Filters products by name.
        /// </summary>
        public async Task FilterProductsByNameAsync(string filterText)
        {
            _logger.Information("Filtering products by: {FilterText}", filterText);
            await FillAsync(_productFilter, filterText);
            await Task.Delay(500); // Allow filter to process
        }

        /// <summary>
        /// Clicks add to cart for first product.
        /// </summary>
        public async Task AddFirstProductToCartAsync()
        {
            _logger.Information("Adding first product to cart");
            var firstAddButton = _page.Locator(_productItem).First.Locator(_addToCartButton);
            await firstAddButton.ClickAsync();
            _logger.Information("Product added to cart");
        }

        /// <summary>
        /// Adds product by name to cart.
        /// </summary>
        public async Task AddProductToCartByNameAsync(string productName)
        {
            _logger.Information("Adding product to cart: {ProductName}", productName);
            var productElement = _page.Locator(_productItem)
                .Filter(new LocatorFilterOptions { HasText = productName }).First;
            var addButton = productElement.Locator(_addToCartButton);
            await addButton.ClickAsync();
            _logger.Information("Product '{ProductName}' added to cart", productName);
        }

        /// <summary>
        /// Gets product price by name.
        /// </summary>
        public async Task<string> GetProductPriceByNameAsync(string productName)
        {
            _logger.Information("Getting price for product: {ProductName}", productName);
            var productElement = _page.Locator(_productItem)
                .Filter(new LocatorFilterOptions { HasText = productName }).First;
            var priceElement = productElement.Locator(_productPrice);
            var price = await priceElement.TextContentAsync();
            _logger.Information("Product price: {Price}", price);
            return price;
        }
    }
}
