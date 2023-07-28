namespace Tests.BasicParserTest;

public class SharedTest
{
    [Fact]
    public static void SimpleTest()
    {
        var source = "1 + 1 * 1";

        var actual = Utils.EvaluateInternal<int>(source);

        Assert.Equal(2, actual);
    }

    [Fact]
    public static void RandomNumericalTest()
    {
        var source = Utils.GenerateRandomNumericalSequence<int>();
        var actual = Utils.EvaluateInternal<int>(source);
        var expected = Utils.EvaluateExternal<int>(source);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void RandomBooleanTest()
    {
        var source = Utils.GenerateRandomBooleanSequence();
        var actual = Utils.EvaluateInternal<bool>(source);
        var expected = Utils.EvaluateExternal<bool>(source);

        Assert.Equal(expected, actual);
    }
}
