using CodeAnalysis.Lexer.Model;

namespace CodeAnalysis.Parser.Expressions.AST;

internal static class SyntaxFacts
{
    public static int GetBinaryOperatorPrecedence(this SyntaxKind kind) => kind switch
    {
        SyntaxKind.SlashToken or SyntaxKind.StarToken => 2,
        SyntaxKind.Minus or SyntaxKind.Plus => 1,
        _ => 0
    };
    public static int GetUnaryOperatorPrecedence(this SyntaxKind kind) => kind switch
    {
        SyntaxKind.Minus or SyntaxKind.Plus => 3,
        _ => 0
    };

    
}