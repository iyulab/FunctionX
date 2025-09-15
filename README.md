# FunctionX

A high-performance, Excel-compatible formula evaluation engine for .NET applications. FunctionX enables developers to execute Excel-like formulas with dynamic parameters, robust security validation, and comprehensive function support.

## ✨ Key Features

- **🔧 Excel Compatibility**: 32 built-in functions matching Excel behavior
- **⚡ High Performance**: Roslyn-powered compilation with intelligent caching
- **🛡️ Security-First**: Advanced expression validation and sandboxing
- **🎯 Type Safety**: Full nullable reference type support with comprehensive error handling
- **📊 Rich Data Support**: Arrays, dictionaries, nested structures, and mixed data types
- **🔄 Async/Await**: Modern asynchronous API design
- **🎪 Multi-Platform**: Supports .NET Standard 2.1, .NET 5-9

## 🚀 Quick Start

### Basic Usage

```csharp
using FunctionX;

// Simple mathematical expressions
var result = await Fx.EvaluateAsync("1 + 2 * 3"); // Returns: 7

// Excel-style functions
var sum = await Fx.EvaluateAsync("SUM(1, 2, 3, 4, 5)"); // Returns: 15
var average = await Fx.EvaluateAsync("AVERAGE(10, 20, 30)"); // Returns: 20
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
```

## Implemented Functions
| Function Name | Description |
|---------------|-------------|
| **Mathematical Functions** |
| `SUM` | Calculates the sum of numeric values. |
| `AVERAGE` | Calculates the average of numeric values. |
| `MAX` | Finds the maximum value among numeric values. |
| `MIN` | Finds the minimum value among numeric values. |
| `COUNT` | Counts the number of numeric values. |
| `COUNTA` | Counts the number of non-empty values. |
| `ROUND` | Rounds a number to a specified number of decimal places. |
| `ABS` | Returns the absolute value of a number. |
| **Conditional Functions** |
| `COUNTIF` | Counts the number of cells that meet a specified condition. |
| `SUMIF` | Sums the values that meet a specified condition. |
| `IF` | Returns one of two values depending on a condition. |
| `IFS` | Returns the result of the first true condition among multiple conditions. |
| `SWITCH` | Returns a value based on given cases or provides a default value. |
| `IFERROR` | Returns a value if an expression results in an error. |
| **Logical Functions** |
| `AND` | Returns true if all values are true. |
| `OR` | Returns true if at least one value is true. |
| `NOT` | Returns true if the value is false. |
| `XOR` | Returns true if an odd number of values are true. |
| **Text Functions** |
| `CONCAT` | Concatenates string values. |
| `LEFT` | Returns a specified number of characters from the beginning of a string. |
| `RIGHT` | Returns a specified number of characters from the end of a string. |
| `MID` | Returns a specified number of characters from a string starting at a specified position. |
| `TRIM` | Removes leading and trailing whitespace from a string. |
| `UPPER` | Converts a string to uppercase. |
| `LOWER` | Converts a string to lowercase. |
| `PROPER` | Converts the first letter of each word in a string to uppercase. |
| `REPLACE` | Replaces occurrences of a specified substring within a string with another substring. |
| `LEN` | Returns the length of a string. |
| **Data Functions** |
| `INDEX` | Returns the value at a specified position in an array or dictionary. |
| `VLOOKUP` | Searches for a value in the first column of a range and returns a value in the same row from a specified column. |
| `UNIQUE` | Returns an array of unique values with duplicates removed. |
| **Validation Functions** |
| `ISBLANK` | Checks if a value is blank (null or empty). |
| `ISNUMBER` | Checks if a value is a number. |

## 🔧 Performance & Security

### Performance Features
- **Compilation Caching**: Compiled expressions are cached for repeated use
- **Optimized Execution**: Roslyn-based compilation provides near-native performance
- **Memory Efficient**: Smart memory management for large data sets
- **Async Operations**: Non-blocking evaluation with full async support

```csharp
// Cache management
Fx.ClearCache(); // Clear compilation cache when needed
var (scripts, options) = Fx.GetCacheStatistics(); // Monitor cache usage
```

### Security Features
- **Expression Validation**: Prevents injection attacks and unsafe code execution
- **Sandboxed Execution**: Restricted access to system resources
- **Type Safety**: Strong typing with nullable reference type support
- **Input Sanitization**: Automatic validation of all input parameters

## 📚 Function Categories

### Mathematical Functions (8)
`SUM`, `AVERAGE`, `MAX`, `MIN`, `COUNT`, `COUNTA`, `ROUND`, `ABS`

### Conditional Functions (6)
`COUNTIF`, `SUMIF`, `IF`, `IFS`, `SWITCH`, `IFERROR`

### Logical Functions (4)
`AND`, `OR`, `NOT`, `XOR`

### Text Functions (9)
`CONCAT`, `LEFT`, `RIGHT`, `MID`, `TRIM`, `UPPER`, `LOWER`, `PROPER`, `LEN`

### Data Functions (3)
`INDEX`, `VLOOKUP`, `UNIQUE`

### Validation Functions (2)
`ISBLANK`, `ISNUMBER`

**Total: 32 Excel-compatible functions**

## 📦 Installation

Install via NuGet Package Manager:

```bash
# .NET CLI
dotnet add package FunctionX

# Package Manager Console
Install-Package FunctionX

# PackageReference
<PackageReference Include="FunctionX" Version="0.2.0" />
```

## 🎯 Supported Platforms

- **.NET Standard 2.1** - Broad compatibility
- **.NET 5.0** - Cross-platform support
- **.NET 6.0** - Long-term support
- **.NET 7.0** - Performance improvements
- **.NET 8.0** - Latest stable features
- **.NET 9.0** - Cutting-edge support

## 🧪 Testing & Quality

- **Comprehensive Test Suite**: 150+ unit tests covering all functions
- **Edge Case Coverage**: Extensive testing of boundary conditions
- **Performance Testing**: Validated with large datasets (10K+ elements)
- **Security Testing**: Validation against injection attacks
- **Cross-Platform Testing**: Verified across all supported .NET versions

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
cd FunctionX
dotnet build
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Inspired by Microsoft Excel's formula engine
- Built with [Roslyn](https://github.com/dotnet/roslyn) for compilation
- Comprehensive testing with [xUnit](https://xunit.net/)

---

**Made with ❤️ by [Iyulab Corporation](https://github.com/iyulab)**