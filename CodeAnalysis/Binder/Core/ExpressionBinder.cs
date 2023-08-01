using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binder;

public sealed partial class Binder
{
    private partial BoundExpression BindExpression(ExpressionSyntax syntax)
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
                throw new NotSupportedException(syntax.Kind.ToString());
        }
    }

    private partial BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        return new BoundParenthesizedExpression(boundExpression);
    }
    private partial BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var operand = BindExpression(syntax.Operand);

        if (operand.Kind == BoundNodeKind.LiteralExpression && (operand as BoundLiteralExpression).Value is null)
            return operand;

        var operatorToken = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);

        if (operatorToken is null)
            _diagnostics.MakeIssue($"Unary operator {syntax.OperatorToken.Text} is not defined for type {operand.Type}",
                                    _diagnostics.SourceText[syntax.StartPosition..syntax.EndPosition], syntax.StartPosition);

        return new BoundUnaryExpression(operatorToken, operand);
    }
    private partial BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);

        if ((left.Kind == BoundNodeKind.LiteralExpression && (left as BoundLiteralExpression).Value is null)
         || (right.Kind == BoundNodeKind.LiteralExpression && (right as BoundLiteralExpression).Value is null))
            return left;

        var operatorToken = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);

        if (operatorToken is null)
        {
            _diagnostics.MakeIssue($"Unknown operator `{syntax.OperatorToken.Kind}` for types `{left.Type}` and `{right.Type}`",
                                    _diagnostics.SourceText[syntax.StartPosition..syntax.EndPosition], syntax.StartPosition);
            return left;
        }

        return new BoundBinaryExpression(left, operatorToken, right);
    }
    private partial BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        switch (syntax.LiteralToken.Kind)
        {
            case SyntaxKind.NameExpression:
                {
                    if (!_sessionVariables.TryGetValue(syntax.LiteralToken.Text, out var localValue))
                    {
                        _diagnostics.MakeIssue($"Undefined local variable {syntax.Value}", syntax.LiteralToken.Text, syntax.LiteralToken.StartPosition, IssueKind.Problem);
                        return new BoundLiteralExpression(null);
                    }
                    return new BoundLiteralExpression(localValue);
                }

            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
                return new BoundLiteralExpression((bool)syntax.Value);

            default:
                var value = syntax.Value ?? new object();
                return new BoundLiteralExpression(value);
        }
    }

}
