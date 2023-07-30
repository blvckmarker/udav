using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;

namespace CodeAnalysis.Binder;

public sealed partial class Binder
{
    public partial BoundStatement BindStatement(StatementSyntax syntax)
        => syntax.Kind switch
        {
            SyntaxKind.AssignmentStatement => BindAssignmentStatement(syntax as AssignmentStatementSyntax),
            _ => throw new NotSupportedException(syntax.Kind.ToString())
        };
    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement)
    {
        var boundExpression = BindExpression(statement.Expression);
        return new BoundAssignmentStatement(statement.NameExpression as LiteralExpressionSyntax, boundExpression);
    }
}
