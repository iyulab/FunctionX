namespace FunctionX.Tests;

public class FxNotTests
{
    [Fact]
    public async Task TestNotFunction_WithTrue_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("NOT(true)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestNotFunction_WithFalse_ReturnsTrue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("NOT(false)", parameters);

        // Assert
        Assert.True((bool)result!);
    }

    [Fact]
    public async Task TestNotFunction_WithVariable()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
            {
                { "condition", true }
            };

        // Act
        var result = await Fx.EvaluateAsync("NOT(@condition)", parameters);

        // Assert
        Assert.False((bool)result!);
    }
}
