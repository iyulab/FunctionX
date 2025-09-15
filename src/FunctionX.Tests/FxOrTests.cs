namespace FunctionX.Tests;

public class FxOrTests
{
    [Fact]
    public async Task TestOrFunction_AnyTrue_ReturnsTrue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("OR(true, false, false)", parameters);

        // Assert
        Assert.True((bool)result!);
    }

    [Fact]
    public async Task TestOrFunction_AllFalse_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("OR(false, false, false)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestOrFunction_WithVariables()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
            {
                { "condition1", false },
                { "condition2", true }
            };

        // Act
        var result = await Fx.EvaluateAsync("OR(@condition1, @condition2)", parameters);

        // Assert
        Assert.True((bool)result!);
    }
}
