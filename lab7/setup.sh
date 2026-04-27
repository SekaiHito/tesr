#!/bin/bash

################################################################################
# Playwright NUnit Test Framework - Bootstrap Script for Arch Linux
#
# This script sets up the complete development environment for the Playwright
# NUnit testing framework on Arch Linux / CachyOS systems.
#
# Usage: bash setup.sh [--help] [--install] [--restore] [--clean]
################################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Script variables
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_NAME="PlaywrightTests"
PROJECT_FILE="${SCRIPT_DIR}/${PROJECT_NAME}/${PROJECT_NAME}.csproj"

################################################################################
# Helper Functions
################################################################################

print_header() {
    echo -e "${BLUE}================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

print_usage() {
    cat << EOF
Usage: bash setup.sh [OPTIONS]

Options:
    --help              Show this help message
    --install           Install all system dependencies
    --restore           Restore NuGet dependencies only
    --clean             Clean build artifacts
    --build             Build the project
    --test              Run tests
    --setup             Full setup (install + restore + build)
    --all               Run everything (install + restore + build + test)

Examples:
    bash setup.sh --help
    bash setup.sh --setup         # Install dependencies and restore packages
    bash setup.sh --all           # Complete setup and test run
EOF
}

################################################################################
# System Requirements Check
################################################################################

check_system() {
    print_header "Checking System Requirements"

    # Check if running on Arch Linux
    if [[ ! -f /etc/os-release ]]; then
        print_warning "Cannot determine OS"
    else
        source /etc/os-release
        print_info "Operating System: $PRETTY_NAME"
    fi

    # Check .NET SDK
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK is not installed"
        return 1
    fi

    local dotnet_version=$(dotnet --version)
    print_success ".NET SDK: $dotnet_version"

    if [[ ! "$dotnet_version" =~ ^10\. ]] && [[ ! "$dotnet_version" =~ ^9\. ]] && [[ ! "$dotnet_version" =~ ^8\. ]]; then
        print_warning ".NET SDK version is older than recommended. Current: $dotnet_version, Recommended: 10.0+"
    fi

    return 0
}

################################################################################
# Install System Dependencies
################################################################################

install_dependencies() {
    print_header "Installing System Dependencies"

    print_info "Updating package manager cache..."
    sudo pacman -Sy --noconfirm || {
        print_error "Failed to update package cache"
        return 1
    }

    # Install required packages
    local packages=(
        "dotnet-sdk"
        "dotnet-runtime"
        "aspnet-runtime"
    )

    print_info "Installing packages: ${packages[*]}"

    for package in "${packages[@]}"; do
        if pacman -Q "$package" &> /dev/null; then
            print_success "$package is already installed"
        else
            print_info "Installing $package..."
            sudo pacman -S "$package" --noconfirm || {
                print_warning "Failed to install $package, but continuing..."
            }
        fi
    done

    # Check if dotnet is available
    if command -v dotnet &> /dev/null; then
        print_success "Dependencies installed successfully"
        return 0
    else
        print_error "Installation failed: dotnet command not available"
        return 1
    fi
}

################################################################################
# Restore NuGet Dependencies
################################################################################

restore_dependencies() {
    print_header "Restoring NuGet Dependencies"

    if [[ ! -f "$PROJECT_FILE" ]]; then
        print_error "Project file not found: $PROJECT_FILE"
        return 1
    fi

    print_info "Restoring dependencies for: $PROJECT_NAME"
    cd "$SCRIPT_DIR/$PROJECT_NAME"

    if dotnet restore; then
        print_success "Dependencies restored successfully"
        cd - > /dev/null
        return 0
    else
        print_error "Failed to restore dependencies"
        cd - > /dev/null
        return 1
    fi
}

################################################################################
# Build Project
################################################################################

build_project() {
    print_header "Building Project"

    if [[ ! -f "$PROJECT_FILE" ]]; then
        print_error "Project file not found: $PROJECT_FILE"
        return 1
    fi

    print_info "Building $PROJECT_NAME..."
    cd "$SCRIPT_DIR/$PROJECT_NAME"

    if dotnet build --configuration Release; then
        print_success "Project built successfully"
        cd - > /dev/null
        return 0
    else
        print_error "Build failed"
        cd - > /dev/null
        return 1
    fi
}

################################################################################
# Run Tests
################################################################################

run_tests() {
    print_header "Running Tests"

    if [[ ! -f "$PROJECT_FILE" ]]; then
        print_error "Project file not found: $PROJECT_FILE"
        return 1
    fi

    print_info "Running tests..."
    cd "$SCRIPT_DIR/$PROJECT_NAME"

    if dotnet test --configuration Release --verbosity normal; then
        print_success "Tests completed"
        cd - > /dev/null
        return 0
    else
        print_warning "Some tests may have failed"
        cd - > /dev/null
        return 1
    fi
}

################################################################################
# Clean Build Artifacts
################################################################################

clean_project() {
    print_header "Cleaning Build Artifacts"

    if [[ ! -f "$PROJECT_FILE" ]]; then
        print_error "Project file not found: $PROJECT_FILE"
        return 1
    fi

    print_info "Cleaning $PROJECT_NAME..."
    cd "$SCRIPT_DIR/$PROJECT_NAME"

    if dotnet clean; then
        print_success "Project cleaned successfully"

        # Remove additional artifacts
        rm -rf bin obj .vs .vscode
        print_success "Build artifacts removed"

        cd - > /dev/null
        return 0
    else
        print_error "Clean failed"
        cd - > /dev/null
        return 1
    fi
}

################################################################################
# Generate Allure Report
################################################################################

generate_report() {
    print_header "Generating Allure Report"

    if ! command -v allure &> /dev/null; then
        print_warning "Allure command-line tool is not installed"
        print_info "To install: sudo pacman -S allure or visit https://docs.qameta.io/allure/"
        return 1
    fi

    if [[ -d "allure-results" ]]; then
        print_info "Generating report from allure-results..."
        allure generate allure-results -o allure-report --clean
        print_success "Allure report generated in allure-report/"
        return 0
    else
        print_warning "No allure-results directory found"
        return 1
    fi
}

################################################################################
# Setup Environment
################################################################################

setup_environment() {
    print_header "Setting Up Environment"

    # Create required directories
    local directories=(
        "Logs"
        "Screenshots"
        "allure-results"
    )

    for dir in "${directories[@]}"; do
        if [[ -d "$SCRIPT_DIR/$PROJECT_NAME/$dir" ]]; then
            print_info "$dir directory already exists"
        else
            mkdir -p "$SCRIPT_DIR/$PROJECT_NAME/$dir"
            print_success "Created $dir directory"
        fi
    done

    # Set permissions for scripts
    chmod +x "$SCRIPT_DIR/setup.sh" 2>/dev/null || true

    return 0
}

################################################################################
# Full Setup
################################################################################

full_setup() {
    print_header "Full Setup: Install, Restore, Build"

    check_system || return 1
    setup_environment || return 1
    install_dependencies || return 1
    restore_dependencies || return 1
    build_project || return 1

    print_success "Full setup completed successfully!"
    return 0
}

################################################################################
# Run All
################################################################################

run_all() {
    full_setup || return 1
    run_tests || return 1

    print_header "Complete!"
    print_success "Environment is ready for testing"
    return 0
}

################################################################################
# Main Script
################################################################################

main() {
    local option="${1:---help}"

    case "$option" in
        --help)
            print_usage
            ;;
        --check)
            check_system
            ;;
        --install)
            install_dependencies
            ;;
        --restore)
            restore_dependencies
            ;;
        --build)
            build_project
            ;;
        --test)
            run_tests
            ;;
        --clean)
            clean_project
            ;;
        --report)
            generate_report
            ;;
        --setup)
            full_setup
            ;;
        --all)
            run_all
            ;;
        *)
            print_error "Unknown option: $option"
            print_usage
            exit 1
            ;;
    esac
}

# Run main function with all arguments
main "$@"
