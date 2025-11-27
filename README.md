# FunctionX

[![NuGet](https://img.shields.io/nuget/v/FunctionX.svg)](https://www.nuget.org/packages/FunctionX)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FunctionX.svg)](https://www.nuget.org/packages/FunctionX)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-512BD4)](https://dotnet.microsoft.com/)
[![Build Status](https://github.com/iyulab/FunctionX/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/iyulab/FunctionX/actions/workflows/nuget-publish.yml)

A high-performance, Excel-compatible formula evaluation engine for .NET applications. FunctionX enables developers to execute Excel-like formulas with dynamic parameters, robust security validation, and comprehensive function support.

## ✨ Key Features

- **🔧 Excel Compatibility**: 36 built-in functions matching Excel behavior
- **⚡ High Performance**: Roslyn-powered compilation with intelligent caching
- **🛡️ Security-First**: Advanced expression validation with pre-compiled regex patterns
- **🎯 Type Safety**: Full nullable reference type support with comprehensive error handling
- **📊 Rich Data Support**: Arrays, dictionaries, nested structures, and mixed data types
- **🔄 Async/Await**: Modern asynchronous API design
- **🎪 Multi-Platform**: Supports .NET 8.0, 9.0, and 10.0 (LTS)

## 🚀 Quick Start

### Basic Usage

```csharp
using FunctionX;

// Simple mathematical expressions
var result = await Fx.EvaluateAsync("1 + 2 * 3"); // Returns: 7

// Excel-style functions
var sum = await Fx.EvaluateAsync("SUM(1, 2, 3, 4, 5)"); // Returns: 15
var average = await Fx.EvaluateAsync("AVERAGE(10, 20, 30)"); // Returns: 20

// New in v0.3.0: Math functions
var sqrt = await Fx.EvaluateAsync("SQRT(16)"); // Returns: 4
var power = await Fx.EvaluateAsync("POWER(2, 10)"); // Returns: 1024
var mod = await Fx.EvaluateAsync("MOD(17, 5)"); // Returns: 2
```

### Working with Parameters

```csharp
// Using dynamic parameters with @ syntax
var parameters = new Dictionary<string, object?>
{
    { "sales", new[] { 1000, 1500, 2000, 1200 } },
    { "target", 1300 }
};

// Calculate values meeting criteria
var highSales = await Fx.EvaluateAsync("COUNTIF(@sales, \">1300\")", parameters); // Returns: 2
var totalHigh = await Fx.EvaluateAsync("SUMIF(@sales, \">@target\", @sales)", parameters); // Returns: 3500

// New in v0.3.0: AVERAGEIF
var avgHigh = await Fx.EvaluateAsync("AVERAGEIF(@sales, \">1300\")", parameters); // Returns: 1750

// Conditional logic with parameters
var performance = await Fx.EvaluateAsync(
    "IF(AVERAGE(@sales) >= @target, \"Above Target\", \"Below Target\")",
    parameters); // Returns: "Above Target"
```

### Advanced Examples

```csharp
// Complex nested expressions
var complexResult = await Fx.EvaluateAsync(
    "ROUND(AVERAGE(@data) * 1.15, 2)",
    new Dictionary<string, object?> { { "data", new[] { 85.6, 92.3, 78.9 } } }
); // Returns: 98.42

// Data validation and processing
var validationResult = await Fx.EvaluateAsync(
    "IF(AND(ISNUMBER(@input), NOT(ISBLANK(@input))), ABS(@input), 0)",
    new Dictionary<string, object?> { { "input", -42.5 } }
); // Returns: 42.5

// Text processing
var formatted = await Fx.EvaluateAsync(
    "PROPER(TRIM(@name))",
    new Dictionary<string, object?> { { "name", "  john doe  " } }
); // Returns: "John Doe"

// Math operations (v0.3.0)
var distance = await Fx.EvaluateAsync(
    "SQRT(POWER(@x, 2) + POWER(@y, 2))",
    new Dictionary<string, object?> { { "x", 3 }, { "y", 4 } }
); // Returns: 5
```

### Error Handling

```csharp
try
{
    var result = await Fx.EvaluateAsync("INVALID_FUNCTION(1, 2)");
}
catch (FxCompilationErrorException ex)
{
    Console.WriteLine($"Formula error: {ex.Message}");
}
catch (FxUnsafeExpressionException ex)
{
    Console.WriteLine($"Security error: {ex.Message}");
}
catch (FxValueException ex)
{
    Console.WriteLine($"Value error: {ex.Message}");
}
catch (FxNumException ex)
{
    Console.WriteLine($"Numeric error: {ex.Message}"); // e.g., SQRT of negative
}
catch (FxDivideByZeroException ex)
{
    Console.WriteLine($"Division error: {ex.Message}"); // e.g., MOD with zero divisor
}
```

## 📋 Implemented Functions

| Category | Functions |
|----------|-----------|
| **Mathematical (11)** | `SUM`, `AVERAGE`, `MAX`, `MIN`, `COUNT`, `COUNTA`, `ROUND`, `ABS`, `SQRT`, `POWER`, `MOD` |
| **Conditional (7)** | `COUNTIF`, `SUMIF`, `AVERAGEIF`, `IF`, `IFS`, `SWITCH`, `IFERROR` |
| **Logical (4)** | `AND`, `OR`, `NOT`, `XOR` |
| **Text (10)** | `CONCAT`, `LEFT`, `RIGHT`, `MID`, `TRIM`, `UPPER`, `LOWER`, `PROPER`, `REPLACE`, `LEN` |
| **Data (3)** | `INDEX`, `VLOOKUP`, `UNIQUE` |
| **Validation (2)** | `ISBLANK`, `ISNUMBER` |
| **Utility (1)** | `INT` |

**Total: 38 Excel-compatible functions**

### Function Details

| Function | Description | Example |
|----------|-------------|---------|
| `SUM` | Calculates the sum of numeric values | `SUM(1, 2, 3)` → 6 |
| `AVERAGE` | Calculates the average of numeric values | `AVERAGE(10, 20)` → 15 |
| `MAX` | Finds the maximum value | `MAX(1, 5, 3)` → 5 |
| `MIN` | Finds the minimum value | `MIN(1, 5, 3)` → 1 |
| `COUNT` | Counts numeric values | `COUNT(1, "a", 2)` → 2 |
| `COUNTA` | Counts non-empty values | `COUNTA(1, "a", null)` → 2 |
| `ROUND` | Rounds to decimal places | `ROUND(3.456, 2)` → 3.46 |
| `ABS` | Returns absolute value | `ABS(-5)` → 5 |
| `SQRT` | Returns square root | `SQRT(16)` → 4 |
| `POWER` | Returns number raised to power | `POWER(2, 3)` → 8 |
| `MOD` | Returns remainder after division | `MOD(10, 3)` → 1 |
| `COUNTIF` | Counts values meeting condition | `COUNTIF(@arr, ">5")` |
| `SUMIF` | Sums values meeting condition | `SUMIF(@arr, ">5")` |
| `AVERAGEIF` | Averages values meeting condition | `AVERAGEIF(@arr, ">5")` |
| `IF` | Conditional branching | `IF(true, "Yes", "No")` → "Yes" |
| `IFS` | Multiple condition check | `IFS(false, 1, true, 2)` → 2 |
| `SWITCH` | Match value against cases | `SWITCH(@v, 1, "A", 2, "B")` |
| `IFERROR` | Handle errors gracefully | `IFERROR(1/0, "Error")` |
| `AND` | Logical AND | `AND(true, true)` → true |
| `OR` | Logical OR | `OR(false, true)` → true |
| `NOT` | Logical NOT | `NOT(false)` → true |
| `XOR` | Logical XOR | `XOR(true, true)` → false |
| `CONCAT` | Concatenate strings | `CONCAT("A", "B")` → "AB" |
| `LEFT` | Left characters | `LEFT("Hello", 2)` → "He" |
| `RIGHT` | Right characters | `RIGHT("Hello", 2)` → "lo" |
| `MID` | Middle characters | `MID("Hello", 2, 3)` → "ell" |
| `TRIM` | Remove whitespace | `TRIM("  Hi  ")` → "Hi" |
| `UPPER` | Convert to uppercase | `UPPER("hi")` → "HI" |
| `LOWER` | Convert to lowercase | `LOWER("HI")` → "hi" |
| `PROPER` | Title case | `PROPER("john doe")` → "John Doe" |
| `REPLACE` | Replace substring | `REPLACE("Hi", "i", "ello")` → "Hello" |
| `LEN` | String length | `LEN("Hello")` → 5 |
| `INDEX` | Get value at position | `INDEX(@arr, 1, 1)` |
| `VLOOKUP` | Vertical lookup | `VLOOKUP(@key, @range, 2)` |
| `UNIQUE` | Remove duplicates | `UNIQUE(1, 2, 1)` → [1, 2] |
| `ISBLANK` | Check if blank | `ISBLANK(null)` → true |
| `ISNUMBER` | Check if number | `ISNUMBER(42)` → true |
| `INT` | Convert to integer | `INT(3.7)` → 3 |

## 🔧 Performance & Security

### Performance Features

- **Compilation Caching**: Compiled expressions are cached for repeated use with configurable cache size
- **Pre-compiled Regex**: Security patterns use compiled regex for faster validation
- **Optimized Execution**: Roslyn-based compilation provides near-native performance
- **Memory Efficient**: Automatic cache trimming prevents unbounded memory growth
- **Async Operations**: Non-blocking evaluation with full async support

```csharp
// Cache management
Fx.MaxCacheSize = 500; // Configure max cached expressions (default: 1000)
Fx.ClearCache(); // Clear compilation cache when needed
var (scripts, options) = Fx.GetCacheStatistics(); // Monitor cache usage
```

### Security Features

- **Expression Validation**: 60+ pre-compiled regex patterns prevent injection attacks
- **Forbidden Patterns**: Blocks access to Process, File, Assembly, Reflection, etc.
- **Length Limits**: Expression length validation prevents DoS attacks
- **Sandboxed Execution**: Restricted access to system resources
- **Type Safety**: Strong typing with nullable reference type support

## 📦 Installation

Install via NuGet Package Manager:

```bash
# .NET CLI
dotnet add package FunctionX

# Package Manager Console
Install-Package FunctionX

# PackageReference
<PackageReference Include="FunctionX" Version="0.3.0" />
```

## 🎯 Supported Platforms

- **.NET 8.0** - Long-term support (LTS)
- **.NET 9.0** - Standard term support (STS)
- **.NET 10.0** - Long-term support (LTS) ✨ Latest

> **Note**: Starting from v0.3.0, FunctionX focuses on .NET 8.0+ for improved quality and performance. For older .NET versions, use v0.2.x.

## 🧪 Testing & Quality

- **Comprehensive Test Suite**: 180+ unit tests covering all functions
- **Edge Case Coverage**: Extensive testing of boundary conditions
- **Performance Testing**: Validated with large datasets
- **Security Testing**: Validation against injection attacks
- **XML Documentation**: Full IntelliSense support with XML comments

## 📝 Changelog

### v0.3.0 (2024-2025)

**New Features**
- Added `SQRT` function - square root calculation
- Added `POWER` function - exponentiation
- Added `MOD` function - modulo with Excel-compatible sign behavior
- Added `AVERAGEIF` function - conditional averaging

**Improvements**
- Extended target frameworks to .NET 8.0, 9.0, and 10.0 (LTS)
- Pre-compiled 60+ security regex patterns for faster validation
- Added configurable cache size limit (`MaxCacheSize`) with automatic trimming
- Complete XML documentation for all public APIs
- DRY refactoring: extracted common criteria parsing logic

**Breaking Changes**
- Dropped support for .NET Standard 2.1, .NET 5.0, 6.0, 7.0
- Minimum supported version is now .NET 8.0
- Maximum supported version is .NET 10.0 (LTS)

### v0.2.x

- Initial release with 32 Excel-compatible functions
- Support for .NET Standard 2.1 through .NET 9.0

## 🤝 Contributing

FunctionX is an open-source project. We welcome contributions!

### How to Contribute
1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-function`)
3. **Commit** your changes (`git commit -m 'Add amazing function'`)
4. **Push** to the branch (`git push origin feature/amazing-function`)
5. **Open** a Pull Request

### Development Setup
```bash
git clone https://github.com/iyulab/FunctionX.git
cd FunctionX/src
dotnet build
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.