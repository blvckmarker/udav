using System.Xml.Serialization;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Binder.Scopes;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Parser.Statements;

namespace CodeAnalysis.Binder.Core;

public sealed partial class Binder
{
    private partial BoundStatement BindStatement(StatementSyntax syntax)
        => syntax.Kind switch
        {
            SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
            SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
            SyntaxKind.AssignmentStatement => BindAssignmentStatement((AssignmentStatementSyntax)syntax),
            SyntaxKind.AssignmentExpressionStatement => BindAssignmentExpressionStatement((AssignmentExpressionStatementSyntax)syntax),
            _ => throw new NotSupportedException(syntax.Kind.ToString())
        };

    private partial BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
    {
        var boundStatements = new List<BoundStatement>();
        _currScope = new BoundStatementScope(_currScope);

        foreach (var statement in syntax.Statements)
        {
            var boundStatement = BindStatement(statement);
            boundStatements.Add(boundStatement);
        }

        _currScope = _currScope.Parent;
        return new BoundBlockStatement(boundStatements);
    }
    /*
    {
        var a = 0
        {
            var b = 1
            {
                var c = a + b
            }
            var d = a
        }
    }
    */
    private partial BoundIfStatement BindIfStatement(IfStatementSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        if (boundExpression.Type != typeof(bool))
            _diagnostics.MakeIssue("Condition expression must be boolean type", syntax.Expression.ToString(), syntax.Expression.Span);

        var boundStatement = BindStatement(syntax.Statement);
        BoundElseStatement boundElseStatement = null;
        if (syntax.ElseStatement is { })
            boundElseStatement = BindElseStatement(syntax.ElseStatement);

        return new BoundIfStatement(boundExpression, boundStatement, boundElseStatement);
    }

    private partial BoundElseStatement BindElseStatement(ElseStatementSyntax syntax)
    {
        var statement = BindStatement(syntax.Statement);
        return new BoundElseStatement(statement);
    }

    private partial BoundAssignmentExpressionStatement BindAssignmentExpressionStatement(AssignmentExpressionStatementSyntax syntax)
    {
        var asExpressionSyntax = new AssignmentExpressionSyntax(syntax.Variable, syntax.EqualsToken, syntax.Expression);
        var boundAssignmentExpression = BindAssignmentExpression(asExpressionSyntax);

        var expression = boundAssignmentExpression.BoundExpression;
        var identifier = boundAssignmentExpression.BoundIdentifier;

        if (expression.Type != identifier.Type)
            _diagnostics.MakeIssue($"Cannot cast type {expression.Type} to {identifier.Type}", syntax.Expression.ToString(), syntax.Expression.Span);

        return new BoundAssignmentExpressionStatement(boundAssignmentExpression.BoundIdentifier, boundAssignmentExpression.BoundExpression);
    }
    // TODO: Override ToString() method in syntax nodes
    private partial BoundAssignmentStatement BindAssignmentStatement(AssignmentStatementSyntax statement)
    {
        var primaryType = BoundIdentifierType.Bind(statement.TypeToken.Kind);
        var identifierName = statement.VariableToken.Text;

        if (_currScope.TryGetValueOf(identifierName, out var _))
        {
            _diagnostics.MakeIssue($"Local variable is already defined", identifierName, statement.VariableToken.Span);
            var errorSymbol = new VariableSymbol(identifierName, null);
            return new BoundAssignmentStatement(errorSymbol, null);
        }

        var boundExpression = BindExpression(statement.Expression);

        if (primaryType.TypeKind is BoundTypeKind.DefinedType)
        {
            if (primaryType.Type is null)
                _diagnostics.MakeIssue("Error type", statement.TypeToken.Text, statement.TypeToken.Span);
            if (primaryType.Type != boundExpression.Type)
                _diagnostics.MakeIssue($"Cannot cast type {boundExpression.Type} to {primaryType.Type}", statement.Expression.ToString(), statement.Expression.Span);
        }

        var boundIdentifier = new VariableSymbol(identifierName, boundExpression.Type);
        _currScope.TryDeclareVariable(boundIdentifier);

        return new BoundAssignmentStatement(boundIdentifier, boundExpression);
    }
}
