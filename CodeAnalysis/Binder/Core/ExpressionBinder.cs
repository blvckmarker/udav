using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;

namespace CodeAnalysis.Binder.Core;

public sealed partial class Binder
{
    private partial BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
            SyntaxKind.VariableExpression => BindNameExpression((VariableExpressionSyntax)syntax),
            SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
            SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
            SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
            SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
            _ => throw new NotSupportedException(syntax.Kind.ToString()),
        };
    }

    private partial BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax expressionSyntax)
    {
        var boundIdentifier = BindNameExpression(expressionSyntax.Variable);
        var boundExpression = BindExpression(expressionSyntax.Expression);
        return new BoundAssignmentExpression(boundIdentifier, boundExpression);
    }
    private partial BoundVariableExpression BindNameExpression(VariableExpressionSyntax syntax)
    {
        var name = syntax.Variable.Text;
        if (!_currScope.TryGetValueOf(name, out var symbol))
        {
            _diagnostics.MakeIssue($"Undefined local variable", name, syntax.Span);
            var errorSymbol = new VariableSymbol(name, null);
            return new BoundVariableExpression(errorSymbol);
        }
        return new BoundVariableExpression(symbol);
    }

    private partial BoundParenthesizedExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        return new BoundParenthesizedExpression(boundExpression);
    }
    private partial BoundUnaryExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var operand = BindExpression(syntax.Operand);
        var operatorToken = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);

        if (operatorToken is null)
            _diagnostics.MakeIssue($"Unary operator {syntax.OperatorToken.Text} is not defined for type {operand.Type}", syntax.ToString(), syntax.Span);

        return new BoundUnaryExpression(operatorToken, operand);
    }
    private partial BoundBinaryExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);
        var operatorToken = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);

        if (operatorToken is null)
            _diagnostics.MakeIssue($"Unknown operator `{syntax.OperatorToken.Kind}` for types `{(left.Type is null ? "ErrorType" : left.Type)}` and `{(right.Type is null ? "ErrorType" : right.Type)}`", syntax.ToString(), syntax.Span);

        return new BoundBinaryExpression(left, operatorToken, right);
    }

    private partial BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

}
