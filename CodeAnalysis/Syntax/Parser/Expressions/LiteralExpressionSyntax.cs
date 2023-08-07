using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Expressions;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public LiteralExpressionSyntax(SyntaxToken literalToken, object? value)
    {
        LiteralToken = literalToken;
        Value = value;
    }

    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
    public SyntaxToken LiteralToken { get; }
    public object? Value { get; }

    public override TextSpan Span => TextSpan.FromBounds(LiteralToken.Span.Start, LiteralToken.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}