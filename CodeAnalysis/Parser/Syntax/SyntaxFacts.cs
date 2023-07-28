using CodeAnalysis.Scanner.Model;

namespace CodeAnalysis.Parser.Syntax;

internal static class SyntaxFacts
{
    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.SlashToken or SyntaxKind.AsteriskToken => 2,
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 1,
            _ => 0
        };

    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 3,
            _ => 0
        };

    public static SyntaxKind GetKeywordKind(string text)
        => text switch
        {
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            _ => SyntaxKind.LiteralExpression
        };
}