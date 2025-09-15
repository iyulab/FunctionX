namespace FunctionX.Tests;

/// <summary>
/// FxTests 클래스는 Fx 클래스의 메서드를 테스트합니다.
/// 엑셀 함수의 사용을 모방하여 다양한 테스트를 구현합니다.
/// </summary>
public class FxCountTests
{
    [Fact]
    public async Task TestEvaluateCountWithConstants()
    {
        // Arrange
        var results = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("COUNT(1, 2, 3, 4, 5)", results);

        // Assert
        Assert.Equal(5.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestEvaluateCountWithVariable()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "countValues", new object[] {1, 2, 3, 4, 5} }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@countValues)", results);

        // Assert
        Assert.Equal(5.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestEvaluateCountWithMixedTypes()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "mixedValues", new object?[] {1, null, "apple", "", 5} }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@mixedValues)", results);

        // Assert
        Assert.Equal(2.0, Convert.ToDouble(result)); // 여기서는 숫자만 계산
    }

    [Fact]
    public async Task TestEvaluateCountWithAllStrings()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "stringValues", new object?[] {"apple", "banana", null, "cherry"} }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@stringValues)", results);

        // Assert
        // COUNT 함수는 숫자만 계산
        Assert.Equal(0.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestEvaluateCountWithEmptyArray()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "emptyArray", Array.Empty<object>() }
        };

        // Act
        var result = await Fx.EvaluateAsync("COUNT(@emptyArray)", results);

        // Assert
        Assert.Equal(0.0, Convert.ToDouble(result));
    }
}