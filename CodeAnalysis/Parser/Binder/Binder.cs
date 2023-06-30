#region

using CodeAnalysis.Lexer.Model;
using CodeAnalysis.Parser.Expressions;

#endregion

namespace CodeAnalysis.Parser.Binder;

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
        var operatorToken = BindUnaryOperatorKind(syntax.OperatorToken.Kind, operand.Type);

        if (operatorToken is null)
            _diagnostics.Add($"Unary operator {operatorToken.ToString()} is not defined for type {operand.Type}");

        return new BoundUnaryExpression(operatorToken, operand);
    }

    private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind operatorTokenKind, Type operandType)
    {
        if (operandType != typeof(int))
            return null;

        return operatorTokenKind switch
        {
            SyntaxKind.Minus => BoundUnaryOperatorKind.Negation,
            SyntaxKind.Plus => BoundUnaryOperatorKind.Identity,
            _ => throw new InvalidOperationException($"Unexpected unary operator {operatorTokenKind}")
        };
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var operatorToken = BindBinaryOperatorKind(syntax.OperatorToken.Kind, left.Type);
        var right = BindExpression(syntax.Right);

        if (operatorToken is null)
        {
            _diagnostics.Add($"Unknown type for left expression {left.Type}");
            return left;
        }

        return new BoundBinaryExpression(left, operatorToken, right);
    }

    private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind operatorTokenKind, Type leftType)
    {
        if (leftType != typeof(int))
            return null;

        return operatorTokenKind switch
        {
            SyntaxKind.Minus => BoundBinaryOperatorKind.Subtraction,
            SyntaxKind.Plus => BoundBinaryOperatorKind.Addition,
            SyntaxKind.SlashToken => BoundBinaryOperatorKind.Division,
            SyntaxKind.StarToken => BoundBinaryOperatorKind.Multiplication,
            _ => throw new Exception($"Unexpected operator token kind {operatorTokenKind}")
        };
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.LiteralToken.Value as int? ?? 0;
        return new BoundLiteralExpression(value);
    }
}