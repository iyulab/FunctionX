namespace FunctionX.Tests;

/// <summary>
/// FxTests 클래스는 Fx 클래스의 메서드를 테스트합니다.
/// 엑셀 함수의 사용을 모방하여 다양한 테스트를 구현합니다.
/// </summary>
public class FxMinTests
{
    [Fact]
    public async Task TestEvaluateMinWithConstants()
    {
        // Arrange
        var results = new Dictionary<string, object?>();

        // Act
        var result = await Fx.EvaluateAsync("MIN(10, 20, 5, 15)", results);

        // Assert
        Assert.Equal(5.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestEvaluateMinWithVariable()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "minValues", new object[] {10, 5, 15, 20} }
        };

        // Act
        var result = await Fx.EvaluateAsync("MIN(@minValues)", results);

        // Assert
        Assert.Equal(5.0, Convert.ToDouble(result));
    }


    [Fact]
    public async Task TestEvaluateMinWithEmptyArray()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "emptyArray", Array.Empty<object>() }
        };

        // Act
        var result = await Fx.EvaluateAsync("MIN(@emptyArray)", results);

        // Assert
        Assert.Equal(double.NaN, Convert.ToDouble(result));
    }


    [Fact]
    public async Task TestEvaluateMinWithNullValues()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "nullValues", new object?[] {10, null, 5, null, 15} }
        };

        // Act
        var result = await Fx.EvaluateAsync("MIN(@nullValues)", results);

        // Assert
        // 배열에 null이 포함되어 있더라도 유효한 숫자 값만을 고려하여 최소값을 계산해야 합니다.
        Assert.Equal(5.0, Convert.ToDouble(result));
    }

    [Fact]
    public async Task TestEvaluateMinWithNaN()
    {
        // Arrange
        var results = new Dictionary<string, object?>
        {
            { "nanValues", new object[] {10, double.NaN, 5, double.NaN, 15} }
        };

        // Act
        var result = await Fx.EvaluateAsync("MIN(@nanValues)", results);

        // Assert
        // NaN 값을 포함하는 배열에서 최소값은 NaN이어야 합니다.
        Assert.True(double.IsNaN(Convert.ToDouble(result)));
    }

}