#region

using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Parser.Expressions;
using CodeAnalysis.Scanner.Model;

#endregion

namespace CodeAnalysis.Binder;

internal sealed class Binder
{
    private readonly List<string> _diagnostics = new();
    public IEnumerable<string> Diagnostics => _diagnostics;

    public BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression(syntax as LiteralExpressionSyntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression(syntax as BinaryExpressionSyntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression(syntax as UnaryExpressionSyntax);
            case SyntaxKind.ParenthesizedExpression:
                return BindParenthesizedExpression(syntax as ParenthesizedExpressionSyntax);
            default:
                throw new Exception($"Unknown syntax expression {syntax.Kind}");
        }
    }

    private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        return new BoundParenthesizedExpression(boundExpression);
    }
    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var operand = BindExpression(syntax.Operand);
        var operatorToken = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);

        if (operatorToken is null)
            _diagnostics.Add($"Unary operator {operatorToken} is not defined for type {operand.Type}");

        return new BoundUnaryExpression(operatorToken, operand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);
        var operatorToken = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);

        if (operatorToken is null)
        {
            _diagnostics.Add($"Unknown operator `{syntax.OperatorToken.Kind}` for types `{left.Type}` and `{right.Type}`");
            return left;
        }

        return new BoundBinaryExpression(left, operatorToken, right);
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
}