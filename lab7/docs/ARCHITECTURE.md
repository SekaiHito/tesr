# Framework Architecture

## Overview

The PlaywrightTests framework is built on a layered architecture that separates concerns and promotes maintainability, reusability, and extensibility.

## Architecture Layers

```
┌───────────────────────────────────────────────────────────────┐
│                    Test Layer (UI/API Tests)                  │
│         [LoginTest] [ProductTest] [ProductApiTest]            │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│            Page Object / API Client Layer                     │
│  [LoginPage] [ProductPage] [ProductApiClient] [HomePageAPI]   │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│        Step Definition / Base Classes Layer                   │
│           [BaseSteps] [BasePage] [BaseApiClient]              │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│      Manager & Helper Layer                                   │
│ [BrowserManager] [ConfigManager] [LoggerManager] [WaitHelpers]│
│ [DataHelpers] [AllureHelper] [PlaywrightSettingsHelper]       │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│        External Libraries & Frameworks                        │
│      [Playwright] [Serilog] [NUnit] [Allure] [HttpClient]     │
└───────────────────────────────────────────────────────────────┘
```

## Component Breakdown

### 1. Test Layer

**Responsibility**: Define test scenarios and assertions

- **UI Tests** (`Tests/UI/`): Browser-based UI test automation
- **API Tests** (`Tests/API/`): REST API endpoint testing
- **Inheritance**: `BaseTest` (UI) or `BaseApiTest` (API)
- **Pattern**: AAA (Arrange-Act-Assert)
- **Attributes**: NUnit, Allure metadata

**Example Flow**:
```
Test → Page Object → Base Page → Playwright
```

### 2. Page Object / API Client Layer

**Responsibility**: Encapsulate page interactions or API calls

- **Page Objects** (`Pages/`): UI element selectors and interactions
  - `LoginPage` - Login modal and form
  - `ProductPage` - Product listing and details
  - `HomePage` - Navigation and main content

- **API Clients** (`Core/API/`): HTTP request handling
  - `ProductApiClient` - Product REST endpoints
  - `BaseApiClient` - Common HTTP methods

**Key Principles**:
- One class per page/API resource
- Private selectors/endpoints
- Public action methods
- Clear naming conventions
- Comprehensive logging

**Example**:
```csharp
public class LoginPage : BasePage
{
    private readonly string _usernameInput = "#username";
    
    public async Task LoginAsync(string username, string password)
    {
        await FillAsync(_usernameInput, username);
        // ... more actions
    }
}
```

### 3. Step Definition / Base Classes Layer

**Responsibility**: Provide base functionality and BDD structure

- **BasePage**: Common element operations
  - Click, Fill, Navigate, WaitFor
  - Screenshot on error
  - Error handling and retry

- **BaseSteps**: BDD-style methods
  - Given (setup preconditions)
  - When (perform actions)
  - Then (verify results)

- **BaseApiClient**: HTTP operations
  - GET, POST, PUT, DELETE
  - Response handling
  - Error management

**Key Features**:
- Async/await pattern
- Comprehensive logging
- Timeout management
- Retry mechanisms
- Screenshot capabilities

### 4. Manager & Helper Layer

**Responsibility**: Infrastructure and utilities

- **BrowserManager**
  - Playwright lifecycle
  - Browser initialization
  - Context management
  - Screenshot capture

- **ConfigManager**
  - Configuration loading
  - Environment management
  - Settings binding
  - Caching

- **LoggerManager**
  - Serilog initialization
  - Multi-level logging
  - Async output
  - Context enrichment

- **WaitHelpers**
  - Element waits
  - Navigation waits
  - Custom waits
  - Retry logic

- **DataHelpers**
  - Test data generation
  - Caching
  - Validation
  - Masking sensitive data

- **AllureHelper**
  - Report initialization
  - Results directory management
  - File attachment

- **PlaywrightSettingsHelper**
  - Configuration conversion
  - Browser options creation
  - Viewport management

### 5. External Libraries Layer

- **Playwright**: Browser automation
- **Serilog**: Structured logging
- **NUnit**: Test framework & assertions
- **Allure**: Report generation
- **HttpClient**: HTTP communication
- **System.Text.Json**: JSON serialization

## Design Patterns Used

### 1. Page Object Model (POM)

Encapsulates page-specific selectors and interactions in dedicated classes.

**Benefits**:
- Maintainability - Changes in selectors only affect page object
- Reusability - Methods reused across tests
- Readability - Tests read like user stories

**Example**:
```csharp
// Page Object
public class LoginPage : BasePage
{
    private readonly string _loginButton = "#login";
    public async Task LoginAsync(string user, string pass) { ... }
}

// Test
[Test]
public async Task UserLogin()
{
    var login = new LoginPage(Page, Logger);
    await login.LoginAsync("user", "pass");
}
```

### 2. BDD Step Definition

Implements Given-When-Then structure for readable tests.

**Structure**:
- Given (Given...) - Setup test preconditions
- When (When...) - Perform user actions
- Then (Then...) - Verify expected results

