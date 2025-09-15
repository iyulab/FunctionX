namespace FunctionX.Tests;

/// <summary>
/// Comprehensive tests for the COUNTIF function
/// </summary>
public class FxCountIfTests
{
    [Fact]
    public async Task TestCountIf_GreaterThan_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 1, 5, 10, 15, 20 } },
            { "criteria", ">10" }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@data, @criteria)", parameters);

        // Assert
        Assert.Equal(2.0, Convert.ToDouble(result)); // 15, 20
    }

    [Fact]
    public async Task TestCountIf_LessThan_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "numbers", new object[] { 1, 5, 10, 15, 20 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@numbers, \"<10\")", parameters);

        // Assert
        Assert.Equal(2.0, Convert.ToDouble(result)); // 1, 5
    }

    [Fact]
    public async Task TestCountIf_GreaterThanOrEqual_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "values", new object[] { 5, 10, 15, 20, 25 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@values, \">=15\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // 15, 20, 25
    }

    [Fact]
    public async Task TestCountIf_LessThanOrEqual_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 1, 5, 10, 15, 20 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@data, \"<=10\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // 1, 5, 10
    }

    [Fact]
    public async Task TestCountIf_NotEqual_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "items", new object[] { "apple", "banana", "apple", "orange", "banana" } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@items, \"<>apple\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // banana, orange, banana
    }

    [Fact]
    public async Task TestCountIf_ExactMatch_String_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "fruits", new object[] { "apple", "banana", "apple", "orange", "apple" } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@fruits, \"apple\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestCountIf_ExactMatch_Number_ReturnsCorrectCount()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "numbers", new object[] { 1, 2, 3, 2, 4, 2, 5 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@numbers, 2)", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestCountIf_WithEquals_ExplicitMatch()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "values", new object[] { "test", "Test", "test", "other" } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@values, \"=test\")", parameters);

        // Assert
        Assert.Equal(2.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestCountIf_MixedTypes_OnlyCountsAppropriateTypes()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "mixed", new object[] { 1, "5", 10, "text", 15, null } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@mixed, \">5\")", parameters);

        // Assert
        Assert.Equal(2.0, Convert.ToDouble(result)); // 10, 15 (string "5" and "text" are ignored)
    }

    [Fact]
    public async Task TestCountIf_EmptyArray_ReturnsZero()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "empty", new object[] { } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@empty, \">0\")", parameters);

        // Assert
        Assert.Equal(0.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestCountIf_NullValues_HandledCorrectly()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "withNulls", new object?[] { 1, null, 5, null, 10 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@withNulls, \">0\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // null values ignored
    }

    [Fact]
    public async Task TestCountIf_NestedArrays_FlattenCorrectly()
    {
        // Arrange
        var nestedArray = new object[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 4, 5, 6 }
        };

        var parameters = new Dictionary<string, object?>
        {
            { "nested", nestedArray }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@nested, \">3\")", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // 4, 5, 6
    }

    [Fact]
    public async Task TestCountIf_InvalidCriteria_ThrowsException()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 1, 2, 3 } },
            { "badCriteria", ">notanumber" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<FxValueException>(async () =>
        {
            await Fx.EvaluateAsync("COUNTIF(@data, @badCriteria)", parameters);
        });
    }

    [Fact]
    public async Task TestCountIf_PerformanceWithLargeDataset()
    {
        // Arrange
        var largeDataset = Enumerable.Range(1, 10_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "bigData", largeDataset }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("COUNTIF(@bigData, \">5000\")", parameters);

        // Assert
        stopwatch.Stop();
        Assert.Equal(5000.0, Convert.ToDouble(result)); // Numbers 5001-10000
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"COUNTIF performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }
}