# AGENTS.md - Sharpie Project Guide for Coding Agents

## Project Overview

**Sharpie** is a modern .NET terminal manipulation library that provides object-oriented bindings
for `NCurses`, `PDCurses`, and `PDCursesMod`. It targets .NET 9.0+ and provides a comprehensive API
for building terminal-based applications with features like windows, pads, colors, mouse support,
and event handling.

### Key Characteristics

-   **Target Framework**: .NET 9.0
-   **Language Version**: C# 10
-   **Architecture**: Object-oriented wrapper around native Curses libraries
-   **Cross-platform**: Linux, macOS, Windows (x64 and ARM64)
-   **Testing**: `MSTest` with `Moq` and `Shouldly`
-   **Build System**: Makefile + dotnet CLI

## Project Structure

```
Sharpie/
├── Sharpie/                    # Main library
│   ├── Abstractions/           # Interface definitions
│   ├── Backend/                # Curses backend implementations
│   ├── Font/                   # Font handling (Figlet)
│   └── *.cs                    # Core classes (Terminal, Screen, etc.)
├── Sharpie.Tests/              # Test suite
├── Demos/                      # Example applications
├── NativeLibraries/            # Native library projects
└── lib/                        # Pre-built native libraries
```

## Core Architecture

### Main Components

1. **Terminal** - Main entry point, manages terminal lifecycle
2. **Screen** - Main screen window, manages child windows/pads
3. **Window/Pad** - Drawing surfaces with different capabilities
4. **EventPump** - Event handling system
5. **ColorManager** - Color and styling management
6. **ICursesBackend** - Abstraction over native Curses libraries

### Key Design Patterns

-   **Backend Pattern**: `ICursesBackend` abstracts native library differences
-   **Surface Hierarchy**: `Surface` → `TerminalSurface` → `Screen`/`Window`/`Pad`
-   **Event-Driven**: Uses event pump for input handling
-   **Resource Management**: IDisposable pattern throughout
-   **Thread Safety**: SynchronizationContext-based execution model

## Development Environment

### Prerequisites

-   .NET 9.0 SDK
-   Make (for build automation)
-   Native Curses libraries (NCurses, PDCurses, PDCursesMod)

### Build Commands (from Makefile)

```bash
make check-tools   # Ensure all tools are installed
make build         # Build the project
make lint          # Check formatting and code style
make format        # Apply formatting
make test          # Run tests with coverage
make test-report   # Generate test coverage report
make docs          # Generate documentation
```

### Key Dependencies

-   **Nito.AsyncEx.Context** - Async context management
-   **Moq** - Mocking framework (tests)
-   **Shouldly** - Assertion library (tests)
-   **MSTest** - Testing framework

## Coding Standards & Patterns

### Naming Conventions

-   **Classes**: PascalCase (e.g., `Terminal`, `ColorManager`)
-   **Methods**: PascalCase (e.g., `WriteText`, `Refresh`)
-   **Properties**: PascalCase (e.g., `Size`, `Handle`)
-   **Fields**: camelCase with underscore prefix (e.g., `_batchUpdateLocks`)
-   **Interfaces**: PascalCase with 'I' prefix (e.g., `ICursesBackend`)

### Code Style Rules

-   **NO COMMENTS** in code (explicitly forbidden by user rules)
-   Use `[PublicAPI]` attribute for public APIs
-   Use `[SuppressMessage]` for intentional violations
-   Prefer `Debug.Assert()` for internal validation
-   Use `Should.Throw<>()` for exception testing

### Exception Handling

-   Use custom exception types: `CursesOperationException`, `CursesSynchronizationException`
-   Check return values from `Curses` libraries with `.Check()` extension method
-   Use `AssertAlive()` and `AssertSynchronized()` for state validation

## Testing Patterns

### Test Structure

```csharp
[TestClass]
public class ClassNameTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private ClassUnderTest _instance = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        // Setup mocks
    }

    [TestMethod]
    public void MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        _cursesMock.Setup(s => s.SomeMethod()).Returns(expectedValue);

        // Act
        var result = _instance.SomeMethod();

        // Assert
        result.ShouldBe(expectedValue);
        _cursesMock.Verify(v => v.SomeMethod(), Times.Once);
    }
}
```

