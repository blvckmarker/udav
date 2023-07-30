using CodeAnalysis.Parser.Syntax;
using CodeAnalysis.Scanner.Syntax;

namespace CodeAnalysis.Parser.Expressions;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public LiteralExpressionSyntax(SyntaxToken literalToken, object? value)
    {
        LiteralToken = literalToken;
        Value = value;
    }

    public LiteralExpressionSyntax(SyntaxToken literalToken)
        : this(literalToken, literalToken.Value) { }

    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
    public SyntaxToken LiteralToken { get; }
    public object? Value { get; }

    public override int StartPosition => LiteralToken.StartPosition;
    public override int EndPosition => LiteralToken.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}