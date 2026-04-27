# Best Practices for PlaywrightTests Framework

This document outlines best practices for writing reliable, maintainable, and performant tests using the PlaywrightTests framework.

## Test Organization

### Directory Structure

```
Tests/
├── UI/
│   ├── Authentication/
│   │   ├── LoginTest.cs
│   │   └── LogoutTest.cs
│   ├── Products/
│   │   ├── ProductListTest.cs
│   │   └── ProductDetailTest.cs
│   └── Cart/
│       └── CartTest.cs
└── API/
    ├── Users/
    │   └── UserApiTest.cs
    ├── Products/
    │   └── ProductApiTest.cs
    └── Orders/
        └── OrderApiTest.cs
```

### Test Grouping

```csharp
[TestFixture]
[AllureSuite("Authentication")]  // Test suite/group
[AllureFeature("Login")]          // Feature being tested
public class LoginTest : BaseTest
{
    [Test]
    [Category("Smoke")]           // Test category
    [Category("UI")]
    [AllureTag("Critical")]       // Priority tag
    public async Task TestName() { }
}
```

## Test Naming Conventions

### Format

Use descriptive names that clearly indicate what is being tested:

```
TestName_Scenario_ExpectedResult
```

### Examples

```csharp
// ✅ Good - Clear and descriptive
UserCanLoginWithValidCredentials_EntersCorrectPassword_DashboardDisplayed
LoginFails_WithInvalidPassword_ErrorMessageShown
ProductCanBePurchased_ValidPayment_OrderConfirmed

// ❌ Bad - Unclear or too generic
Test1
LoginTest
VerifyLogin
Check
```

## Test Structure

### AAA Pattern (Arrange-Act-Assert)

```csharp
[Test]
public async Task ProductCanBeAddedToCart()
{
    // Arrange - Setup test preconditions
    var productPage = new ProductPage(Page, Logger);
    var productName = "Test Product";
    
    // Act - Execute the scenario
    await productPage.NavigateAsync(BaseUrl);
    await productPage.SearchForProductAsync(productName);
    await productPage.AddFirstProductToCartAsync();
    
    // Assert - Verify expected outcome
    int cartCount = await productPage.GetCartCountAsync();
    Assert.That(cartCount, Is.EqualTo(1), "Cart should contain 1 item");
}
```

## Best Practices

### 1. Independence

Tests must be completely independent:

```csharp
// ✅ Good - Each test can run standalone
[Test]
public async Task UserCanLogin()
{
    await page.NavigateAsync(BaseUrl);
    await page.LoginAsync(username, password);
    // Verify login
}

[Test]
public async Task UserCanLogout()
{
    await page.NavigateAsync(BaseUrl);
    await page.LoginAsync(username, password);  // Setup
    await page.LogoutAsync();
    // Verify logout
}

// ❌ Bad - Tests depend on each other
[Test]
public async Task UserCanLogin()
{
    // Expected to run first
}

[Test]
public async Task UserCanLogout()
{
    // Depends on UserCanLogin running first
}
```

### 2. Proper Waits

Always use explicit waits, never hard waits:

```csharp
// ✅ Good - Explicit wait for element
await page.WaitForElementVisibleAsync("#success-message");
string message = await page.GetTextAsync("#success-message");

// ❌ Bad - Hard wait
await Task.Delay(5000);  // Unreliable
string message = await page.GetTextAsync("#success-message");
```

### 3. Selector Strategy

Use stable, meaningful selectors:

```csharp
// ✅ Good - Stable selectors
private readonly string _loginButton = "#login-button";
private readonly string _usernameInput = "#username";
private readonly string _passwordInput = "input[name='password']";
private readonly string _submitButton = "button[data-testid='submit']";

// ❌ Bad - Fragile selectors
private readonly string _loginButton = "/html/body/div[1]/div[2]/button";  // Position dependent
private readonly string _usernameInput = "#form > div:nth-child(1) > input";  // Fragile CSS path
```

### 4. Error Handling

Provide meaningful error messages:

```csharp
// ✅ Good - Descriptive assertion messages
Assert.That(
    actualText,
    Is.EqualTo(expectedText),
    $"Expected text '{expectedText}' but got '{actualText}'"
);

// ❌ Bad - Generic message
Assert.That(actualText, Is.EqualTo(expectedText));
```

### 5. Logging

Log important steps and failures:

```csharp
// ✅ Good - Comprehensive logging
Logger.Information("Starting login process for user: {Username}", username);
Logger.Debug("Clicking login button at selector: {Selector}", _loginButton);
Logger.Information("Login completed successfully");
Logger.Error(ex, "Login failed for user: {Username}", username);

// ❌ Bad - Insufficient logging
Logger.Information("Login");
```

### 6. Configuration

Use configuration, not hardcoded values:

```csharp
// ✅ Good - Uses configuration
string baseUrl = ConfigManager.Environment.BaseUrl;
string username = Environment.GetEnvironmentVariable("TEST_USERNAME");
int timeout = ConfigManager.Playwright.Timeout;

// ❌ Bad - Hardcoded values
string baseUrl = "https://localhost:3000";
string username = "testuser";
int timeout = 30000;
```

### 7. Page Objects

Use Page Objects for all UI interactions:

```csharp
// ✅ Good - Clean separation of concerns
var loginPage = new LoginPage(Page, Logger);
await loginPage.LoginAsync("user", "pass");

// ❌ Bad - Direct Playwright usage in tests
await Page.Locator("#username").FillAsync("user");
await Page.Locator("#password").FillAsync("pass");
await Page.Locator("#login").ClickAsync();
```

