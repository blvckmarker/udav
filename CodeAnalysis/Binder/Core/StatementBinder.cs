using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;

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

    private partial BoundAssignmentExpressionStatement BindAssignmentExpressionStatement(AssignmentExpressionStatementSyntax syntax)
    {
        var asExpressionSyntax = new AssignmentExpressionSyntax(syntax.Variable, syntax.EqualsToken, syntax.Expression);
        var boundAssignmentExpression = BindAssignmentExpression(asExpressionSyntax);

        var expression = boundAssignmentExpression.BoundExpression;
        var identifier = boundAssignmentExpression.BoundIdentifier;
        if (expression.Type != identifier.Type)
            _diagnostics.MakeIssue($"Cannot cast type {expression.Type} to {identifier.Type}", _sourceProgram.ToString(syntax.Expression.Span), syntax.Expression.Span);

        return new BoundAssignmentExpressionStatement(boundAssignmentExpression.BoundIdentifier, boundAssignmentExpression.BoundExpression);
    }

    private partial BoundAssignmentStatement BindAssignmentStatement(AssignmentStatementSyntax statement)
    {
        var boundType = BoundIdentifierType.Bind(statement.TypeToken.Kind);
        var identifierName = statement.VariableToken.Text;

        if (_sessionVariables.Keys.FirstOrDefault(x => x.Name == identifierName) is not null)
        {
            _diagnostics.MakeIssue($"Local variable is already defined", identifierName, statement.VariableToken.Span);
            var errorSymbol = new VariableSymbol(identifierName, null);
            return new BoundAssignmentStatement(boundType, errorSymbol, null);
        }

        var boundExpression = BindExpression(statement.Expression);

        if (boundType.TypeKind is BoundTypeKind.DefinedType)
        {
            if (boundType.Type is null)
                _diagnostics.MakeIssue("Error type", statement.TypeToken.Text, statement.TypeToken.Span);
            if (boundType.Type != boundExpression.Type)
                _diagnostics.MakeIssue($"Cannot cast type {boundExpression.Type} to {boundType.Type}", _sourceProgram.ToString(statement.Expression.Span), statement.Expression.Span);
        }

        var boundIdentifier = new VariableSymbol(identifierName, boundExpression.Type);

        return new BoundAssignmentStatement(boundType, boundIdentifier, boundExpression);
    }
}