### Test Naming Convention

-   `MethodName_WhenCondition_ExpectedResult`
-   `MethodName_Throws_IfCondition`
-   `MethodName_Returns_IfCondition`

### Mocking Patterns

-   Use `Mock<T>` for all dependencies
-   Setup return values with `.Setup().Returns()`
-   Verify calls with `.Verify()`
-   Use `It.IsAny<T>()` for parameter matching
-   Use `Times.Once`, `Times.Never` for call verification

### Test Helpers

-   `TestHelpers.cs` contains extension methods for common test operations
-   `MockResolve<T>()` for native symbol resolver mocking
-   `MockArea()` for surface area mocking

## Key Implementation Patterns

### Resource Management

```csharp
public sealed class Terminal : ITerminal, IDisposable
{
    public void Dispose()
    {
        if (Disposed) return;

        // Cleanup resources
        _screen?.Dispose();
        _terminalInstanceActive = false;
        Disposed = true;
    }
}
```

### Event Handling

```csharp
terminal.Run((t, e) =>
{
    switch (e)
    {
        case KeyEvent { Key: Key.Character, Char.Value: 'q' }:
            return Task.FromResult(false); // Exit
        case TerminalResizeEvent:
            // Handle resize
            return Task.CompletedTask;
        default:
            return Task.CompletedTask;
    }
});
```

### Atomic Operations

```csharp
using (terminal.AtomicRefresh())
{
    // Multiple operations that should be batched
    screen.WriteText("Hello");
    screen.DrawBorder();
    // Refresh happens automatically on dispose
}
```

## Backend Architecture

### ICursesBackend Interface

-   Defines all Curses function signatures
-   Platform-specific implementations in `Backend/` folder
-   Uses native symbol resolution for dynamic loading

### Backend Implementation Pattern

```csharp
public abstract class BaseCursesBackend : ICursesBackend
{
    protected internal INativeSymbolResolver CursesSymbolResolver { get; }

    public int SomeMethod() =>
        CursesSymbolResolver.Resolve<FunctionMap.SomeMethod>()();
}
```

## Common Gotchas & Best Practices

### Thread Safety

-   All operations must run on the correct `SynchronizationContext`
-   Use `AssertSynchronized()` to validate thread context
-   Only one `Terminal` instance can be active at a time

### Memory Management

-   Always dispose `Terminal` instances
-   Use `using` statements for automatic disposal
-   Be careful with native handle management

### Platform Differences

-   Different Curses backends have different capabilities
-   Use feature detection rather than platform detection
-   Handle missing features gracefully

### Event Handling

-   Events are processed in the main thread
-   Use `Delegate()` for cross-thread operations
-   Handle `TerminalResizeEvent` for responsive layouts

## Development Workflow

### Making Changes

1. Follow existing patterns and naming conventions
2. Add comprehensive tests for new functionality
3. Use appropriate mocking for dependencies
4. Ensure thread safety considerations

### After Making Changes

1. Run `make format` to apply consistent formatting
2. Run `make lint` to check for lint warnings
3. Run `make test` to ensure no regressions
4. Run `make test-report` to check coverage
5. Update documentation if needed

## Native Library Management

The project includes pre-built native libraries in `lib/`:

-   **NCurses** - Linux/macOS
-   **PDCurses** - Windows
-   **PDCursesMod** - Enhanced Windows support

Native library projects in `NativeLibraries/` handle packaging these libraries for NuGet distribution.

## Documentation Generation

-   Uses DocFX for API documentation
-   Run `make docs` to generate documentation
-   Documentation is published to GitHub Pages

## CI/CD Integration

The project uses GitHub Actions for:

-   Multi-platform builds (Linux, macOS, Windows)
-   Native library compilation
-   Test execution and coverage reporting
-   Documentation publishing
-   NuGet package publishing
