# Framework Index & Summary

## 📦 Framework Overview

The **PlaywrightTests** framework is a comprehensive, enterprise-grade test automation solution for .NET applications. It combines Page Object Model (POM) architecture, BDD patterns, and industry best practices.

**Version**: 1.0.0  
**.NET Version**: 10.0 LTS  
**Last Updated**: 2026-04-27

## 📋 Quick Navigation

### Getting Started
- **First Time?** → Start with [README.md](../README.md)
- **Setup on Arch Linux?** → See [docs/SETUP.md](SETUP.md)
- **Want to Contribute?** → Read [CONTRIBUTING.md](../CONTRIBUTING.md)

### Deep Dives
- **Understand Architecture** → [docs/ARCHITECTURE.md](ARCHITECTURE.md)
- **Learn API Testing** → [docs/API.md](API.md)
- **Best Practices** → [docs/BEST_PRACTICES.md](BEST_PRACTICES.md)

## 📁 Directory Structure

```
lab7/
├── 📄 README.md                        # Main documentation
├── 📄 CONTRIBUTING.md                  # Contributing guide
├── 📄 Makefile                         # Build automation (make)
├── 📄 setup.sh                         # Bootstrap script (Arch Linux)
├── 📄 .gitignore                       # Git ignore rules
│
├── docs/                               # Documentation
│   ├── SETUP.md                        # Detailed Arch Linux setup
│   ├── ARCHITECTURE.md                 # System design & patterns
│   ├── API.md                          # API testing guide
│   ├── BEST_PRACTICES.md               # Testing best practices
│   └── INDEX.md                        # This file
│
├── PlaywrightTests/
│   ├── PlaywrightTests.csproj         # Project file
│   ├── appsettings.json               # Main configuration
│   │
│   ├── Config/
│   │   ├── ConfigManager.cs            # Configuration manager
│   │   └── Environments/
│   │       ├── appsettings.dev.json
│   │       ├── appsettings.staging.json
│   │       └── appsettings.production.json
│   │
│   ├── Core/
│   │   ├── Managers/
│   │   │   ├── BrowserManager.cs
│   │   │   ├── LoggerManager.cs
│   │   │   ├── AllureHelper.cs
│   │   │   └── PlaywrightSettingsHelper.cs
│   │   │
│   │   ├── Pages/
│   │   │   └── BasePage.cs
│   │   │
│   │   ├── Steps/
│   │   │   └── BaseSteps.cs
│   │   │
│   │   ├── Helpers/
│   │   │   ├── WaitHelpers.cs
│   │   │   └── DataHelpers.cs
│   │   │
│   │   └── API/
│   │       ├── BaseApiClient.cs
│   │       └── ProductApiClient.cs
│   │
│   ├── Pages/
│   │   ├── BasePage.cs                 # Base page object (✓ done)
│   │   ├── LoginPage.cs                # Login page object (✓ done)
│   │   ├── HomePage.cs                 # Home page object (✓ done)
│   │   └── ProductPage.cs              # Product page object (✓ done)
│   │
│   ├── Tests/
│   │   ├── UI/
│   │   │   ├── BaseTest.cs             # Base test class
│   │   │   ├── LoginTest.cs            # Login tests
│   │   │   └── ProductTest.cs          # Product tests
│   │   │
│   │   └── API/
│   │       ├── BaseApiTest.cs          # Base API test class
│   │       └── ProductApiTest.cs       # Product API tests
│   │
│   ├── Logs/                           # Test execution logs
│   ├── Screenshots/                    # Failed test screenshots
│   └── allure-results/                 # Allure report data
│
└── .github/
    └── workflows/
        └── tests.yml                    # GitHub Actions pipeline
```

## 🚀 Quick Start Commands

```bash
# One-time setup
cd lab7
bash setup.sh --setup

# Run tests
make test                  # All tests
make test-ui              # UI tests only
make test-api             # API tests only
make test-smoke           # Quick validation
make test-parallel        # Parallel execution

# Build & Quality
make build                # Build project
make clean                # Clean artifacts
make format               # Format code
make lint                 # Code analysis

# Reporting
make report               # Generate report
make report-open          # Generate & view

# Help
make help                 # Show all commands
```

## 🏗️ Framework Components

### Core Components

| Component | Purpose | Location |
|-----------|---------|----------|
| **BrowserManager** | Playwright lifecycle | `Core/Managers/` |
| **ConfigManager** | Configuration loading | `Config/` |
| **LoggerManager** | Serilog configuration | `Core/Managers/` |
| **BasePage** | Page object base class | `Core/Pages/` |
| **BaseSteps** | BDD step base class | `Core/Steps/` |
| **BaseApiClient** | HTTP client base | `Core/API/` |
| **WaitHelpers** | Wait utilities | `Core/Helpers/` |
| **DataHelpers** | Test data utilities | `Core/Helpers/` |

### Page Objects

| Page | Description | Status |
|------|-------------|--------|
| LoginPage | Authentication & login | ✅ Complete |
| HomePage | Home page navigation | ✅ Complete |
| ProductPage | Product listing & details | ✅ Complete |

### Test Suites

| Test | Type | Status | Categories |
|------|------|--------|------------|
| LoginTest | UI | ✅ Complete | Smoke, Regression |
| ProductTest | UI | ✅ Complete | Smoke, Regression |
| ProductApiTest | API | ✅ Complete | Smoke, Regression |

## 📚 Documentation Files

### Root Documentation
- **README.md** - Overview, features, usage guide
- **CONTRIBUTING.md** - Contribution guidelines, code style
- **.gitignore** - Git ignore patterns
- **Makefile** - Build automation targets
- **setup.sh** - Bootstrap script for Arch Linux

