# Contributing to PlaywrightTests Framework

Thank you for interest in contributing to this test automation framework! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Maintain professional and respectful communication
- Focus on code quality and test reliability
- Help other contributors succeed
- Report issues constructively

## Getting Started

### 1. Set Up Development Environment

```bash
# Clone repository
git clone <repository-url>
cd lab7

# Run complete setup
bash setup.sh --setup

# Verify installation
make test-smoke
```

### 2. Create Feature Branch

```bash
git checkout -b feature/feature-name
# or
git checkout -b fix/issue-name
```

### 3. Make Changes

See [Code Style Guide](#code-style-guide) for standards.

### 4. Test Your Changes

```bash
# Run all tests
make test

# Run specific test category
make test-ui
make test-api

# Run with code analysis
make lint
make format

# Generate report
make report-open
```

### 5. Commit Changes

```bash
git add .
git commit -m "type(scope): description"
```

See [Commit Message Format](#commit-message-format) for details.

### 6. Push and Create Pull Request

```bash
git push origin feature/feature-name
```

Then create a Pull Request on GitHub with detailed description.

## Code Style Guide

### C# Conventions

```csharp
// ✅ Good: Clear, descriptive names
public async Task LoginWithValidCredentialsAsync()
{
    var loginPage = new LoginPage(Page, Logger);
    await loginPage.LoginAsync("test@example.com", "password");
}

// ❌ Bad: Unclear names
public async Task Test1()
{
    var p = new LoginPage(Page, Logger);
    await p.Login("test@example.com", "password");
}
```

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `LoginPage`, `ProductApiClient` |
| Methods | PascalCase | `LoginAsync()`, `GetUserIdAsync()` |
| Properties | PascalCase | `UserName` |
| Private fields | _camelCase | `_logger`, `_page` |
| Constants | UPPER_SNAKE_CASE | `DEFAULT_TIMEOUT` |
| Local variables | camelCase | `userName`, `productId` |

### Async/Await

- Always use async/await for I/O operations
- Method names should end with `Async`
- Never use `.Wait()` or `.Result`

```csharp
// ✅ Good
public async Task ClickLoginButtonAsync()
{
    await _page.Locator(_loginButton).ClickAsync();
}

// ❌ Bad
public void ClickLoginButton()
{
    _page.Locator(_loginButton).ClickAsync().Wait();
}
```

### Logging

- Log meaningful information
- Use appropriate log levels
- Don't log sensitive data

```csharp
// ✅ Good
Logger.Information("Logging in with username: {Username}", username);
Logger.Error(ex, "Login failed for user: {Username}", username);

// ❌ Bad
Logger.Information("Logging in");
Logger.Error(ex, "Login failed with password: {Password}", password);
```

### Comments & Documentation

```csharp
/// <summary>
/// Performs login with provided credentials.
/// Waits for success/failure response before returning.
/// </summary>
public async Task LoginAsync(string username, string password)
{
    // Implementation
}
```

### AccessModifiers

- Use appropriate access modifiers
- Default to `private` unless there's a reason for `public`
- Exposed public methods in Page Objects and API Clients only

```csharp
public class LoginPage : BasePage
{
    // Public: Part of page object interface
    public async Task LoginAsync(string username, string password)
    {
        await FillUsernameAsync(username);
        await FillPasswordAsync(password);
        await ClickLoginAsync();
    }

    // Private: Implementation detail
    private async Task FillUsernameAsync(string username)
    {
        await _page.Locator(_usernameInput).FillAsync(username);
    }
}
```

## Commit Message Format

Follow conventional commit format:

```
type(scope): subject

body

footer
```

### Type

- `feat` - New feature
- `fix` - Bug fix
- `test` - Test additions/modifications
- `refactor` - Code refactoring
- `docs` - Documentation
- `style` - Code style changes
- `chore` - Build, dependencies, etc.
- `perf` - Performance improvements

### Scope

- Area of codebase affected
- Examples: `core`, `pages`, `api`, `config`

### Examples

```
feat(pages): add ProductPage object with filtering functionality

fix(core): correct timeout calculation in WaitHelpers

test(ui): add ProductTest suite with multiple scenarios

docs(readme): update setup instructions for Arch Linux

refactor(core): simplify retry logic in BrowserManager
```

## Test Writing Guidelines

### Test Structure (AAA Pattern)

```csharp
[Test]
public async Task TestName_Scenario_ExpectedOutcome()
{
    // Arrange - Setup preconditions
    var page = new LoginPage(Page, Logger);
    
    // Act - Execute test steps
    await page.NavigateAsync(BaseUrl);
    await page.LoginAsync(username, password);
    
    // Assert - Verify results
    string message = await page.GetWelcomeMessageAsync();
    Assert.That(message, Does.Contain(username));
}
```

### Naming Convention for Tests

```
TestName_Scenario_ExpectedResult

Examples:
- UserCanLoginSuccessfully_ValidCredentials_WelcomeMessageDisplayed
- LoginFailsWithInvalidCredentials_WrongPassword_ErrorMessageShown
- ProductCanBeAddedToCart_OneItemAvailable_CartUpdated
```

### Test Requirements

- ✅ Independent from other tests
- ✅ Descriptive assertion messages
- ✅ Proper setup and teardown
- ✅ Single responsibility (test one behavior)
- ✅ Reproducible results
- ✅ No hardcoded waits

### Page Object Requirements

- ✅ Encapsulate selectors as private fields
- ✅ Provide public methods for user actions
- ✅ Inherit from `BasePage`
- ✅ Include comprehensive logging
- ✅ Use Wait methods from BasePage
- ✅ Proper error handling

```csharp
public class CustomPage : BasePage
{
    // Private selectors
    private readonly string _headerTitle = "h1.title";
    private readonly string _submitButton = "button[type='submit']";
    
    public CustomPage(IPage page, ILogger logger) : base(page, logger)
    {
    }
    
    // Public methods
    public async Task VerifyPageLoadedAsync()
    {
        Logger.Information("Verifying page loaded");
        await WaitForElementVisibleAsync(_headerTitle);
    }
    
    public async Task SubmitFormAsync()
    {
        Logger.Information("Submitting form");
        await ClickAsync(_submitButton);
    }
}
```

### API Client Requirements

- ✅ Inherit from `BaseApiClient`
- ✅ Type-safe response handling
- ✅ Comprehensive logging
- ✅ Proper error handling
- ✅ Token/auth management

## Pull Request Process

### PR Title Format

```
[TYPE] Description

Examples:
[FEATURE] Add ProductPage object with filtering
[FIX] Resolve timeout issue in WaitHelpers
[TEST] Add comprehensive LoginTest suite
```

### PR Description Template

```markdown
## Description
Brief description of changes.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring
- [ ] Test additions

## Related Issue
Closes #123

## Changes Made
- Detailed bullet points of changes
- Impact on existing functionality
- New methods/classes introduced

## Testing Performed
- Manual testing steps
- Test cases added/modified
- Browsers tested (Chromium, Firefox, WebKit)
- Environments tested (dev, staging)

## Checklist
- [ ] Code follows style guide
- [ ] Comments added for complex logic
- [ ] Tests written and passing
- [ ] No breaking changes
- [ ] Documentation updated
- [ ] No sensitive data committed
- [ ] Tested on supported platforms
```

## Code Review Checklist

Reviewers should verify:

- [ ] Code follows style guide
- [ ] Tests are comprehensive and passing
- [ ] No code duplication
- [ ] Proper error handling
- [ ] No security vulnerabilities
- [ ] Performance acceptable
- [ ] Documentation complete
- [ ] Commit messages clear

## Testing Requirements

### Test Coverage

- **Target**: >70% code coverage for new code
- **Utils/Helpers**: >90% coverage
- **Core Libraries**: 100% coverage

### Run Before Submit

```bash
# Format code
make format

# Run linter
make lint

# Run all tests
make test

# Generate report
make report

# Verify no breaking changes
make test-smoke
```

## Documentation

### When to Update Docs

- [ ] Adding new features
- [ ] Changing existing behavior
- [ ] Adding new Page Object or API Client
- [ ] Fixing bugs related to docs
- [ ] Updating dependencies

### Documentation Files

- `README.md` - Overview and quick start
- `CONTRIBUTING.md` - This file
- `docs/ARCHITECTURE.md` - System design
- `docs/SETUP.md` - Detailed setup
- `docs/API.md` - API testing guide
- `docs/BEST_PRACTICES.md` - Testing best practices

### Documentation Standards

- Keep language clear and concise
- Include code examples
- Use consistent formatting
- Update table of contents
- Include links to related docs

## Reporting Issues

### Bug Report Template

```markdown
## Description
Clear description of the issue.

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Expected Behavior
What should happen.

## Actual Behavior
What actually happens.

## Environment
- OS: Windows/Linux/macOS
- .NET Version: 10.0
- Browser: Chromium/Firefox/WebKit
- Framework Version: 1.0.0

## Logs
(Attach relevant log files)
```

### Feature Request Template

```markdown
## Description
What feature or improvement is needed.

## Use Case
Why is this needed.

## Proposed Solution
How could this be implemented.

## Alternative Solutions
Other approaches considered.
```

## Development Workflow

```
1. Create issue or pick existing
2. Create feature branch from main
3. Make changes
4. Write/update tests
5. Run full test suite
6. Update documentation
7. Create Pull Request
8. Address code review comments
9. Merge after approval
10. Delete feature branch
```

## Questions or Need Help?

- Review existing documentation
- Check closed issues/discussions
- Create a new issue with question label
- Contact maintainers

## Performance Considerations

- Avoid nested waits
- Use proper timeouts
- Don't create unnecessary browser instances
- Cache test data when appropriate
- Use data providers for parameterized tests

## Security

- Never commit secrets (API keys, passwords)
- Use environment variables for sensitive data
- Sanitize logs for sensitive information
- Report security issues privately
- Keep dependencies updated

## Version Control Best Practices

- Commit frequently with meaningful messages
- Push to remote regularly
- Keep branches up-to-date with main
- Use meaningful branch names
- Delete merged branches
- Never rewrite published history

---

Thank you for contributing! Your efforts help make this framework better for everyone.
