# PlaywrightTests Framework - Complete Setup Guide for Arch Linux

This comprehensive guide walks through setting up the test automation framework on Arch Linux / CachyOS systems.

## System Requirements

### Minimum
- **OS**: Arch Linux, EndeavourOS, or CachyOS
- **Disk Space**: 2GB minimum (more for parallel execution)
- **.NET SDK**: 10.0 LTS or later
- **RAM**: 4GB for parallel testing
- **Processor**: Any modern processor

### Network
- Internet connection for package installation
- Access to NuGet package repositories

## Installation Steps

### Step 1: System Package Installation

```bash
# Update system packages
sudo pacman -Syu

# Install .NET SDK (main component)
sudo pacman -S dotnet-sdk

# Verify installation
dotnet --version
# Expected output: 10.0.x or higher

# (Optional) Install additional tools
sudo pacman -S git
sudo pacman -S allure  # For report generation
```

### Step 2: Framework Setup

```bash
# Navigate to lab7 directory
cd /path/to/lab7

# Make setup script executable
chmod +x setup.sh

# Run complete setup
bash setup.sh --setup

# This will:
# - Check system requirements
# - Create necessary directories
# - Restore NuGet dependencies
# - Build the project
```

### Step 3: Verify Installation

```bash
# Navigate to project directory
cd lab7/PlaywrightTests

# Run a quick test
dotnet test --configuration Release --filter "Category=Smoke"

# Expected output: Tests complete successfully
```

## Troubleshooting

### Issue: .NET SDK Not Found

```bash
#  Solution: Reinstall .NET SDK
sudo pacman -Rs dotnet-sdk
sudo pacman -S dotnet-sdk

# Verify
dotnet --version
```

### Issue: Permission Denied on setup.sh

```bash
# Solution: Add execute permission
chmod +x setup.sh

# Or run with bash explicitly
bash setup.sh --setup
```

### Issue: Package Manager Locked

```bash
# Solution: Wait for other package manager processes
sudo lsof /var/lib/pacman/db.lck
# Or
sudo rm /var/lib/pacman/db.lck
```

### Issue: Out of Disk Space

```bash
# Check disk usage
df -h

# Clean pacman cache
sudo pacman -Sc

# Remove old packages
sudo pacman -Rns $(pacman -Qdtq)
```

## Development Environment Setup

### IDE Setup

#### VS Code (Recommended for .NET)

```bash
# Install VS Code
sudo pacman -S code

# Install C# extension
# Open VS Code → Extensions → Search "C#" → Install

# Install key extensions
# - C# (Microsoft)
# - .NET Runtime (Microsoft)
# - REST Client (Huachao Mao)
# - Thunder Client (parallels)
```

#### Visual Studio for Mac

```bash
# Note: Not available on Arch Linux
# Use VS Code or JetBrains Rider instead
```

#### JetBrains Rider

```bash
# Install via pacman (if available)
sudo pacman -S jetbrains-rider

# Or download from jetbrains.com
```

### Git Configuration

```bash
# Set user identity
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Set default branch name
git config --global init.defaultBranch main

# Configure SSH (optional but recommended)
ssh-keygen -t ed25519 -C "your.email@example.com"
eval "$(ssh-agent -s)"
ssh-add ~/.ssh/id_ed25519
```

## Quick Commands Reference

### Development

```bash
# Navigate to project
cd lab7

# Setup (one-time)
bash setup.sh --setup

# Build project
make build

# Run all tests
make test

# Run specific test category
make test-ui
make test-api
make test-smoke

# Format code
make format

# Generate report
make report-open
```

### CI/CD Pipeline Setup (GitHub)

```bash
# Create .github/workflows directory (already done)
# workflow files are in .github/workflows/

# Push to GitHub to trigger automatic tests
git push origin main
```

### Docker Setup (Optional)

```bash
# Create Dockerfile for containerized tests
# (Framework is Docker-ready)

# Build container
docker build -t playwright-tests:latest .

# Run tests in container
docker run --rm playwright-tests:latest make test
```

## Environment-Specific Setup

### Development Environment

```bash
# Set dev environment
export TEST_ENV=dev
export TEST_PASSWORD=dev_password

# Run tests against dev
make test
```

### Staging Environment

```bash
# Set staging environment
export TEST_ENV=staging
export TEST_PASSWORD=staging_password

# Run tests against staging
make test
```

### Production Environment

```bash
# Caution: Use production carefully!
export TEST_ENV=production
export TEST_PASSWORD=prod_password

# Run smoke tests only
make test-smoke
```

## Performance Tuning

### For Parallel Testing

```bash
# Edit appsettings.json for optimal settings
{
  "Playwright": {
    "Timeout": 30000,
    "SlowMoDelay": 0,  # Disable for performance
    "HeadlessMode": true
  }
}

# Run parallel tests
make test-parallel

# Monitor system resources
watch -n 1 'dotnet build --no-restore'
```

