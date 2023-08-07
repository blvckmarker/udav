using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Expressions;
public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
    public AssignmentExpressionSyntax(VariableExpressionSyntax name, SyntaxToken equalsToken, ExpressionSyntax expression)
    {
        Variable = name;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

    public VariableExpressionSyntax Variable { get; }
    public SyntaxToken EqualsToken { get; }
    public ExpressionSyntax Expression { get; }

    public override TextSpan Span => TextSpan.FromBounds(Variable.Span.Start, Expression.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Variable;
        yield return EqualsToken;
        yield return Expression;
    }
}
