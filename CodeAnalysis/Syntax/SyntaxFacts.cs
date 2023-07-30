namespace CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.ExclamationToken => 8,
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 8,
            _ => 0
        };

    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.SlashToken or SyntaxKind.AsteriskToken => 7,
            SyntaxKind.MinusToken or SyntaxKind.PlusToken => 6,

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