**Example**:
```csharp
[Test]
public async Task UserLoginWorkflow()
{
    // Given
    await steps.GivenUserNavigatesToAsync(url);
    
    // When
    await steps.WhenUserClicksAsync(loginButton);
    await steps.WhenUserEntersTextAsync(username, "test");
    
    // Then
    await steps.ThenElementShouldBeVisibleAsync(dashboard);
}
```

### 3. Dependency Injection

Injects dependencies to reduce coupling and improve testability.

**Components**:
- Logger instance
- Page instance
- Configuration

**Benefits**:
- Loose coupling
- Easy mocking
- Configuration flexibility
- Centralized initialization

### 4. Repository Pattern (Data Cache)

DataHelpers implements data caching for test data reuse.

```csharp
var dataHelpers = new DataHelpers(logger);
dataHelpers.CacheData("user_id", "12345");
var userId = dataHelpers.GetCachedData("user_id");
```

### 5. Manager Pattern

Managers (BrowserManager, ConfigManager, LoggerManager) handle lifecycle and configuration.

**Advantages**:
- Centralized initialization
- Consistent lifecycle management
- Easy testing setup/teardown
- Error handling

## Configuration Management

```
┌─────────────────────────────────────────┐
│    appsettings.json (Global)            │
│  - Playwright settings               |
│  - Logging configuration             |
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Environment Files (Overrides)           │
│  - appsettings.dev.json              |
│  - appsettings.staging.json          |
│  - appsettings.production.json       |
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Environment Variables (Override All)    │
│  - TEST_ENV                          |
│  - TEST_PASSWORD                     |
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│   ConfigManager (Centralized Access)    │
│  - Caches configuration              |
│  - Provides typed access             |
└─────────────────────────────────────────┘
```

## Logging Flow

```
Test Execution
    ↓
[BDD Step] → [Page Object Method] → [Playwright Action]
    ↓
LoggerManager (Serilog)
    ↓
├─→ Console Output (Real-time)
├─→ Log File (Logs/test-YYYY-MM-DD.log)
└─→ Daily Rotation & Retention
```

## Error Handling & Retry Flow

```
Test Action
    ↓
Try Execute
    ↓
Success? ──Y──→ [Continue]
    ↓ N
Catch Exception
    ↓
Screenshot (if enabled)
    ↓
Log Error Details
    ↓
Throw Exception
    ↓
NUnit catches and marks test as Failed
```

## Test Result Flow

```
Test Execution
    ↓
├─→ Result (Passed/Failed)
├─→ Details (Duration, Output)
├─→ AttachmentS (Screenshots)
└─→ Logging
    ↓
NUnit Adapter
    ↓
├─→ XML Output
└─→ Console Output
    ↓
Allure NUnit Plugin
    ↓
├─→ allure-results/ (JSON)
└─→ Metadata (Features, Tags)
    ↓
Allure Generate
    ↓
allure-report/ (HTML)
```

## Extension Points

### Adding New Page Object

1. Create class inheriting from `BasePage`
2. Define private selectors
3. Implement public action methods
4. Use WaitHelpers for reliability
5. Add logging

```csharp
public class NewPage : BasePage
{
    private readonly string _element = ".selector";
    
    public NewPage(IPage page, ILogger logger) : base(page, logger) { }
    
    public async Task DoSomethingAsync()
    {
        Logger.Information("Doing something");
        await ClickAsync(_element);
    }
}
```

### Adding New API Client

1. Create class inheriting from `BaseApiClient`
2. Define API endpoints as methods
3. Use generic GET/POST/PUT/DELETE methods
4. Define DTOs for requests/responses

```csharp
public class UserApiClient : BaseApiClient
{
    public UserApiClient(ILogger logger) : base(logger) { }
    
    public async Task<ApiResponse<User>> GetUserAsync(int id)
    {
        return await GetAsync<ApiResponse<User>>($"users/{id}");
    }
}
```

### Adding New Manager/Helper

1. Implement initialization logic
2. Provide static or instance access
3. Handle errors gracefully
4. Add comprehensive logging

```csharp
public static class CustomManager
{
    public static void Initialize()
    {
        // Setup logic
    }
    
    public static string GetValue()
    {
        // Return value
    }
}
```

## Performance Considerations

### Optimization Techniques

1. **Parallel Test Execution**
   ```bash
   dotnet test --parallel
   ```

2. **Browser Context Reuse**
   - Share context across tests in same fixture

3. **Smart Waits**
   - Use specific wait conditions
   - Avoid global waits

4. **Connection Pooling**
   - HTTP client reuse for API tests
   - Browser context reuse

## Security Considerations

1. **Credential Management**
   - Use environment variables
   - Never hardcode passwords

2. **Log Sanitization**
   - Mask sensitive data
   - Use DataHelpers.MaskSensitiveData()

3. **Configuration Secrets**
   - Store in secure location
   - Use file permissions

## Scalability

The framework supports:
- **Horizontal Scaling**: Parallel test distribution
- **Vertical Scaling**: Multi-threaded test execution
- **CI/CD Integration**: GitHub Actions, Azure Pipelines, GitLab CI
- **Container Deployment**: Docker support
- **Cloud Execution**: AWS, Azure, GCP compatible

---

**Revision**: 1.0  
**Last Updated**: 2026-04-27
