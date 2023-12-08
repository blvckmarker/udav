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

            SyntaxKind.LessThanEqualToken or SyntaxKind.GreaterThanEqualToken => 7,
            SyntaxKind.LessThanToken or SyntaxKind.GreaterThanToken => 7,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.ExclamationEqualsToken => 6,

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
            "bool" => SyntaxKind.BoolKeyword,
            "int" => SyntaxKind.IntKeyword,
            "let" => SyntaxKind.LetKeyword,
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            _ => SyntaxKind.IdentifierToken
        };
}