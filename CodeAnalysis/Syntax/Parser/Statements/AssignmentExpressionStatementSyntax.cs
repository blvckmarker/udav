using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Statements;
public sealed class AssignmentExpressionStatementSyntax : StatementSyntax
{
    public AssignmentExpressionStatementSyntax(VariableExpressionSyntax identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
    {
        Variable = identifier;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public override SyntaxKind Kind => SyntaxKind.AssignmentExpressionStatement;

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
