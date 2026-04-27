# PlaywrightTests Framework - Quick Reference Card

## 🚀 One-Minute Quick Start

```bash
cd lab7
bash setup.sh --setup    # One-time only
make test               # Run all tests
make report-open        # View results
```

---

## 📊 Test Commands Cheat Sheet

| Command | Purpose |
|---------|---------|
| `make test` | Run all tests |
| `make test-ui` | UI tests only |
| `make test-api` | API tests only |
| `make test-smoke` | Quick tests (~5 min) |
| `make test-parallel` | Parallel execution (faster) |
| `dotnet test --filter "Name~LoginTest"` | Run specific test class |
| `dotnet test --filter "Name~UserCanLogin"` | Run specific test method |

---

## 📁 Key Files & Locations

| Path | Purpose |
|------|---------|
| `PlaywrightTests/Tests/UI/` | Create UI tests here |
| `PlaywrightTests/Tests/API/` | Create API tests here |
| `PlaywrightTests/Pages/` | Page objects go here |
| `PlaywrightTests/Core/API/` | API clients go here |
| `PlaywrightTests/appsettings.json` | Global configuration |
| `PlaywrightTests/Config/Environments/` | Environment-specific configs |
| `PlaywrightTests/Logs/` | Auto-generated test logs |
| `PlaywrightTests/Screenshots/` | Auto-generated error screenshots |
| `PlaywrightTests/allure-results/` | Auto-generated test results |

---

## 🧪 Test Structure Template

### UI Test Template
```csharp
[TestFixture]
[AllureSuite("My Tests")]
[AllureFeature("My Feature")]
public class MyTest : BaseTest
{
    [Test]
    [Category("Smoke")]
    public async Task TestName_Scenario_ExpectedResult()
    {
        // Arrange
        var page = new MyPage(Page, Logger);
        
        // Act
        await page.DoSomethingAsync();
        
        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
```

### API Test Template
```csharp
[TestFixture]
[AllureSuite("My API")]
public class MyApiTest : BaseApiTest
{
    [Test]
    [Category("API")]
    public async Task TestName()
    {
        var response = await ApiClient.GetDataAsync();
        Assert.That(response.Success, Is.True);
    }
}
```

### Page Object Template
```csharp
public class MyPage : BasePage
{
    private readonly string _selector = "#id";
    
    public MyPage(IPage page, ILogger logger) : base(page, logger) { }
    
    public async Task DoSomethingAsync()
    {
        await ClickAsync(_selector);
    }
}
```

---

## 🎯 Configuration Scenarios

### Run Against Different Environments
```bash
TEST_ENV=dev make test        # Dev
TEST_ENV=staging make test    # Staging
TEST_ENV=production make test # Production
```

### Use Different Browsers
```bash
export PLAYWRIGHT_BROWSER=chromium   # Default
export PLAYWRIGHT_BROWSER=firefox    # Firefox
export PLAYWRIGHT_BROWSER=webkit     # Safari-like
```

### Debug Mode (See Browser UI)
```bash
export PLAYWRIGHT_HEADLESS=false
make test
```

### Verbose Logging
```bash
# Edit appsettings.json
"MinimumLevel": "Debug"  # Instead of "Information"

# Then run
make test
```

---

## 📚 Page Object Methods

### Navigation & Waits
```csharp
await page.NavigateAsync(url);              // Go to URL
await page.WaitForElementVisibleAsync(sel); // Wait for element
await page.WaitForPageLoadAsync();          // Wait for page load
```

### User Interactions
```csharp
await page.ClickAsync(selector);            // Click element
await page.FillAsync(selector, text);       // Fill text input
await page.TypeAsync(selector, text);       // Type (slower, key by key)
```

### Retrieving Data
```csharp
string text = await page.GetTextAsync(sel);           // Get text
string attr = await page.GetAttributeAsync(sel, "id"); // Get attribute
bool visible = await page.IsVisibleAsync(selector);    // Check visibility
```

