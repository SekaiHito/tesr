# PlaywrightTests - .NET NUnit Test Automation Framework

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Playwright](https://img.shields.io/badge/Playwright-1.48.0-green)](https://learn.microsoft.com/en-us/playwright/dotnet/)
[![NUnit](https://img.shields.io/badge/NUnit-4.3.2-red)](https://nunit.org/)
[![Allure Report](https://img.shields.io/badge/Allure-Report-blue)](https://qameta.io/allure/)

A comprehensive, enterprise-grade test automation framework for .NET applications using Playwright and NUnit. Designed with SOLID principles, modularity, and extensibility in mind.

## Table of Contents

- [Features](#features)
- [System Requirements](#system-requirements)
- [Quick Start](#quick-start)
- [Detailed Setup](#detailed-setup)
- [Project Structure](#project-structure)
- [Usage Guide](#usage-guide)
- [Architecture](#architecture)
- [Configuration](#configuration)
- [Writing Tests](#writing-tests)
- [Reporting](#reporting)
- [CI/CD Integration](#cicd-integration)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

## Features

✅ **Page Object Model (POM)** - Maintainable and reusable UI interaction patterns
✅ **BDD Step Definition Pattern** - Human-readable Given-When-Then test scenarios
✅ **Async/Await** - Full asynchronous test execution with modern C# patterns
✅ **Multi-Browser Support** - Chromium, Firefox, and WebKit browsers
✅ **Comprehensive Logging** - Serilog with file and console output
✅ **Auto-Retry Mechanism** - Flaky test mitigation
✅ **Allure Reporting** - Rich HTML reports with screenshots and video
✅ **CI/CD Ready** - GitHub Actions, GitLab CI, and Azure Pipelines examples
✅ **Configuration Management** - Environment-specific settings (dev, staging, prod)
✅ **API Testing** - RESTful API testing capabilities
✅ **Screenshot on Failure** - Automatic error documentation
✅ **Performance Testing** - Built-in performance metrics and assertions
✅ **Dependency Injection** - Microsoft DI container integration
✅ **Code Analysis** - Static code analysis and formatting tools

## System Requirements

### Minimum Requirements
- **.NET 10.0 SDK** or later
- **4GB RAM** (for parallel test execution)
- **2GB Disk Space** (for browser binaries and logs)
- **Linux** (CachyOS/Arch), macOS, or Windows

### Supported Operating Systems
- Arch Linux / CachyOS (Primary Development)
- Ubuntu 20.04+
- macOS 11+
- Windows 10+

### Additional Tools (Optional)
- **Allure CLI** - For report generation (`pacman -S allure` on Arch)
- **Git** - For version control
- **Docker** - For containerized test execution

## Quick Start

### On Arch Linux / CachyOS

```bash
# Clone or navigate to the lab7 directory
cd lab7

# Make setup script executable
chmod +x setup.sh

# Run full setup
bash setup.sh --setup

# Run tests
make test

# Generate report (optional)
make report
make report-open
```

### Using Make (All platforms)

```bash
# Full setup and run
make all

# Or individual steps
make setup          # Install dependencies
make build          # Build project
make test           # Run tests
make report-open    # Generate and view report
```

## Detailed Setup

### Step 1: Install System Dependencies (Arch Linux)

```bash
sudo pacman -Sy
sudo pacman -S dotnet-sdk dotnet-runtime aspnet-runtime
```

Or use the automated setup script:

```bash
bash setup.sh --install
```

### Step 2: Restore NuGet Dependencies

```bash
make restore
```

### Step 3: Build Project

```bash
make build
```

### Step 4: Run Tests

```bash
make test
```

### Step 5: View Reports

```bash
make report-open
```

## Project Structure

```
lab7/
├── setup.sh                          # Bootstrap script for Arch Linux
├── Makefile                          # Build automation
├── README.md                         # This file
├── CONTRIBUTING.md                  # Contributing guidelines
├──
 PlaywrightTests/
│   ├── PlaywrightTests.csproj       # Project file
│   ├── appsettings.json             # Main configuration
│   │
│   ├── Config/
│   │   ├── ConfigManager.cs         # Configuration loading & management
│   │   └── Environments/
│   │       ├── appsettings.dev.json
│   │       ├── appsettings.staging.json
│   │       └── appsettings.production.json
│   │
│   ├── Core/
│   │   ├── Managers/
│   │   │   ├── BrowserManager.cs     # Playwright lifecycle management
│   │   │   ├── LoggerManager.cs      # Serilog configuration
│   │   │   ├── AllureHelper.cs       # Allure report integration
│   │   │   └── PlaywrightSettingsHelper.cs
│   │   │
│   │   ├── Pages/
│   │   │   └── BasePage.cs           # Base page object class
│   │   │
│   │   ├── Steps/
│   │   │   └── BaseSteps.cs          # BDD step base class
│   │   │
│   │   ├── Helpers/
│   │   │   ├── WaitHelpers.cs        # Wait mechanisms
│   │   │   └── DataHelpers.cs        # Test data utilities
│   │   │
│   │   └── API/
│   │       ├── BaseApiClient.cs      # HTTP client base
│   │       └── ProductApiClient.cs   # Example API client
│   │
│   ├── Pages/
│   │   ├── LoginPage.cs              # Login page object
│   │   ├── HomePage.cs               # Home page object
│   │   └── ProductPage.cs            # Product page object
│   │
│   ├── Tests/
│   │   ├── UI/
│   │   │   ├── BaseTest.cs           # Base test class
│   │   │   ├── LoginTest.cs          # Login tests
│   │   │   └── ProductTest.cs        # Product tests
│   │   │
│   │   └── API/
│   │       ├── BaseApiTest.cs        # Base API test class
│   │       └── ProductApiTest.cs     # Product API tests
│   │
│   ├── Logs/                          # Generated test logs
│   ├── Screenshots/                   # Failed test screenshots
│   └── allure-results/                # Allure report data
│
└── docs/                              # Documentation
    ├── ARCHITECTURE.md
    ├── SETUP.md
    ├── API.md
    └── BEST_PRACTICES.md
```

## Usage Guide

### Running Specific Tests

```bash
# Run all tests
make test

# Run only UI tests
make test-ui

# Run only API tests
make test-api

# Run smoke tests (quick validation)
make test-smoke

# Run tests in parallel
make test-parallel

# Run specific test class
dotnet test --filter "Category=LoginTest"

# Run specific test method
dotnet test --filter "Name~UserCanLoginSuccessfully"
```

### Configuration

Edit environment-specific configuration files:

```json
// PlaywrightTests/Config/Environments/appsettings.dev.json
{
  "Environment": "dev",
  "BaseUrl": "https://dev.example.com",
  "ApiUrl": "https://api-dev.example.com",
  "Credentials": {
    "Username": "testuser",
    "Password": "${TEST_PASSWORD}"  // Use environment variable
  },
  "Features": {
    "DebugMode": true,
    "ScreenshotsOnFailure": true
  }
}
```

### Setting Environment Variables

```bash
# Linux/macOS
export TEST_ENV=dev
export TEST_PASSWORD=your_password

# Windows (PowerShell)
$env:TEST_ENV = "dev"
$env:TEST_PASSWORD = "your_password"

# Run tests with specific environment
TEST_ENV=staging make test
```

## Architecture

### Design Patterns

**Page Object Model (POM)**
- Encapsulates page-specific selectors and interactions
- Reduces maintenance effort when elements change
- Improves code reusability

**Step Definition Pattern**
- Implements Given-When-Then BDD structure
- Makes tests human-readable
- Facilitates collaboration with QA/Business

**Dependency Injection**
- Centralized configuration management
- Loose coupling between components
- Easy testing and mocking

### Layer Architecture

```
┌─────────────────────────────────────────┐
│         Test Layer (@Test)              │  UI & API Tests
├─────────────────────────────────────────┤
│     Page Object / API Client Layer      │  Page/APIClient
├─────────────────────────────────────────┤
│      Step Definition / Base Classes     │  BaseSteps/BaseApiClient
├─────────────────────────────────────────┤
│         Manager / Helper Layer          │  Browser/Logger/Config
├─────────────────────────────────────────┤
│      Playwright / HTTP Client Layer     │  External libraries
└─────────────────────────────────────────┘
```

## Configuration

### Main Configuration (appsettings.json)

```json
{
  "Playwright": {
    "BrowserType": "chromium",
    "HeadlessMode": true,
    "Timeout": 30000,
    "SlowMoDelay": 0,
    "ViewportWidth": 1920,
    "ViewportHeight": 1080
  },
  "Logging": {
    "MinimumLevel": "Information",
    "FilePath": "Logs/test-{Date:yyyy-MM-dd}.log"
  },
  "Allure": {
    "Enabled": true,
    "ResultsDirectory": "allure-results"
  }
}
```

### Environment Variables

- `TEST_ENV` - Environment to run against (dev, staging, production)
- `TEST_PASSWORD` - Password for test accounts
- `TEST_HEADLESS` - Override headless mode
- `TEST_BROWSER` - Override browser type

## Writing Tests

### UI Test Example

```csharp
[TestFixture]
[AllureFeature("Authentication")]
public class LoginTest : BaseTest
{
    [Test]
    [AllureTag("Smoke")]
    [Category("UI")]
    public async Task ValidLoginScenario()
    {
        // Arrange
        var loginPage = new LoginPage(Page, Logger);
        
        // Act
        await loginPage.NavigateAsync(ConfigManager.Environment.BaseUrl);
        await loginPage.OpenLoginModalAsync();
        await loginPage.LoginAsync("testuser", "password");
        
        // Assert
        string message = await loginPage.GetWelcomeMessageAsync();
        Assert.That(message, Does.Contain("testuser"));
    }
}
```

### API Test Example

```csharp
[TestFixture]
[AllureFeature("Products")]
public class ProductApiTest : BaseApiTest
{
    [Test]
    [Category("API")]
    public async Task GetProducts_ReturnsSuccessfulResponse()
    {
        // Arrange & Act
        var response = await ProductApiClient.GetProductsAsync();
        
        // Assert
        Assert.That(response.Success, Is.True);
        Assert.That(response.Data.Count, Is.GreaterThan(0));
    }
}
```

### Creating Custom Page Objects

```csharp
public class CustomPage : BasePage
{
    private readonly string _headerTitle = "h1.page-title";
    
    public CustomPage(IPage page, ILogger logger) : base(page, logger) { }
    
    public async Task VerifyPageLoadedAsync()
    {
        await WaitForElementVisibleAsync(_headerTitle);
    }
}
```

## Reporting

### Allure Reports

Allure provides beautiful, comprehensive HTML reports:

```bash
# Generate report
make report

# Open report in default browser
make report-open

# View report history
allure history allure-report
```

### Report Features

- Test execution timeline
- Test categorization by feature/story
- Screenshot attachments
- Video recordings (when enabled)
- Test result history
- Failure root cause analysis

## CI/CD Integration

### GitHub Actions

See `.github/workflows/tests.yml` for full configuration:

```yaml
name: Test Automation

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0'
      - run: make setup
      - run: make test
      - run: make report
```

### Running Tests in Container

```bash
# Build container
docker build -t playwright-tests .

# Run container
docker run --rm playwright-tests make test
```

## Best Practices

### Test Structure

✅ **One test = One scenario**
✅ **Use AAA pattern** (Arrange, Act, Assert)
✅ **Descriptive test names** - Purpose should be clear
✅ **Avoid test dependencies** - Tests should be independent

### Selectors

✅ **Use stable locators** - ID, data-testid, accessible role
❌ **Avoid fragile selectors** - XPath with position, deep CSS paths

### Waits

✅ **Use explicit waits** - WaitForVisible, WaitForClickable
❌ **Avoid hard waits** - Thread.Sleep()

### Assertions

✅ **Clear assertion messages**
✅ **One assertion per test** (or related assertions)
✅ **Assert on user-visible behavior**

### Error Handling

✅ **Let Playwright handle retries**
✅ **Screenshot on failure**
✅ **Proper error logging**

## Troubleshooting

### Common Issues

**Issue**: Tests fail with "Browser not initialized"  
**Solution**: Ensure `SetUp` is properly decorated with `[SetUp]`

**Issue**: Timeout errors  
**Solution**: Check if element selector is correct, increase timeout if needed

**Issue**: Playwright browser won't start  
**Solution**: Install browsers: `cd PlaywrightTests && dotnet build`

**Issue**: Logs not appearing  
**Solution**: Verify Logs directory exists and has write permissions

### Debug Mode

Enable debug logging:

```json
{
  "Logging": {
    "MinimumLevel": "Debug"
  }
}
```

Or via environment variable:

```bash
export SERILOG_MINIMUM_LEVEL=Debug
make test
```

### Running Single Test with Verbose Output

```bash
dotnet test --filter "Name~SpecificTest" --verbosity detailed --logger "console;verbosity=detailed"
```

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for detailed guidelines on:
- Code style
- Commit messages
- Pull request process
- Test quality standards

### Quick Contribution Steps

1. Fork the repository
2. Create feature branch: `git checkout -b feature/your-feature`
3. Make changes and run tests: `make all`
4. Commit with meaningful messages
5. Push and create Pull Request

## License

This framework is provided as-is for testing and development purposes.

## Support & Resources

- [Playwright Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Allure Reporting](https://qameta.io/allure/)
- [Serilog Documentation](https://serilog.net/)

## Roadmap

- [ ] Mobile testing support (Mobile browsers)
- [ ] Visual testing integration
- [ ] Performance optimization
- [ ] Load testing capabilities
- [ ] Cost analysis reports
- [ ] Test data management system
- [ ] Advanced retry strategies

---

**Version**: 1.0.0  
**Last Updated**: 2026-04-27  
**Maintained By**: QA Engineering Team
