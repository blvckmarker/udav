using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser.Statements;
public sealed class AssignmentStatementSyntax : StatementSyntax
{
    public AssignmentStatementSyntax(SyntaxToken typeToken,
                               SyntaxToken variable,
                               SyntaxToken equalToken,
                               ExpressionSyntax expression)
    {
        TypeToken = typeToken;
        VariableToken = variable;
        EqualToken = equalToken;
        Expression = expression;
    }

    public SyntaxToken TypeToken { get; }
    public SyntaxToken VariableToken { get; }
    public SyntaxToken EqualToken { get; }
    public ExpressionSyntax Expression { get; }

    public override SyntaxKind Kind => SyntaxKind.AssignmentStatement;

    public override TextSpan Span => TextSpan.FromBounds(TypeToken.Span.Start, Expression.Span.End);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return TypeToken;
        yield return VariableToken;
        yield return EqualToken;
        yield return Expression;
    }
}
