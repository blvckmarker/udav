using CodeAnalysis.Text;
using SyntaxKind = CodeAnalysis.Syntax.SyntaxKind;

namespace Tests;
public class LexicalAnalysisTest
{
    [Fact]
    public static void CorrectTextTest()
    {
        var source = Utils.GenerateRandomBooleanSequence();

        var expected = Utils.GetDescendantTokens(source)
                            .MapTokensToBasic();
        var actual = Utils.GetTokens(source)
                          .MapTokensToBasic();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void AllTokensTest()
    {
        var source = "+ - * / ( ) ! && & || | ^ = == != ~ <= > >= % < a 0";

        var expected = Utils.GetTokens(source).MapTokensToBasic().ToList();

        var actual = typeof(SyntaxKind).GetEnumNames()
                                       .Where(x => x.EndsWith("Token"))
                                       .SkipLast(3) //eof bad whitespace
                                       .ToList();

        Assert.True(actual.Count() == expected.Count(), "Size of `expected` not equal to `actual`");

        for (int i = 0; i < actual.Count; i++)
            Assert.Equal(expected[i].Kind.ToString(), actual[i]);
    }

    [Fact]
    public static void AllKeywordsTest()
    {
        var source = "true false int let bool if else";
        var expected = Utils.GetTokens(source).MapTokensToBasic().ToList();
        var actual = typeof(SyntaxKind).GetEnumNames()
                                       .Where(x => x.EndsWith("Keyword"))
                                       .ToList();
        Assert.True(expected.Count == actual.Count(), "Size of `expected` not equal to `actual`");

        for (int i = 0; i < actual.Count; ++i)
            Assert.Equal(expected[i].Kind.ToString(), actual[i]);
    }


    [Fact]
    public static void EmptyTextTest()
    {
        var expected = new List<BasicTokenModel>();
        var actual = Utils.GetTokens(" ").MapTokensToBasic();

        Assert.Equal(actual, expected);
    }

    [Fact]
    public static void WrongLiteralTest1()
    {
        var expected = new List<BasicTokenModel>
            {
                new(SyntaxKind.NumericToken, "111"),
                new(SyntaxKind.IdentifierToken, "a"),
                new(SyntaxKind.MinusToken, "-"),
                new(SyntaxKind.PlusToken, "+"),
            };

        var actual = Utils.GetTokens("111a -+").MapTokensToBasic();

        Assert.Equal(actual, expected);
    }

    [Fact]
    public static void WrongLiteralTest2()
    {
        var primaryExpected = new List<BasicTokenModel>
            {
                new(SyntaxKind.IdentifierToken, "abc"),
            };
        var actual = Utils.GetTokens("abc").MapTokensToBasic();
        Assert.Equal(actual, primaryExpected);
    }

    [Fact]
    public static void LexicalException1()
    {
        var lexer = new Lexer(SourceText.From("Hello world!"));
        lexer.LexAll();

        Assert.Throws<Exception>(lexer.Lex);
    }

    [Fact]
    public static void LexicalException2()
    {
        var lexer = new Lexer(SourceText.From("Hello world!"));
        lexer.LexAll();

        Assert.Throws<Exception>(lexer.LexAll);
    }

    [Fact]
    public static void RandomNumericalTest()
    {
        var source = Utils.GenerateRandomNumericalSequence<int>();

        var actual = Utils.GetTokens(source)
                          .MapTokensToBasic();
        var expected = Utils.GetDescendantTokens(source)
                            .MapTokensToBasic();

        Assert.Equal(actual, expected);
    }

    [Fact]
    public static void RandomBooleanTest()
    {
        var source = Utils.GenerateRandomBooleanSequence();

        var actual = Utils.GetTokens(source)
                          .MapTokensToBasic();
        var expected = Utils.GetDescendantTokens(source)
                            .MapTokensToBasic();

        Assert.Equal(actual, expected);
    }

}
