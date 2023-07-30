namespace CodeAnalysis.Syntax.Parser.Expressions;

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken Open { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken Close { get; }

    public ParenthesizedExpressionSyntax(SyntaxToken open, ExpressionSyntax expression, SyntaxToken close)
    {
        Open = open;
        Expression = expression;
        Close = close;
    }

    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

    public override int StartPosition => Open.StartPosition;
    public override int EndPosition => Close.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Open;
        yield return Expression;
        yield return Close;
    }
}