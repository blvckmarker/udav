namespace Tests.Eval;

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
        int actual = 0;
        int expected = -1;
        try
        {
            actual = Utils.EvaluateInternal<int>(source);
            expected = Utils.EvaluateExternal<int>(source);
        }
        catch (DivideByZeroException)
        {
            RandomNumericalTest();
        }


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
