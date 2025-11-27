namespace FunctionX.Tests;

/// <summary>
/// Comprehensive tests for the SUMIF function
/// </summary>
public class FxSumIfTests
{
    [Fact]
    public async Task TestSumIf_BasicCondition_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 1, 5, 10, 15, 20 } },
            { "criteria", ">10" }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@data, @criteria)", parameters);

        // Assert
        Assert.Equal(35.0, Convert.ToDouble(result)); // 15 + 20
    }

    [Fact]
    public async Task TestSumIf_LessThan_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "numbers", new object[] { 2, 8, 12, 18, 25 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@numbers, \"<15\")", parameters);

        // Assert
        Assert.Equal(22.0, Convert.ToDouble(result)); // 2 + 8 + 12
    }

    [Fact]
    public async Task TestSumIf_WithSeparateSumRange_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "criteria_range", new object[] { 100, 200, 300, 400, 500 } },
            { "sum_range", new object[] { 1, 2, 3, 4, 5 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@criteria_range, \">250\", @sum_range)", parameters);

        // Assert
        Assert.Equal(12.0, Convert.ToDouble(result)); // 3 + 4 + 5 (where criteria_range values are 300, 400, 500)
    }

    [Fact]
    public async Task TestSumIf_GreaterThanOrEqual_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "values", new object[] { 5, 10, 15, 20, 25 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@values, \">=15\")", parameters);

        // Assert
        Assert.Equal(60.0, Convert.ToDouble(result)); // 15 + 20 + 25
    }

    [Fact]
    public async Task TestSumIf_LessThanOrEqual_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 3, 7, 12, 18, 23 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@data, \"<=12\")", parameters);

        // Assert
        Assert.Equal(22.0, Convert.ToDouble(result)); // 3 + 7 + 12
    }

    [Fact]
    public async Task TestSumIf_NotEqual_WithStrings_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "categories", new object[] { "A", "B", "A", "C", "B" } },
            { "amounts", new object[] { 10, 20, 30, 40, 50 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@categories, \"<>A\", @amounts)", parameters);

        // Assert
        Assert.Equal(110.0, Convert.ToDouble(result)); // 20 + 40 + 50 (where categories are B, C, B)
    }

    [Fact]
    public async Task TestSumIf_ExactStringMatch_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "products", new object[] { "apple", "banana", "apple", "orange", "apple" } },
            { "prices", new object[] { 1.5, 2.0, 1.5, 3.0, 1.5 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@products, \"apple\", @prices)", parameters);

        // Assert
        Assert.Equal(4.5, Convert.ToDouble(result)); // 1.5 + 1.5 + 1.5
    }

    [Fact]
    public async Task TestSumIf_ExactNumberMatch_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "grades", new object[] { 85, 92, 85, 78, 85, 95 } },
            { "weights", new object[] { 1, 2, 1, 3, 1, 2 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@grades, 85, @weights)", parameters);

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result)); // 1 + 1 + 1 (where grades are 85)
    }

    [Fact]
    public async Task TestSumIf_WithEqualsPrefix_ReturnsCorrectSum()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "items", new object[] { "test1", "test2", "test1", "other" } },
            { "values", new object[] { 10, 20, 30, 40 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@items, \"=test1\", @values)", parameters);

        // Assert
        Assert.Equal(40.0, Convert.ToDouble(result)); // 10 + 30
    }

    [Fact]
    public async Task TestSumIf_MixedDataTypes_OnlySumsNumbers()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "criteria_data", new object[] { 1, 2, 3, 4, 5 } },
            { "sum_data", new object?[] { 10, "text", 30, null, 50 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@criteria_data, \">2\", @sum_data)", parameters);

        // Assert
        Assert.Equal(80.0, Convert.ToDouble(result)); // 30 + 50 (text and null are ignored)
    }

    [Fact]
    public async Task TestSumIf_EmptyArray_ReturnsZero()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "empty", new object[] { } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@empty, \">0\")", parameters);

        // Assert
        Assert.Equal(0.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestSumIf_NoMatches_ReturnsZero()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "data", new object[] { 1, 2, 3, 4, 5 } }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@data, \">10\")", parameters);

        // Assert
        Assert.Equal(0.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestSumIf_UnequalArrayLengths_HandlesGracefully()
    {
        // Arrange - sum_range is shorter than criteria_range
        var parameters = new Dictionary<string, object?>
        {
            { "criteria_range", new object[] { 1, 2, 3, 4, 5 } },
            { "sum_range", new object[] { 10, 20, 30 } } // Only 3 elements
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@criteria_range, \">2\", @sum_range)", parameters);

        // Assert
        Assert.Equal(30.0, Convert.ToDouble(result)); // Only element at index 2 (value 30) meets criteria >2
    }

    [Fact]
    public async Task TestSumIf_NestedArrays_FlattenCorrectly()
    {
        // Arrange
        var nestedCriteria = new object[]
        {
            new object[] { 1, 5, 10 },
            new object[] { 15, 20, 25 }
        };
        var nestedSum = new object[]
        {
            new object[] { 2, 4, 6 },
            new object[] { 8, 10, 12 }
        };

        var parameters = new Dictionary<string, object?>
        {
            { "nested_criteria", nestedCriteria },
            { "nested_sum", nestedSum }
        };

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@nested_criteria, \">10\", @nested_sum)", parameters);

        // Assert
        Assert.Equal(30.0, Convert.ToDouble(result)); // 8 + 10 + 12 (where criteria are 15, 20, 25)
    }

    [Fact]
    public async Task TestSumIf_InvalidCriteria_ThrowsException()
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
            await Fx.EvaluateAsync("SUMIF(@data, @badCriteria)", parameters);
        });
    }

    [Fact]
    public async Task TestSumIf_PerformanceWithLargeDataset()
    {
        // Arrange
        var largeCriteria = Enumerable.Range(1, 10_000).Cast<object>().ToArray();
        var largeSums = Enumerable.Range(1, 10_000).Cast<object>().ToArray();
        var parameters = new Dictionary<string, object?>
        {
            { "bigCriteria", largeCriteria },
            { "bigSums", largeSums }
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await Fx.EvaluateAsync("SUMIF(@bigCriteria, \">5000\", @bigSums)", parameters);

        // Assert
        stopwatch.Stop();
        var expectedSum = Enumerable.Range(5001, 5000).Sum(); // Sum of 5001 to 10000
        Assert.Equal((double)expectedSum, Convert.ToDouble(result));
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"SUMIF performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }
}