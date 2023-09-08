using System.Reflection;

namespace Tests;

public class EvaluatorTest
{
    [Fact]
    public static void SimpleTest()
    {
        var source = "1 + 1 * 1 - 10";

        var actual = Utils.EvaluateExpressionInternal<int>(source);

        Assert.Equal(-8, actual);
    }

    [Fact]
    public static void RandomNumericalTest()
    {
        var source = Utils.GenerateRandomNumericalSequence<int>();
        int actual;
        try
        {
            actual = Utils.EvaluateExpressionInternal<int>(source);
        }
        catch (TargetInvocationException)
        {
            RandomNumericalTest();
            return;
        }
        int expected = Utils.EvaluateExpressionExternal<int>(source);


        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void RandomBooleanTest()
    {
        var source = Utils.GenerateRandomBooleanSequence();
        var actual = Utils.EvaluateExpressionInternal<bool>(source);
        var expected = Utils.EvaluateExpressionExternal<bool>(source);

        Assert.Equal(expected, actual);
    }
}
