namespace FunctionX.Tests;

/// <summary>
/// Comprehensive tests for mathematical functions (ROUND, ABS)
/// </summary>
public class FxMathTests
{
    #region ROUND Function Tests

    [Fact]
    public async Task TestRound_PositiveDecimals_RoundsCorrectly()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "number", 3.14159 },
            { "decimals", 2 }
        };

        // Act
        var result = await Fx.EvaluateAsync("ROUND(@number, @decimals)", parameters);

        // Assert
        Assert.Equal(3.14, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestRound_ZeroDecimals_RoundsToInteger()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "value", 4.7 }
        };

        // Act
        var result = await Fx.EvaluateAsync("ROUND(@value, 0)", parameters);

        // Assert
        Assert.Equal(5.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestRound_NegativeDecimals_RoundsToTensHundreds()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "number", 1234.56 }
        };

        // Act
        var result1 = await Fx.EvaluateAsync("ROUND(@number, -1)", parameters);
        var result2 = await Fx.EvaluateAsync("ROUND(@number, -2)", parameters);

        // Assert
        Assert.Equal(1230.0, Convert.ToDouble(result1)); // Round to nearest 10
        Assert.Equal(1200.0, Convert.ToDouble(result2)); // Round to nearest 100
    }

    [Fact]
    public async Task TestRound_MidpointRounding_RoundsAwayFromZero()
    {
        // Arrange & Act
        var result1 = await Fx.EvaluateAsync("ROUND(2.5, 0)");
        var result2 = await Fx.EvaluateAsync("ROUND(-2.5, 0)");

        // Assert
        Assert.Equal(3.0, Convert.ToDouble(result1)); // Away from zero
        Assert.Equal(-3.0, Convert.ToDouble(result2)); // Away from zero
    }

    [Fact]
    public async Task TestRound_ExtremeValues_HandlesCorrectly()
    {
        // Arrange & Act
        var result1 = await Fx.EvaluateAsync("ROUND(999999.999, 2)");
        var result2 = await Fx.EvaluateAsync("ROUND(0.000001, 5)");

        // Assert
        Assert.Equal(1000000.0, Convert.ToDouble(result1));
        Assert.Equal(0.00000, Convert.ToDouble(result2));
    }

    [Fact]
    public async Task TestRound_InvalidInput_ThrowsException()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "notNumber", "text" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<FxValueException>(async () =>
        {
            await Fx.EvaluateAsync("ROUND(@notNumber, 2)", parameters);
        });
    }

    #endregion

    #region ABS Function Tests

    [Fact]
    public async Task TestAbs_PositiveNumber_ReturnsSameValue()
    {
        // Arrange & Act
        var result = await Fx.EvaluateAsync("ABS(42)");

        // Assert
        Assert.Equal(42.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestAbs_NegativeNumber_ReturnsPositive()
    {
        // Arrange & Act
        var result = await Fx.EvaluateAsync("ABS(-42)");

        // Assert
        Assert.Equal(42.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestAbs_Zero_ReturnsZero()
    {
        // Arrange & Act
        var result = await Fx.EvaluateAsync("ABS(0)");

        // Assert
        Assert.Equal(0.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestAbs_DecimalNumber_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result1 = await Fx.EvaluateAsync("ABS(-3.14159)");
        var result2 = await Fx.EvaluateAsync("ABS(2.71828)");

        // Assert
        Assert.Equal(3.14159, Convert.ToDouble(result1));
        Assert.Equal(2.71828, Convert.ToDouble(result2));
    }

    [Fact]
    public async Task TestAbs_WithParameter_ReturnsCorrectValue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "negativeValue", -100.5 }
        };

        // Act
        var result = await Fx.EvaluateAsync("ABS(@negativeValue)", parameters);

        // Assert
        Assert.Equal(100.5, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestAbs_VeryLargeNumber_HandlesCorrectly()
    {
        // Arrange & Act
        var result = await Fx.EvaluateAsync("ABS(-9999999999.99)");

        // Assert
        Assert.Equal(9999999999.99, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestAbs_InvalidInput_ThrowsException()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "text", "not a number" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<FxValueException>(async () =>
        {
            await Fx.EvaluateAsync("ABS(@text)", parameters);
        });
    }

    #endregion

    #region Combined Math Operations Tests

    [Fact]
    public async Task TestCombinedMathOperations_RoundAndAbs()
    {
        // Arrange & Act
        var result = await Fx.EvaluateAsync("ROUND(ABS(-3.14159), 2)");

        // Assert
        Assert.Equal(3.14, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestComplexMathExpression_WithMultipleFunctions()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "values", new object[] { -2.7, 3.2, -1.8, 4.9 } }
        };

        // Act - Calculate sum of absolute values rounded to 1 decimal
        var result = await Fx.EvaluateAsync("ROUND(ABS(-2.7) + ABS(3.2) + ABS(-1.8) + ABS(4.9), 1)", parameters);

        // Assert
        Assert.Equal(12.6, Convert.ToDouble(result)); // 2.7 + 3.2 + 1.8 + 4.9 = 12.6
    }

    [Fact]
    public async Task TestMathFunctions_Performance()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - Perform 1000 operations
        for (int i = 0; i < 1000; i++)
        {
            await Fx.EvaluateAsync($"ROUND(ABS({i * 0.123 - 50}), 2)");
        }

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 2000, $"Math functions performance too slow: {stopwatch.ElapsedMilliseconds}ms");
    }

    #endregion
}