# PlaywrightTests - .NET NUnit Playwright Automation Framework

A comprehensive test automation framework built with .NET, NUnit, Playwright, and Serilog for UI and API testing.

## 📋 Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running Tests](#running-tests)
- [Project Structure](#project-structure)

## ✨ Features

- **Page Object Model (POM)** pattern for maintainable test code
- **BDD-style Step Definitions** with BaseSteps class
- **Multi-environment support** (dev, staging, production)
- **Comprehensive logging** with Serilog to file and console
- **Allure reporting** integration for detailed test reports
- **Automatic retry** mechanism for flaky tests
- **Wait helpers** for robust element interaction
- **Data management** utilities for test data handling
- **Screenshot on failure** for debugging
- **Async/await** support throughout

## 🏗️ Architecture

### Layered Architecture

```
Tests
├── UI Tests
└── API Tests

Pages (Page Object Model)
├── BasePage (Core functionality)
├── LoginPage (Example)
└── ...

Core Managers
├── BrowserManager (Playwright lifecycle)
├── ConfigManager (Configuration management)
├── LoggerManager (Serilog integration)
└── AllureHelper (Allure reporting)

Helpers
├── WaitHelpers (Element waiting, retry)
├── DataHelpers (Test data management)
└── PlaywrightSettingsHelper (Playwright configuration)

Configuration
├── appsettings.json (Main config)
└── Config/Environments/
    ├── appsettings.dev.json
    ├── appsettings.staging.json
    └── appsettings.production.json
```

## 📦 Prerequisites

- **.NET 10.0 SDK** or higher
- **Windows, Linux, or macOS**
- **4GB RAM** (minimum)
- **2GB disk space**

## 🚀 Installation

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd PlaywrightTests
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Install Playwright browsers:**
   ```bash
   dotnet playwright install
   ```

4. **Build the project:**
   ```bash
   dotnet build
   ```

## ⚙️ Configuration

### Set Environment

```bash
# Linux/macOS
export TEST_ENV=dev

# Windows
set TEST_ENV=dev
```

Supported: `dev`, `staging`, `production`

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run by category
dotnet test --filter "Category=Smoke"

# Run in parallel
dotnet test -- --workers=4

# Generate Allure report
allure generate allure-results -o allure-report -c
allure open allure-report
```

## 📁 Project Structure

```
PlaywrightTests/
├── appsettings.json
├── Config/
│   ├── ConfigManager.cs
│   └── Environments/
├── Core/
│   ├── Managers/
│   ├── Pages/
│   ├── Steps/
│   └── Helpers/
├── Pages/
├── Tests/
│   ├── UI/
│   └── API/
├── Logs/
├── Screenshots/
└── allure-results/
```

## ✅ Success Criteria

Framework is ready when:
- ✅ All dependencies installed
- ✅ Configuration works for all environments
- ✅ Logging works to file and console
- ✅ Allure reports generate correctly
- ✅ Tests run with retry mechanism
- ✅ Screenshots captured on failure
- ✅ CI/CD pipeline configured
- ✅ Documentation complete

## 📝 Logging

Logs are created in `Logs/` directory with daily rotation.

Log format: `[Timestamp] [Level] [Machine] [User] [ThreadId] Message`

## 🔧 Troubleshooting

- **Tests timing out**: Increase `Playwright.Timeout` in config
- **Flaky tests**: Use `[Retry(3)]` attribute
- **Browser crashes**: Run `dotnet playwright install`
- **Logs not appearing**: Check `Logs/` directory permissions

For more information, see detailed README at project root.
