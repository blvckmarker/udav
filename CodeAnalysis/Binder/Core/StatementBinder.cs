using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Statements;

namespace CodeAnalysis.Binder;

public sealed partial class Binder
{
    private partial BoundStatement BindStatement(StatementSyntax syntax)
        => syntax.Kind switch
        {
            SyntaxKind.AssignmentStatement => BindAssignmentStatement((AssignmentStatementSyntax)syntax),
            _ => throw new NotSupportedException(syntax.Kind.ToString())
        };
    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement)
    {
        var boundType = BoundIdentifierType.Bind(statement.TypeToken.Kind);
        var boundIdentifier = (BoundDeclaredVariableExpression)BindExpression(statement.IdentifierName);
        var boundExpression = BindExpression(statement.Expression);

        if (boundType.TypeKind is BoundTypeKind.DefinedType)
        {
            if (boundType.Type is null)
                _diagnostics.MakeIssue("Invalid type", statement.TypeToken.Text, statement.TypeToken.StartPosition);
            if (boundType.Type != boundExpression.Type)
                _diagnostics.MakeIssue($"Cannot cast type {boundExpression.Type} to {boundType.Type}", _sourceProgram[statement.StartPosition..statement.EndPosition], statement.StartPosition);
        }

        boundIdentifier.Variable.Type = boundExpression.Type;
        return new BoundAssignmentStatement(boundType, boundIdentifier, boundExpression);
    }
}
