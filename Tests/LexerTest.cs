#region

using CodeAnalysis.Lexer;
using CodeAnalysis.Lexer.Model;

#endregion

namespace Tests;

public class LexerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CorrectTextTest()
    {
        var expected = new List<TokenModel>
        {
            new(SyntaxKind.LiteralExpression, "111"),
            new(SyntaxKind.Plus, "+"),
            new(SyntaxKind.Minus, "-"),
            new(SyntaxKind.LiteralExpression, "1"),
        };
        var actual = MapTokens(GetTokens("111   + -1"));
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void EmptyTextTest()
    {
        var expected = new List<TokenModel>();
        var actual = MapTokens(GetTokens(" "));

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void WrongLiteralTest1()
    {
        var expected = new List<TokenModel>
        {
            new(SyntaxKind.LiteralExpression, "111"),
            new(SyntaxKind.BadToken, "a"),
            new(SyntaxKind.Minus, "-"),
            new(SyntaxKind.Plus, "+"),
        };
        var actual = MapTokens(GetTokens("111a -+"));

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void WrongLiteralTest2()
    {
        var expected = new List<TokenModel>
        {
            new(SyntaxKind.BadToken, "a"),
            new(SyntaxKind.BadToken, "b"),
            new(SyntaxKind.BadToken, "c"),
        };
        var actual = MapTokens(GetTokens("abc"));

        Assert.That(expected, Is.EqualTo(actual));
    }

    private static IEnumerable<TokenModel> MapTokens(IEnumerable<SyntaxToken> tokens)
        => tokens
            .SkipLast(1)
            .Select(token => new TokenModel(token.Kind, token.Text));

    private static IEnumerable<SyntaxToken> GetTokens(string source)
    {
        var lexer = new Lexer(source);
        var tokens = new List<SyntaxToken>();

        SyntaxToken currentToken;
        do
        {
            currentToken = lexer.Lex();

            if (currentToken.Kind is SyntaxKind.Whitespace)
                continue;

            tokens.Add(currentToken);
        } while (currentToken.Kind is not SyntaxKind.Eof);

        return tokens;
    }


    private record TokenModel(SyntaxKind Kind, string Text);
}