# How to Use PlaywrightTests Framework

## 📚 Table of Contents
1. [Initial Setup](#initial-setup)
2. [Running Tests](#running-tests)
3. [Creating Tests](#creating-tests)
4. [Creating Page Objects](#creating-page-objects)
5. [Configuration](#configuration)
6. [Troubleshooting](#troubleshooting)

---

## Initial Setup

### Step 1: Navigate to Framework
```bash
cd /path/to/lab7
cd lab7
```

### Step 2: Make setup script executable
```bash
chmod +x setup.sh
```

### Step 3: Run one-time setup (Arch Linux)
```bash
bash setup.sh --setup
```

This will:
- ✅ Check system requirements
- ✅ Install .NET dependencies
- ✅ Restore NuGet packages  
- ✅ Build the project
- ✅ Create necessary directories

### Step 4: Verify installation
```bash
cd PlaywrightTests
dotnet test --configuration Release --filter "Category=Smoke"
```

Expected output: Tests pass successfully ✓

---

## Running Tests

### Run All Tests
```bash
make test
```

### Run Specific Test Categories
```bash
make test-ui        # UI tests only
make test-api       # API tests only  
make test-smoke     # Quick smoke tests (~5 min)
```

### Run Parallel Tests (Faster)
```bash
make test-parallel
```

### Run Single Test Class
```bash
dotnet test --filter "Name~LoginTest"
```

### Run Single Test Method
```bash
dotnet test --filter "Name~UserCanLoginSuccessfully"
```

### Run with Debug Output
```bash
dotnet test --verbosity detailed --logger "console;verbosity=detailed"
```

### View Test Results
```bash
make report-open    # Generate & open Allure report
```

---

## Creating Tests

### Example 1: Simple UI Test

Create file: `PlaywrightTests/Tests/UI/MyFirstTest.cs`

```csharp
using System.Threading.Tasks;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using PlaywrightTests.Pages;
using PlaywrightTests.Config;

namespace PlaywrightTests.Tests.UI
{
    [TestFixture]
    [AllureSuite("My Tests")]
    [AllureFeature("My Feature")]
    public class MyFirstTest : BaseTest
    {
        /// <summary>
        /// Test: User can navigate to home page
        /// </summary>
        [Test]
        [AllureTag("Smoke")]
        [Category("Smoke")]
        public async Task UserCanNavigateToHomePage()
        {
            // Arrange
            Logger.Information("Test started: UserCanNavigateToHomePage");
            var homePage = new HomePage(Page, Logger);
            string baseUrl = ConfigManager.Environment.BaseUrl;

            // Act
            Logger.Information("Navigating to: {BaseUrl}", baseUrl);
            await homePage.NavigateAsync(baseUrl);

            // Assert
            Logger.Information("Verifying home page loaded");
            await homePage.VerifyHomePageLoadedAsync();

            Logger.Information("Test passed!");
        }
    }
}
```

### Example 2: API Test

Create file: `PlaywrightTests/Tests/API/MyApiTest.cs`

```csharp
using System.Threading.Tasks;
using NUnit.Framework;
using Allure.NUnit.Attributes;
using PlaywrightTests.Core.API;

namespace PlaywrightTests.Tests.API
{
    [TestFixture]
    [AllureSuite("My API Tests")]
    [AllureFeature("My API")]
    public class MyApiTest : BaseApiTest
    {
        [Test]
        [AllureTag("Smoke")]
        [Category("API")]
        public async Task MyApiTest_ReturnsData()
        {
            // Arrange
            Logger.Information("Test: MyApiTest_ReturnsData");

            // Act
            var response = await ProductApiClient.GetProductsAsync();

            // Assert
            Assert.That(response, Is.Not.Null, "Response should not be null");
            Assert.That(response.Success, Is.True, "Response should indicate success");
            Assert.That(response.Data, Is.Not.Null, "Data should be returned");

            Logger.Information("Test passed!");
        }
    }
}
```

### Run Your New Tests
```bash
# Run your specific test
dotnet test --filter "Name~MyFirstTest"

# Or run all tests
make test
```

---

## Creating Page Objects

### Step 1: Create Page Class

Create file: `PlaywrightTests/Pages/MyCustomPage.cs`

```csharp
using System.Threading.Tasks;
using Microsoft.Playwright;
using Serilog;
using PlaywrightTests.Core.Pages;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// Page object for my custom page.
    /// Note: Replace locators with real selectors for your application.
    /// </summary>
    public class MyCustomPage : BasePage
    {
        // Private selectors - customize these for your page
        private readonly string _pageTitle = "h1";
        private readonly string _submitButton = "button[type='submit']";
        private readonly string _inputField = "#myInput";
        private readonly string _successMessage = ".success-message";

        public MyCustomPage(IPage page, ILogger logger) : base(page, logger)
        {
        }

        /// <summary>
        /// Verifies page is loaded
        /// </summary>
        public async Task VerifyPageLoadedAsync()
        {
            _logger.Information("Verifying page loaded");
            await WaitForElementVisibleAsync(_pageTitle);
        }

        /// <summary>
        /// Enters text in input field
        /// </summary>
        public async Task EnterTextAsync(string text)
        {
            _logger.Information("Entering text: {Text}", text);
            await FillAsync(_inputField, text);
        }

        /// <summary>
        /// Clicks submit button
        /// </summary>
        public async Task SubmitAsync()
        {
            _logger.Information("Clicking submit button");
            await ClickAsync(_submitButton);
            await Task.Delay(500); // Wait for action to process
        }

        /// <summary>
        /// Gets success message text
        /// </summary>
        public async Task<string> GetSuccessMessageAsync()
        {
            _logger.Information("Getting success message");
            await WaitForElementVisibleAsync(_successMessage);
            return await GetTextAsync(_successMessage);
        }
    }
}
```

### Step 2: Use in Test

```csharp
[Test]
public async Task UserCanSubmitForm()
{
    // Use your page object
    var myPage = new MyCustomPage(Page, Logger);
    
    // Navigate
    await myPage.NavigateAsync("https://example.com/mypage");
    await myPage.VerifyPageLoadedAsync();
    
    // Interact
    await myPage.EnterTextAsync("Test input");
    await myPage.SubmitAsync();
    
    // Verify
    string message = await myPage.GetSuccessMessageAsync();
    Assert.That(message, Does.Contain("Success"));
}
```

### Page Object Best Practices

```csharp
public class BestPracticePage : BasePage
{
    // ✅ GOOD: Descriptive selector names, stable locators
    private readonly string _nextButton = "button[aria-label='Next']";
    private readonly string _emailInput = "input[data-testid='email']";
    private readonly string _errorAlert = ".alert.alert-danger";
    
    // ❌ BAD: Fragile selectors, unclear names
    private readonly string _btn = "button:nth-child(3)";
    private readonly string _txt = "input";
    
    // ✅ GOOD: Async, logging, error handling
    public async Task EnterEmailAsync(string email)
    {
        _logger.Information("Entering email: {Email}", email);
        await FillAsync(_emailInput, email);
    }
    
    // ❌ BAD: No logging, synchronous
    public void EnterEmail(string email)
    {
        _page.Locator(_emailInput).FillAsync(email).Wait();
    }
}
```

---

## Configuration

### Change Environment

```bash
# Dev environment (default)
TEST_ENV=dev make test

# Staging environment
TEST_ENV=staging make test

# Production environment (careful!)
TEST_ENV=production make test-smoke
```

### Change Browser Type

```bash
# Chromium (default)
dotnet test

# Firefox
export PLAYWRIGHT_BROWSER=firefox
dotnet test

# WebKit
export PLAYWRIGHT_BROWSER=webkit
dotnet test
```

### Headless/Headed Mode

```bash
# Headless (no UI, default)
make test

# With UI (for debugging)
export PLAYWRIGHT_HEADLESS=false
dotnet test
```

### Configuration Files

Edit `PlaywrightTests/appsettings.json`:

```json
{
  "Playwright": {
    "BrowserType": "chromium",
    "HeadlessMode": true,
    "Timeout": 30000,        // 30 seconds
    "SlowMoDelay": 0,        // Set to 1000 to slow down for debugging
    "ViewportWidth": 1920,
    "ViewportHeight": 1080
  },
  "Logging": {
    "MinimumLevel": "Information"  // Change to "Debug" for verbose logging
  }
}
```

Edit environment configs:

**Dev**: `PlaywrightTests/Config/Environments/appsettings.dev.json`
**Staging**: `PlaywrightTests/Config/Environments/appsettings.staging.json`
**Production**: `PlaywrightTests/Config/Environments/appsettings.production.json`

```json
{
  "Environment": "dev",
  "BaseUrl": "https://dev.example.com",
  "ApiUrl": "https://api-dev.example.com",
  "Credentials": {
    "Username": "testuser",
    "Password": "${TEST_PASSWORD}"  // Use environment variable
  }
}
```

### Set Environment Variables

```bash
# One-time for current session
export TEST_ENV=dev
export TEST_PASSWORD=mypassword
export TEST_BROWSER=chromium
export PLAYWRIGHT_HEADLESS=false

# Or run inline
TEST_ENV=staging TEST_PASSWORD=pass123 make test

# On Windows PowerShell
$env:TEST_ENV = "dev"
$env:TEST_PASSWORD = "mypassword"
dotnet test
```

---

## Building Workflows

### Complete Development Workflow

```bash
# 1. Navigate to framework
cd lab7

# 2. Clean previous run
make clean-all

# 3. Build project
make build

# 4. Run all tests
make test

# 5. Generate report
make report-open

# 6. View logs
tail -f PlaywrightTests/Logs/test-*.log

# 7. View screenshots
ls -la PlaywrightTests/Screenshots/
```

### Quick Testing During Development

```bash
# Terminal 1: Watch for test file changes
while inotifywait -e modify PlaywrightTests/Tests/UI/*.cs; do
    clear
    make test-smoke
done

# Terminal 2: View logs in real-time
tail -f PlaywrightTests/Logs/test-*.log
```

### Code Quality & Testing

```bash
# Format code
make format

# Run code analysis
make lint

# Build with strict analysis
dotnet build /p:EnforceCodeStyleInBuild=true

# Run all quality checks + tests
make all
```

---

## Advanced Usage

### Create Custom API Client

Create file: `PlaywrightTests/Core/API/MyApiClient.cs`

```csharp
using System.Threading.Tasks;
using PlaywrightTests.Core.API;
using Serilog;

namespace PlaywrightTests.Core.API
{
    public class MyApiClient : BaseApiClient
    {
        public MyApiClient(ILogger logger) : base(logger) { }

        // GET request
        public async Task<ApiResponse<MyData>> GetDataAsync(int id)
        {
            _logger.Information("Fetching data with ID: {Id}", id);
            return await GetAsync<ApiResponse<MyData>>($"api/data/{id}");
        }

        // POST request
        public async Task<ApiResponse<MyData>> CreateDataAsync(CreateDataRequest request)
        {
            _logger.Information("Creating data");
            return await PostAsync<ApiResponse<MyData>>("api/data", request);
        }

        // PUT request
        public async Task<ApiResponse<MyData>> UpdateDataAsync(int id, UpdateDataRequest request)
        {
            _logger.Information("Updating data with ID: {Id}", id);
            return await PutAsync<ApiResponse<MyData>>($"api/data/{id}", request);
        }

        // DELETE request
        public async Task<ApiResponse<object>> DeleteDataAsync(int id)
        {
            _logger.Information("Deleting data with ID: {Id}", id);
            return await DeleteAsync<ApiResponse<object>>($"api/data/{id}");
        }
    }

    // Data Transfer Objects
    public class MyData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateDataRequest
    {
        public string Name { get; set; }
    }

    public class UpdateDataRequest
    {
        public string Name { get; set; }
    }
}
```

### Use Custom API Client in Tests

```csharp
[TestFixture]
public class MyApiTest : BaseApiTest
{
    private MyApiClient _myApiClient;

    [SetUp]
    public void Setup()
    {
        base.Setup();
        _myApiClient = new MyApiClient(Logger);
    }

    [Test]
    public async Task GetData_ReturnsSuccessfulResponse()
    {
        var response = await _myApiClient.GetDataAsync(1);
        Assert.That(response.Success, Is.True);
    }
}
```

### Using Test Data Helpers

```csharp
[Test]
public async Task TestWithGeneratedData()
{
    var dataHelpers = new DataHelpers(Logger);
    
    // Generate random data
    string randomEmail = dataHelpers.GenerateRandomEmail();
    string randomString = dataHelpers.GenerateRandomString(10);
    (string user, string pass) = dataHelpers.GenerateTestCredentials();
    int randomNumber = dataHelpers.GenerateRandomNumber(1, 100);
    
    // Cache data for later use
    dataHelpers.CacheData("test_user_id", 12345);
    var cachedId = dataHelpers.GetCachedData("test_user_id");
    
    // Validate data
    bool isValidEmail = dataHelpers.IsValidEmail(randomEmail);
    bool matchesPattern = dataHelpers.ValidatePattern("test123", @"^[a-z]+\d+$");
    
    Logger.Information("Generated email: {Email}", randomEmail);
    Logger.Information("Generated credentials - User: {User}", user);
}
```

---

## Troubleshooting

### Issue: Tests timeout

**Solution 1: Increase timeout**
```json
// In appsettings.json
{
  "Playwright": {
    "Timeout": 60000  // increased from 30000
  }
}
```

**Solution 2: Check selector**
```csharp
// Make sure selector is correct
private readonly string _element = "#correct-id";  // ✓ Good

// Avoid fragile selectors
private readonly string _element = "div:nth-child(3) > span";  // ✗ Bad
```

### Issue: Browser won't launch

**Solution:**
```bash
# Remove cached browsers
rm -rf ~/.cache/ms-playwright

# Reinstall browsers
cd PlaywrightTests
dotnet build

# Or manually
dotnet build
```

### Issue: Tests fail intermittently (Flaky)

**Solution: Use proper waits**
```csharp
// ✓ Good: Explicit wait
await WaitForElementVisibleAsync("#success-message");

// ✗ Bad: Hard wait
await Task.Delay(5000);
```

### Issue: Can't find log files

```bash
# Check logs
ls -la PlaywrightTests/Logs/

# View latest logs
tail -f PlaywrightTests/Logs/test-*.log

# Search for errors
grep -i error PlaywrightTests/Logs/test-*.log
```

### Issue: Screenshots not saving

```bash
# Check permissions
ls -la PlaywrightTests/Screenshots/

# Enable screenshots in config
{
  "TestData": {
    "Screenshots": true,
    "Videos": false
  }
}
```

### Issue: Allure report won't generate

```bash
# Check if Allure is installed
allure --version

# Install Allure (Arch Linux)
sudo pacman -S allure

# Or install on Mac
brew install allure

# Manual generation
allure generate PlaywrightTests/allure-results -o allure-report --clean

# View
allure open allure-report
```

---

## Common Commands Reference

```bash
# Setup & Maintenance
bash setup.sh --setup          # One-time setup
bash setup.sh --install        # Install dependencies only
bash setup.sh --clean          # Clean artifacts
make clean-all                 # Deep clean

# Development
make build                     # Build project
make format                    # Format code
make lint                      # Run code analysis

# Testing
make test                      # Run all tests
make test-ui                   # UI tests
make test-api                  # API tests
make test-smoke                # Quick tests
make test-parallel             # Parallel execution

# Reporting
make report                    # Generate report
make report-open               # Generate & open

# Help
make help                      # Show all targets
bash setup.sh --help           # Setup help
```

---

## Next Steps

1. **Run existing tests**: `make test-smoke`
2. **View the report**: `make report-open`
3. **Create your first test**: Follow "Creating Tests" section
4. **Create page objects**: Follow "Creating Page Objects" section
5. **Read documentation**: Check `README.md` and `docs/`

---

## Need Help?

- 📖 **README.md** - Overview and features
- 🏗️ **docs/ARCHITECTURE.md** - System design
- 🧪 **docs/BEST_PRACTICES.md** - Testing patterns
- 🔌 **docs/API.md** - API testing guide
- 🔧 **docs/SETUP.md** - Detailed setup
- 📖 **CONTRIBUTING.md** - Code standards

**Happy Testing! 🚀**
