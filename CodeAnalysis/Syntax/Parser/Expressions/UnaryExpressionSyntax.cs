using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Expressions;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public ExpressionSyntax Operand { get; }
    public SyntaxToken OperatorToken { get; }

    public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

    public override TextSpan Span => TextSpan.FromBounds(Operand.Span.Start, Operand.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }

}