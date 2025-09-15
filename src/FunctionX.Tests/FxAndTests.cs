namespace FunctionX.Tests;

/// <summary>
/// FxTests 클래스는 Fx 클래스의 메서드를 테스트합니다.
/// 엑셀 함수의 사용을 모방하여 다양한 테스트를 구현합니다.
/// </summary>
public class FxAndTests
{
    [Fact]
    public async Task TestAndFunction_AllTrue_ReturnsTrue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("AND(true, true, true)", parameters);

        // Assert
        Assert.True((bool)result!);
    }

    [Fact]
    public async Task TestAndFunction_OneFalse_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("AND(true, false, true)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestAndFunction_WithVariables_ReturnsCorrectValue()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "condition1", true },
            { "condition2", false },
            { "condition3", true }
        };

        // Act
        var result = await Fx.EvaluateAsync("AND(@condition1, @condition2, @condition3)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestAndFunction_AllFalse_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("AND(false, false, false)", parameters);

        // Assert
        Assert.False((bool)result!);
    }

    [Fact]
    public async Task TestAndFunction_WithMixedTypes_ReturnsFalse()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>
        {
            { "var1", true },
            { "var2", "not a boolean" },
            { "var3", false }
        };

        await Assert.ThrowsAsync<FxValueException>(async () =>
        {
            var result = await Fx.EvaluateAsync("AND(@var1, @var2, @var3)", parameters);
        });
    }
}