### 8. Assertions

Keep assertions focused and clear:

```csharp
// ✅ Good - Single, focused assertion
bool isVisible = await loginPage.IsErrorMessageVisibleAsync();
Assert.That(isVisible, Is.True, "Error message should be visible");

// ❌ Bad - Multiple unrelated assertions
var page = new LoginPage(Page, Logger);
Assert.That(page.Title, Is.EqualTo("Login"));
Assert.That(await page.GetErrorMessage(), Does.Contain("Invalid"));
Assert.That(page.IsLoggedIn, Is.False);
Assert.That(await page.GetFieldCount(), Is.EqualTo(2));
```

### 9. Setup and Teardown

Always clean up resources:

```csharp
[SetUp]
public async Task Setup()
{
    Logger.Information("Test setup started");
    // Initialize browser and page
}

[TearDown]
public async Task Teardown()
{
    // Always clean up resources
    if (BrowserManager != null)
    {
        await BrowserManager.CloseBrowserAsync();
    }
}
```

### 10. Data Management

Use DataHelpers for test data:

```csharp
// ✅ Good - Centralized data management
var dataHelpers = new DataHelpers(Logger);
string testEmail = dataHelpers.GenerateRandomEmail();
(string user, string pass) = dataHelpers.GenerateTestCredentials();
dataHelpers.CacheData("created_user_id", userId);

// ❌ Bad - Hardcoded test data
string testEmail = "test123@example.com";  // Unique per run
string user = "staticuser";
string pass = "staticpass";
```

## Performance Optimization

### 1. Parallel Execution

```bash
# Run tests in parallel
dotnet test --parallel

# Or in Makefile
make test-parallel
```

### 2. Browser Context Reuse

```csharp
// In BaseTest setup - reuse context
if (_context == null)
{
    _context = await _browser.NewContextAsync();
}
Page = await _context.NewPageAsync();
```

### 3. Smart Timeouts

```csharp
// Use appropriate timeouts
const int QUICK_WAIT = 5000;      // 5 seconds
const int NORMAL_WAIT = 30000;    // 30 seconds
const int LONG_WAIT = 60000;      // 60 seconds

// Use specific timeouts for different operations
await WaitForElementVisibleAsync(selector, NORMAL_WAIT);
```

### 4. Minimal Logging

```csharp
// Adjust log level for performance
[SetUp]
public void Setup()
{
    LoggerManager.Initialize("Warning");  // Less verbose in production
}
```

## Flaky Test Handling

### Causes and Solutions

| Cause | Solution |
|-------|----------|
| Timing issues | Use explicit waits instead of hard waits |
| Network latency | Implement retry logic |
| Dynamic content | Use more stable selectors |
| Browser state | Reset state between tests |
| Resource contention | Isolate tests |

### Retry Implementation

```csharp
[Test]
[Retry(3)]  // Automatic retry up to 3 times
public async Task FlakyTest()
{
    // Test that might occasionally fail
}

// Or manual retry:
await page.RetryActionAsync(async () =>
{
    await clickButton();
}, maxAttempts: 3, delayMs: 1000);
```

## Continuous Integration

### Test Categorization

```csharp
// For CI/CD pipelines
[Category("Smoke")]        // Quick validation - 5 min
[Category("Regression")]   // Full test suite - 30 min  
[Category("Performance")]  // Performance tests
[Category("Integration")]  // Tests requiring external services
```

### CI/CD Practices

```bash
# Run smoke tests first (fast feedback)
dotnet test --filter "Category=Smoke"

# Then run full suite
dotnet test

# Generate reports
make report-open
```

## Debugging Failing Tests

### Enable Debug Logging

```json
{
  "Logging": {
    "MinimumLevel": "Debug"
  }
}
```

### Screenshots and Videos

```csharp
// Screenshots taken automatically on failure
// Enable videos in configuration:
{
  "TestData": {
    "Screenshots": true,
    "Videos": true
  }
}
```

### Trace Recording

```csharp
// Enable Playwright traces for debugging
Page.Context.Tracing.StartAsync(new()
{
    Screenshots = true,
    Snapshots = true
});

// Save trace on failure
await Page.Context.Tracing.StopAsync(new()
{
    Path = "trace.zip"
});
```

## Security Best Practices

### Credentials

```csharp
// ✅ Good - Use environment variables
string password = Environment.GetEnvironmentVariable("TEST_PASSWORD");

// ❌ Bad - Hardcoded credentials
string password = "MySecurePassword123";
```

### Logging

```csharp
// ✅ Good - Sanitize sensitive data
Logger.Information("Login with user: {Username}", MaskSensitiveData(password));

// ❌ Bad - Logging credentials
Logger.Information("Logging in with: {Username}:{Password}", username, password);
```

## Code Quality

### Formatting

```bash
# Auto-format code
dotnet format
```

### Analysis

```bash
# Run code analysis
dotnet build /p:EnforceCodeStyleInBuild=true
```

### Comments

- Only comment "why", notecode
- Keep comments up-to-date
- Remove commented-out code

```csharp
// ✅ Good - Explains the purpose
// Wait up to 30 seconds for the UI to update after button click
await WaitForElementVisibleAsync(successElement);

// ❌ Bad - Explains what the code does
// Click the button
await ClickAsync(submitButton);
```

---

**Version**: 1.0  
**Last Updated**: 2026-04-27