### Advanced
```csharp
await page.RetryActionAsync(async () => { }, 3);   // Retry logic
await page.ExecuteScriptAsync("script");           // Run JavaScript
```

---

## 📊 API Client Methods

### Base Operations
```csharp
var response = await client.GetAsync<T>("endpoint");
var response = await client.PostAsync<T>("endpoint", data);
var response = await client.PutAsync<T>("endpoint", data);
var response = await client.DeleteAsync<T>("endpoint");
```

### With Headers & Auth
```csharp
var headers = new Dictionary<string, string> { {"X-Key", "value"} };
await client.GetAsync<T>("endpoint", headers);
client.SetAuthorizationHeader("token");  // Bearer token
```

---

## 🔍 Debugging

### View Logs
```bash
tail -f PlaywrightTests/Logs/test-*.log    # Real-time logs
grep -i error PlaywrightTests/Logs/*       # Find errors
```

### View Screenshots
```bash
ls PlaywrightTests/Screenshots/            # List all
open PlaywrightTests/Screenshots/error.png # View screenshot
```

### View Test Report
```bash
make report-open          # Generate & open Allure report
```

### Run Single Test with Details
```bash
dotnet test --filter "Name~TestName" --verbosity detailed
```

---

## ⚠️ Common Issues & Fixes

| Issue | Fix |
|-------|-----|
| Timeout errors | Increase timeout in appsettings.json |
| Element not found | Check selector in browser DevTools |
| Browser won't launch | `rm -rf ~/.cache/ms-playwright && dotnet build` |
| Flaky tests | Use explicit waits instead of `Task.Delay()` |
| Logs not appearing | Create `Logs/` directory, check permissions |
| Report not generating | Install Allure: `sudo pacman -S allure` |

---

## 🎨 Naming Conventions

```
Test Names:
✓ UserCanLoginWithValidCredentials_EntersCorrectPassword_DashboardDisplayed
✗ Test1, LoginTest, Verify

Selector Names:
✓ _usernameInput, _submitButton, _errorMessage
✗ _inp, _btn, _msg

Method Names:
✓ async Task LoginAsync() / LoginWithRetryAsync()
✗ void Login(), LoginAndCheck()

Log Messages:
✓ Logger.Information("Logging in with username: {Username}", username);
✗ Logger.Info("Login");
```

---

## 🔄 Development Workflow

```bash
# 1. Create test
vim PlaywrightTests/Tests/UI/MyTest.cs

# 2. Create page object
vim PlaywrightTests/Pages/MyPage.cs

# 3. Run test
dotnet test --filter "Name~MyTest"

# 4. View logs
tail -f PlaywrightTests/Logs/test-*.log

# 5. Fix if needed, repeat step 3

# 6. Format code
make format

# 7. Run all tests
make test

# 8. Generate report
make report-open
```

---

## 📖 Documentation Files

```
README.md              - Start here (overview, features)
HOW_TO_USE.md         - This file
CONTRIBUTING.md       - Code standards & guidelines
docs/SETUP.md         - Arch Linux detailed setup
docs/ARCHITECTURE.md  - System design & patterns
docs/API.md           - API testing examples
docs/BEST_PRACTICES.md - Testing best practices
docs/INDEX.md         - Navigation & summary
```

---

## 🆘 Getting Help

1. **Quick help**: `make help`
2. **Setup help**: `bash setup.sh --help`
3. **Read docs**: Check `/docs/` folder
4. **Check logs**: `PlaywrightTests/Logs/test-*.log`
5. **View examples**: `PlaywrightTests/Tests/UI/LoginTest.cs`

---

## 🎯 Next Steps

- ✅ Run: `make test-smoke`
- ✅ View: `make report-open`
- ✅ Create: First custom test
- ✅ Design: Page objects
- ✅ Read: Full documentation

**Good luck! 🚀**
