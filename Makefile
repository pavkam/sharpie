# =============================================================================
# Configuration Variables
# =============================================================================

# Build Configuration
CONFIGURATION ?= Release
VERBOSITY ?=

# Test Configuration
COVERAGE_REPORT_PATH ?= coverage_report
BLAME_HANG_TIMEOUT ?= 30000

# Report Configuration
REPORT_DIR = $(COVERAGE_REPORT_PATH)
SUMMARY_FILE = $(REPORT_DIR)/Summary.md

# =============================================================================
# Targets
# =============================================================================

.PHONY: check-tools build run lint format test test-and-report generate-docs

# Ensure all tools are installed locally
check-tools:
	@dotnet tool restore > /dev/null

# Build the project
build:
	dotnet build --configuration $(CONFIGURATION)

# Check formatting and code style
lint: check-tools
	dotnet format --verify-no-changes $(if $(VERBOSITY),--verbosity $(VERBOSITY))
	dotnet format style --verify-no-changes $(if $(VERBOSITY),--verbosity $(VERBOSITY))
	dotnet format analyzers --verify-no-changes $(if $(VERBOSITY),--verbosity $(VERBOSITY))

# Apply formatting to the entire project
format: check-tools
	dotnet format
	dotnet format style
	dotnet format analyzers

# Test with code coverage
test: build
	dotnet test --configuration $(CONFIGURATION) --verbosity normal --blame-hang --blame-hang-timeout $(BLAME_HANG_TIMEOUT) --collect:"Code Coverage;Format=cobertura" --logger "trx;LogFileName=test-results.trx"

# Test report
test-report: check-tools
	dotnet reportgenerator -reports:"**/TestResults/**/*.cobertura.xml" -targetdir:"$(REPORT_DIR)" -reporttypes:"HtmlInline;Cobertura;Badges;MarkdownSummary"
	@echo "✅ Test report generated in '$(REPORT_DIR)' directory"
	@[ -f $(SUMMARY_FILE) ] && cat $(SUMMARY_FILE) || echo " 🚨 Summary not found"

# Generate documentation
generate-docs: check-tools
	mv "./README.md" "./index.md"
	@mv ./LICENSE ./LICENSE.md
	@sed -i 's/LICENSE/LICENSE.md/g' index.md
	@dotnet docfx ./docfx.json
