namespace CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.ExclamationToken or SyntaxKind.TildeToken => 10,
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 10,
            _ => 0
        };

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.SlashToken or SyntaxKind.AsteriskToken or SyntaxKind.PercentToken => 9,
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 8,

            SyntaxKind.LessEqualToken or SyntaxKind.GreaterEqualToken => 7,
            SyntaxKind.LessToken or SyntaxKind.GreaterToken => 7,
            SyntaxKind.EqualEqualToken or SyntaxKind.ExclamationEqualToken => 6,

            SyntaxKind.AmpersandToken => 5,
            SyntaxKind.CaretToken => 4,
            SyntaxKind.PipeToken => 3,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.PipePipeToken => 1,
            _ => 0
        };

    public static SyntaxKind GetKeywordKind(string text)
        => text switch
        {
            "let" => SyntaxKind.LetKeyword,
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            _ => SyntaxKind.NameExpression
        };
}