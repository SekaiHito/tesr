.PHONY: help install restore build clean test report setup all clean-all format lint

# Colors for output
BLUE := \033[0;34m
GREEN := \033[0;32m
RED := \033[0;31m
YELLOW := \033[1;33m
NC := \033[0m

# Project variables
PROJECT_NAME := PlaywrightTests
PROJECT_DIR := $(PROJECT_NAME)
PROJECT_FILE := $(PROJECT_DIR)/$(PROJECT_NAME).csproj
CONFIG := Release

# Directories
LOG_DIR := $(PROJECT_DIR)/Logs
SCREENSHOT_DIR := $(PROJECT_DIR)/Screenshots
ALLURE_RESULTS_DIR := $(PROJECT_DIR)/allure-results
ALLURE_REPORT_DIR := $(PROJECT_DIR)/allure-report
BIN_DIR := $(PROJECT_DIR)/bin
OBJ_DIR := $(PROJECT_DIR)/obj

help:
	@echo "$(BLUE)PlaywrightTests - Makefile$(NC)"
	@echo "$(BLUE)=========================$(NC)"
	@echo ""
	@echo "$(GREEN)Setup Targets:$(NC)"
	@echo "  make setup           - Full environment setup (install + restore + build)"
	@echo "  make install         - Install system dependencies"
	@echo "  make restore         - Restore NuGet dependencies"
	@echo ""
	@echo "$(GREEN)Build & Test Targets:$(NC)"
	@echo "  make build           - Build the project"
	@echo "  make clean           - Clean build artifacts"
	@echo "  make clean-all       - Deep clean (includes logs and screenshots)"
	@echo "  make test            - Run all tests"
	@echo "  make test-ui         - Run UI tests only"
	@echo "  make test-api        - Run API tests only"
	@echo "  make test-smoke      - Run smoke tests"
	@echo ""
	@echo "$(GREEN)Code Quality Targets:$(NC)"
	@echo "  make format          - Format code with dotnet format"
	@echo "  make lint            - Run code analyzers"
	@echo ""
	@echo "$(GREEN)Reporting Targets:$(NC)"
	@echo "  make report          - Generate Allure report"
	@echo "  make report-open     - Generate and open Allure report"
	@echo ""
	@echo "$(GREEN)Other Targets:$(NC)"
	@echo "  make help            - Show this help message"
	@echo "  make all             - Full workflow (setup + build + test + report)"
	@echo ""

.check-project:
	@if [ ! -f "$(PROJECT_FILE)" ]; then \
		echo "$(RED)✗ Project file not found: $(PROJECT_FILE)$(NC)"; \
		exit 1; \
	fi

install:
	@echo "$(BLUE)Installing system dependencies...$(NC)"
	@bash setup.sh --install

restore: .check-project
	@echo "$(BLUE)Restoring NuGet dependencies...$(NC)"
	cd $(PROJECT_DIR) && dotnet restore
	@echo "$(GREEN)✓ Restore completed$(NC)"

build: .check-project restore
	@echo "$(BLUE)Building project...$(NC)"
	cd $(PROJECT_DIR) && dotnet build --configuration $(CONFIG)
	@echo "$(GREEN)✓ Build completed$(NC)"

clean:
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	cd $(PROJECT_DIR) && dotnet clean
	@echo "$(GREEN)✓ Clean completed$(NC)"

clean-all: clean
	@echo "$(BLUE)Deep cleaning...$(NC)"
	rm -rf $(LOG_DIR)/* || true
	rm -rf $(SCREENSHOT_DIR)/* || true
	rm -rf $(ALLURE_RESULTS_DIR)/* || true
	rm -rf $(ALLURE_REPORT_DIR) || true
	find . -name "*.log" -delete
	@echo "$(GREEN)✓ Deep clean completed$(NC)"

test: .check-project build
	@echo "$(BLUE)Running all tests...$(NC)"
	cd $(PROJECT_DIR) && dotnet test --configuration $(CONFIG) --verbosity normal --logger "console;verbosity=normal"
	@echo "$(GREEN)✓ Tests completed$(NC)"

test-ui: .check-project build
	@echo "$(BLUE)Running UI tests...$(NC)"
	cd $(PROJECT_DIR) && dotnet test --configuration $(CONFIG) --filter "Category=UI" --verbosity normal
	@echo "$(GREEN)✓ UI tests completed$(NC)"

test-api: .check-project build
	@echo "$(BLUE)Running API tests...$(NC)"
	cd $(PROJECT_DIR) && dotnet test --configuration $(CONFIG) --filter "Category=API" --verbosity normal
	@echo "$(GREEN)✓ API tests completed$(NC)"

test-smoke: .check-project build
	@echo "$(BLUE)Running smoke tests...$(NC)"
	cd $(PROJECT_DIR) && dotnet test --configuration $(CONFIG) --filter "Category=Smoke" --verbosity normal
	@echo "$(GREEN)✓ Smoke tests completed$(NC)"

test-parallel: .check-project build
	@echo "$(BLUE)Running tests in parallel...$(NC)"
	cd $(PROJECT_DIR) && dotnet test --configuration $(CONFIG) --verbosity normal --parallel
	@echo "$(GREEN)✓ Parallel tests completed$(NC)"

format: .check-project
	@echo "$(BLUE)Formatting code...$(NC)"
	cd $(PROJECT_DIR) && dotnet format --verify-no-changes --verbosity diagnostic || dotnet format
	@echo "$(GREEN)✓ Code formatted$(NC)"

lint: .check-project
	@echo "$(BLUE)Running code analysis...$(NC)"
	cd $(PROJECT_DIR) && dotnet build --no-restore /p:EnforceCodeStyleInBuild=true
	@echo "$(GREEN)✓ Code analysis completed$(NC)"

report:
	@echo "$(BLUE)Generating Allure report...$(NC)"
	@if [ -d "$(ALLURE_RESULTS_DIR)" ]; then \
		allure generate $(ALLURE_RESULTS_DIR) -o $(ALLURE_REPORT_DIR) --clean; \
		echo "$(GREEN)✓ Report generated in $(ALLURE_REPORT_DIR)$(NC)"; \
	else \
		echo "$(YELLOW)⚠ No test results found in $(ALLURE_RESULTS_DIR)$(NC)"; \
	fi

report-open: report
	@echo "$(BLUE)Opening Allure report...$(NC)"
	@if command -v allure >/dev/null 2>&1; then \
		allure open $(ALLURE_REPORT_DIR); \
	else \
		echo "$(YELLOW)⚠ Allure CLI not available. Open $(ALLURE_REPORT_DIR)/index.html manually$(NC)"; \
	fi

setup:
	@echo "$(BLUE)Running full setup...$(NC)"
	@bash setup.sh --setup
	@echo "$(GREEN)✓ Setup completed$(NC)"

all: setup build test report
	@echo "$(GREEN)==== Complete workflow finished! =====$(NC)"
	@echo "$(GREEN)✓ Environment setup$(NC)"
	@echo "$(GREEN)✓ Project built$(NC)"
	@echo "$(GREEN)✓ Tests executed$(NC)"
	@echo "$(GREEN)✓ Report generated$(NC)"

.PHONY: .check-project