### /docs/ Folder
- **SETUP.md** - Detailed setup guide for Arch Linux
- **ARCHITECTURE.md** - System design, patterns, layers
- **API.md** - API testing guide with examples
- **BEST_PRACTICES.md** - Testing best practices
- **INDEX.md** - This file

## ✨ Key Features

✅ **Page Object Model** - Maintainable UI tests  
✅ **BDD Pattern** - Human-readable test structure  
✅ **Async/Await** - Modern C# patterns  
✅ **Multi-Browser** - Chromium, Firefox, WebKit  
✅ **Logging** - Serilog with file & console  
✅ **Screenshots** - Automatic on failure  
✅ **Allure Reports** - Beautiful HTML reports  
✅ **API Testing** - RESTful endpoint testing  
✅ **Retry Logic** - Flaky test handling  
✅ **Configuration** - Environment management  
✅ **CI/CD Ready** - GitHub Actions included  
✅ **Docker Support** - Containerized testing  

## 🔧 Configuration

### Main Settings (appsettings.json)

```json
{
  "Playwright": {
    "BrowserType": "chromium",
    "HeadlessMode": true,
    "Timeout": 30000
  },
  "Logging": {
    "MinimumLevel": "Information"
  },
  "Allure": {
    "Enabled": true,
    "ResultsDirectory": "allure-results"
  }
}
```

### Environment Variables

- `TEST_ENV` - Environment (dev, staging, production)
- `TEST_PASSWORD` - Test account password
- `TEST_HEADLESS` - Headless mode override
- `TEST_BROWSER` - Browser type override

## 🧪 Test Categories

- **Smoke** - Quick validation (~5 min)
- **Regression** - Full test suite (~30 min)
- **Critical** - High-priority tests
- **UI** - Browser-based tests
- **API** - REST endpoint tests
- **Performance** - Speed & load tests

Run by category:
```bash
dotnet test --filter "Category=Smoke"
dotnet test --filter "Category=UI"
dotnet test --filter "Category=API"
```

## 📊 Reporting

### Allure Reports

```bash
# Generate report
make report

# Open in browser
make report-open

# Report includes:
# - Test execution timeline
# - Pass/fail distribution
# - Screenshot attachments
# - Execution history
# - Failure analysis
```

### Log Files

Logs stored in: `PlaywrightTests/Logs/test-YYYY-MM-DD.log`

Includes:
- Timestamp
- Log level (Info, Warning, Error)
- Machine name
- Thread ID
- Message
- Exception details

## 🔐 Security

- ✅ Credentials in environment variables
- ✅ Password masking in logs
- ✅ No secrets in git
- ✅ Secure configuration files

## 🎓 Learning Path

### Beginner
1. Read [README.md](../README.md) - Overview
2. Run [docs/SETUP.md](SETUP.md) - Setup on Arch
3. Run tests locally - `make test-smoke`
4. View reports - `make report-open`

### Intermediate
1. Study [docs/ARCHITECTURE.md](ARCHITECTURE.md) - Design
2. Create custom page objects
3. Write new test scenarios
4. Read [docs/BEST_PRACTICES.md](BEST_PRACTICES.md)

### Advanced
1. Build custom API clients
2. Implement advanced retry logic
3. Performance testing
4. CI/CD pipeline customization

## 🤝 Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for:
- Code style guide
- Commit message format
- Pull request process
- Test requirements
- Documentation standards

## ❓ FAQ

### Q: How do I run a specific test?
**A:** `dotnet test --filter "Name~TestName"`

### Q: How do I add a new page object?
**A:** Create class in `Pages/`, inherit from `BasePage`, define selectors and methods

### Q: How do I debug a failing test?
**A:** Check logs in `Logs/`, screenshots in `Screenshots/`, increase log level to Debug

### Q: How do I run tests in CI/CD?
**A:** Push to main/develop, GitHub Actions runs automatically

### Q: How do I use different environments?
**A:** Set `TEST_ENV=staging make test`

## 📞 Support & Resources

### Documentation
- [Playwright Docs](https://playwright.dev/dotnet/)
- [NUnit Guide](https://docs.nunit.org/)
- [Serilog Wiki](https://github.com/serilog/serilog/wiki)
- [Allure Docs](https://docs.qameta.io/allure/)

### Commands
- `make help` - All make targets
- `bash setup.sh --help` - Setup options
- `dotnet --help` - .NET CLI help

### Troubleshooting
- Check logs: `tail -f PlaywrightTests/Logs/test-*.log`
- Screenshots: `PlaywrightTests/Screenshots/`
- System info: `uname -a`, `dotnet --version`

## 📈 Roadmap

- [ ] Mobile testing support
- [ ] Visual testing integration
- [ ] Advanced load testing
- [ ] Test data management system
- [ ] Cost analysis reports
- [ ] Machine learning-based flaky test detection

## 📝 Version History

### v1.0.0 (2026-04-27)
- ✅ Initial release
- ✅ Core framework complete
- ✅ All documentation
- ✅ CI/CD pipeline
- ✅ Setup script for Arch Linux

## 📄 License

This framework is provided as-is for testing purposes.

## 🙏 Acknowledgments

- Built with [Playwright](https://playwright.dev/)
- Tested with [NUnit](https://nunit.org/)
- Reported with [Allure](https://qameta.io/allure/)
- Logged with [Serilog](https://serilog.net/)

---

**Last Updated**: 2026-04-27  
**Framework Version**: 1.0.0  
**Maintained By**: QA Engineering Team

For questions or issues, refer to the documentation or create an issue on GitHub.
