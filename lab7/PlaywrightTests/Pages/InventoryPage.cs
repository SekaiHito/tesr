using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Core.Pages;
using Serilog;

namespace PlaywrightTests.Pages
{
    public class InventoryPage : BasePage
    {
        private const string InventoryContainer = "[data-test='inventory-container']";
        private const string InventoryItem = ".inventory_item";
        private const string InventoryItemName = ".inventory_item_name";
        private const string InventoryItemPrice = ".inventory_item_price";
        private const string SortSelect = "[data-test='product-sort-container']";
        private const string CartLink = ".shopping_cart_link";
        private const string CartBadge = ".shopping_cart_badge";
        private const string MenuButton = "#react-burger-menu-btn";
        private const string LogoutLink = "#logout_sidebar_link";

        public InventoryPage(IPage page, ILogger logger) : base(page, logger) { }

        public async Task WaitForLoadedAsync()
        {
            await WaitForElementVisibleAsync(InventoryContainer);
        }

        public async Task<int> GetInventoryCountAsync()
        {
            await WaitForLoadedAsync();
            return await _page.Locator(InventoryItem).CountAsync();
        }

        public async Task AddItemToCartByNameAsync(string itemName)
        {
            await WaitForLoadedAsync();
            var item = _page.Locator(InventoryItem).Filter(new LocatorFilterOptions { HasText = itemName }).First;
            var addButton = item.Locator("button[data-test^='add-to-cart-']");
            await addButton.ClickAsync();
        }

        public async Task AddFirstItemToCartAsync()
        {
            await WaitForLoadedAsync();
            var firstItem = _page.Locator(InventoryItem).First;
            await firstItem.Locator("button[data-test^='add-to-cart-']").ClickAsync();
        }

        public async Task<string?> GetCartBadgeTextAsync()
        {
            var badge = _page.Locator(CartBadge);
            if (!await badge.IsVisibleAsync())
                return null;
            return await badge.TextContentAsync();
        }

        public async Task OpenCartAsync()
        {
            await ClickAsync(CartLink);
            await WaitForPageLoadAsync();
        }

        public async Task LogoutAsync()
        {
            await ClickAsync(MenuButton);
            await _page.Locator(LogoutLink).ClickAsync();
            await WaitForPageLoadAsync();
        }

        public async Task SelectSortAsync(string value)
        {
            await WaitForLoadedAsync();
            await _page.Locator(SortSelect).SelectOptionAsync(new SelectOptionValue { Value = value });
        }

        public async Task<IReadOnlyList<decimal>> GetVisiblePricesAsync()
        {
            await WaitForLoadedAsync();
            var texts = await _page.Locator(InventoryItemPrice).AllTextContentsAsync();
            return texts
                .Select(t => t.Trim().TrimStart('$'))
                .Select(t => decimal.Parse(t, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture))
                .ToArray();
        }

        public async Task<string> GetFirstItemNameAsync()
        {
            await WaitForLoadedAsync();
            var name = await _page.Locator(InventoryItemName).First.TextContentAsync();
            return name?.Trim() ?? throw new InvalidOperationException("Failed to read first inventory item name.");
        }
    }
}

