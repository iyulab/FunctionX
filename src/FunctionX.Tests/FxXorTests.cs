namespace FunctionX.Tests;

public class FxXorTests
{
    [Fact]
    public async Task TestXorFunction_WithOneTrueOneFalse_ReturnsTrue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("XOR(true, false)", parameters);

        // Assert
        Assert.True((bool)result!);
    }

    [Fact]
    public async Task TestXorFunction_WithTwoTrue_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("XOR(true, true)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestXorFunction_WithMultipleConditions()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("XOR(true, false, false, true)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestXorFunction_WithVariables()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
            {
                { "condition1", true },
                { "condition2", false },
                { "condition3", true }
            };

        // Act
        var result = await Fx.EvaluateAsync("XOR(@condition1, @condition2, @condition3)", parameters);

        // Assert
        Assert.False((bool)result!);
    }
}
