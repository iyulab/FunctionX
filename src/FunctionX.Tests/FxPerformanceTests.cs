namespace FunctionX.Tests;

/// <summary>
/// Performance tests for FunctionX library with large data sets
/// </summary>
public class FxPerformanceTests
{
    [Fact]
    public async Task TestSumWithLargeArray_Performance()
    {
        // Arrange
        var largeArray = Enumerable.Range(1, 100_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "largeNumbers", largeArray }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("SUM(@largeNumbers)", parameters);

        // Assert
        stopwatch.Stop();
        Assert.Equal(5000050000.0, Convert.ToDouble(result)); // Sum of 1 to 100,000
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"Performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestAverageWithLargeArray_Performance()
    {
        // Arrange
        var largeArray = Enumerable.Range(1, 50_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "numbers", largeArray }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("AVERAGE(@numbers)", parameters);

        // Assert
        stopwatch.Stop();
        Assert.Equal(25000.5, Convert.ToDouble(result)); // Average of 1 to 50,000
        Assert.True(stopwatch.ElapsedMilliseconds < 3000, $"Performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestComplexExpressionWithLargeData_Performance()
    {
        // Arrange
        var array1 = Enumerable.Range(1, 10_000).Cast<object>().ToArray();
        var array2 = Enumerable.Range(10_001, 10_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "set1", array1 },
            { "set2", array2 }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("SUM(@set1) + SUM(@set2)", parameters);

        // Assert
        stopwatch.Stop();
        var expected = array1.Cast<int>().Sum() + array2.Cast<int>().Sum();
        Assert.Equal((double)expected, Convert.ToDouble(result));
        Assert.True(stopwatch.ElapsedMilliseconds < 2000, $"Performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestCountWithLargeDataset_Performance()
    {
        // Arrange - Create large numeric dataset
        var largeArray = Enumerable.Range(1, 50_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "data", largeArray }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@data)", parameters);

        // Assert
        stopwatch.Stop();
        Assert.Equal(50000.0, Convert.ToDouble(result));
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"COUNT performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestNestedFunctionsWithLargeData_Performance()
    {
        // Arrange
        var largeArray = Enumerable.Range(1, 25_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "data", largeArray }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - Test nested function calls
        var result = await Fx.EvaluateAsync("IF(AVERAGE(@data) > 10000, MAX(@data), MIN(@data))", parameters);

        // Assert
        stopwatch.Stop();
        Assert.Equal(25000.0, Convert.ToDouble(result)); // Should return MAX since average > 10000
        Assert.True(stopwatch.ElapsedMilliseconds < 2000, $"Nested functions performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestConcatenationWithLargeStrings_Performance()
    {
        // Arrange
        var largeStringArray = Enumerable.Range(1, 1000)
            .Select(i => $"String{i}")
            .Cast<object>()
            .ToArray();

        var parameters = new Dictionary<string, object?>
        {
            { "strings", largeStringArray }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("CONCAT(@strings)", parameters);

        // Assert
        stopwatch.Stop();
        var expectedLength = largeStringArray.Cast<string>().Sum(s => s.Length);
        Assert.Equal(expectedLength, result?.ToString()?.Length);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"String concatenation performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TestMemoryUsageWithVeryLargeArray()
    {
        // Arrange - Test memory efficiency
        var initialMemory = GC.GetTotalMemory(true);
        var veryLargeArray = Enumerable.Range(1, 1_000_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "megaData", veryLargeArray }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@megaData)", parameters);

        // Assert
        var finalMemory = GC.GetTotalMemory(true);
        var memoryIncrease = finalMemory - initialMemory;

        Assert.Equal(1_000_000.0, Convert.ToDouble(result));
        // Memory increase should be reasonable (less than 100MB for this test)
        Assert.True(memoryIncrease < 100_000_000, $"Memory usage too high: {memoryIncrease} bytes");
    }

    [Fact]
    public async Task TestSecurityValidationPerformance_WithManyPatterns()
    {
        // Arrange - Test that security validation doesn't significantly impact performance
        var safeExpression = "SUM(1,2,3,4,5) + AVERAGE(10,20,30) * MAX(100,200,300)";
        var parameters = new Dictionary<string, object?>();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - Run multiple times to test cumulative performance
        for (int i = 0; i < 1000; i++)
        {
            var result = await Fx.EvaluateAsync(safeExpression, parameters);
        }

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"Security validation performance impact too high: {stopwatch.ElapsedMilliseconds}ms for 1000 evaluations");
    }
}