### For Resource-Constrained Systems

```bash
# Reduce parallel execution
dotnet test -p:ParallelizeTestCollections=false

# Use single browser type
export PLAYWRIGHT_BROWSER=chromium
```

## Monitoring and Logging

### View Real-Time Logs

```bash
# Tail log file
tail -f lab7/PlaywrightTests/Logs/test-*.log

# Or use less for pagination
less lab7/PlaywrightTests/Logs/test-*.log

# Monitor with highlighting
grep -i error lab7/PlaywrightTests/Logs/test-*.log
```

### Screenshot Directory

```bash
# Find failed test screenshots
find lab7/PlaywrightTests/Screenshots -name "*.png" -type f

# Open recent screenshot
ls -lt lab7/PlaywrightTests/Screenshots | head -1
```

## Testing Best Practices on Arch

### Browser Installation

Playwright automatically installs browsers, but you can verify:

```bash
# Check installed browsers
ls ~/.cache/ms-playwright/

# Manual browser installation if needed
cd lab7/PlaywrightTests
dotnet build  # This installs browsers
```

### Headless Mode

```bash
# Headless (default, no UI needed)
export PLAYWRIGHT_HEADLESS=true

# With UI (for debugging)
export PLAYWRIGHT_HEADLESS=false

# Run test with UI
make test
```

### Virtual Display (For CI/CD)

```bash
# If running in CI without display
xvfb-run make test

# Install if needed
sudo pacman -S xorg-server-xvfb
```

## Maintenance

### Regular Cleanup

```bash
# Clean build artifacts
make clean-all

# Clear npm/playwright cache
rm -rf ~/.cache/ms-playwright

# Remove logs older than 7 days
find lab7/PlaywrightTests/Logs -name "*.log" -mtime +7 -delete

# Remove screenshots older than 7 days
find lab7/PlaywrightTests/Screenshots -name "*.png" -mtime +7 -delete
```

### Update Dependencies

```bash
# Update .NET SDK
sudo pacman -Syu dotnet-sdk

# Update NuGet dependencies
cd lab7/PlaywrightTests
dotnet restore --force

# Update Framework
cd ..
git pull origin main
bash setup.sh --restore
```

## Integration with CI/CD

### GitHub Actions

Tests automatically run on:
- Push to main/develop
- Pull requests

Workflows located in: `.github/workflows/tests.yml`

### Local CI Testing

```bash
# Simulate CI locally
make all

# This runs:
# 1. Setup
# 2. Build
# 3. Test
# 4. Report generation
```

## Security Considerations

### Credential Management

```bash
# Never commit credentials
git add .gitignore
echo ".env" >> .gitignore

# Use environment variables
export TEST_PASSWORD="$(pass show passwords/test)"

# Or use .env file (not committed)
cat > .env << EOF
TEST_PASSWORD=your_password
EOF

# Load from .env
source .env
make test
```

### Secure Configuration

```bash
# Store in secure location
chmod 600 ~/.config/test-credentials.json

# Reference in code
string password = File.ReadAllText(Path.Expand("~/.config/test-credentials.json"));
```

## Troubleshooting Common Issues

### Browser Not Launching

```bash
# Solution 1: Reinstall browsers
rm -rf ~/.cache/ms-playwright
cd lab7/PlaywrightTests
dotnet build

# Solution 2: Check dependencies
ldd ~/.cache/ms-playwright/chromium-*/chrome-linux/chrome

# Solution 3: Install missing libraries
sudo pacman -S libxss1 libgconf-2-4
```

### Timeout Errors

```bash
# Increase timeout in appsettings.json
{
  "Playwright": {
    "Timeout": 60000  // increased from 30000
  }
}

# Or export environment variable
export PLAYWRIGHT_TIMEOUT=60000
```

### Memory Issues

```bash
# Run tests one at a time
dotnet test --maxParallelThreads 1

# Monitor memory usage
watch -n 1 free -h

# Reduce parallel browsers
export CI=true  # CI mode runs serially
```

## Getting Help

### Online Resources

- [Playwright .NET Documentation](https://playwright.dev/dotnet/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Arch Linux Wiki](https://wiki.archlinux.org/)
- [.NET SDK on Arch](https://wiki.archlinux.org/title/.NET)

### Local Help

```bash
# Get command help
make help

# Script help
bash setup.sh --help

# .NET help
dotnet help
```

### Reporting Issues

When reporting issues, include:
- OS version: `uname -a`
- .NET version: `dotnet --version`
- Arch Linux version: `pacman -V`
- Error logs: `tail -100 lab7/PlaywrightTests/Logs/test-*.log`
- Screenshot (if available)

---

**Setup Version**: 1.0  
**Last Updated**: 2026-04-27  
**Tested on**: CachyOS (Arch-based)
