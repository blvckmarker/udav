namespace CodeAnalysis.Syntax.Parser.Expressions;
public class VariableExpressionSyntax : ExpressionSyntax
{
    public VariableExpressionSyntax(SyntaxToken literalToken)
    {
        Variable = literalToken;
    }

    public SyntaxToken Variable { get; }
    public override SyntaxKind Kind => SyntaxKind.VariableExpression;
    public override int StartPosition => Variable.StartPosition;
    public override int EndPosition => Variable.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Variable;
    }
}
