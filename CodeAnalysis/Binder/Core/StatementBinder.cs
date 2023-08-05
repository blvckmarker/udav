using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder.Core;

public sealed partial class Binder
{
    private partial BoundStatement BindStatement(StatementSyntax syntax)
        => syntax.Kind switch
        {
            SyntaxKind.AssignmentStatement => BindAssignmentStatement((AssignmentStatementSyntax)syntax),
            SyntaxKind.AssignmentExpressionStatement => BindAssignmentExpressionStatement((AssignmentExpressionStatementSyntax)syntax),
            _ => throw new NotSupportedException(syntax.Kind.ToString())
        };

    private partial BoundStatement BindAssignmentExpressionStatement(AssignmentExpressionStatementSyntax syntax)
    {
        var expressionSyntax = new AssignmentExpressionSyntax(syntax.Identifier, syntax.EqualsToken, syntax.Expression);
        var assignmentExpression = (BoundAssignmentExpression)BindAssignmentExpression(expressionSyntax);
        return new BoundAssignmentExpressionStatement(assignmentExpression.BoundIdentifier, assignmentExpression.BoundExpression);
    }

    private partial BoundStatement BindAssignmentStatement(AssignmentStatementSyntax statement)
    {
        var boundType = BoundIdentifierType.Bind(statement.TypeToken.Kind);
        var identifierName = statement.IdentifierToken.Text;

        if (_sessionVariables.Keys.FirstOrDefault(x => x.Name == identifierName) is not null)
        {
            _diagnostics.MakeIssue($"Local variable is already defined", identifierName, statement.IdentifierToken.StartPosition, IssueKind.Problem);
            var errorSymbol = new VariableSymbol(identifierName, null);
            return new BoundAssignmentStatement(boundType, errorSymbol, null);
        }

        var boundExpression = BindExpression(statement.Expression);

        if (boundType.TypeKind is BoundTypeKind.DefinedType)
        {
            if (boundType.Type is null)
                _diagnostics.MakeIssue("Error type", statement.TypeToken.Text, statement.TypeToken.StartPosition);
            if (boundType.Type != boundExpression.Type)
                _diagnostics.MakeIssue($"Cannot cast type {boundExpression.Type} to {boundType.Type}", _sourceProgram[statement.StartPosition..statement.EndPosition], statement.StartPosition);
        }

        var boundIdentifier = new VariableSymbol(identifierName, boundExpression.Type);

        return new BoundAssignmentStatement(boundType, boundIdentifier, boundExpression);
    }
}
