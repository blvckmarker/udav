using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Expressions;
public sealed class VariableExpressionSyntax : ExpressionSyntax
{
    public VariableExpressionSyntax(SyntaxToken literalToken)
    {
        Variable = literalToken;
    }

    public SyntaxToken Variable { get; }
    public override SyntaxKind Kind => SyntaxKind.VariableExpression;

    public override TextSpan Span => TextSpan.FromBounds(Variable.Span.Start, Variable.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Variable;
    }
}
