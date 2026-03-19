using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTests.Utils; 

namespace PlaywrightTests.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage _page;

        protected BasePage(IPage page)
        {
            _page = page;
        }

        protected async Task ClickElementAsync(string locator)
        {
            Logger.Info($"Клікаю по елементу: {locator}");
            await _page.Locator(locator).ClickAsync();
        }

        protected async Task FillTextAsync(string locator, string text)
        {
            Logger.Info($"Вводжу текст '{text}' у поле: {locator}");
            await _page.Locator(locator).FillAsync(text);
        }

        public async Task NavigateAsync(string url)
        {
            Logger.Info($"Переходжу за адресою: {url}");
            await _page.GotoAsync(url);
        }
    }
}