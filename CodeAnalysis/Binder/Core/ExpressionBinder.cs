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
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.NameExpression:
                return BindNameExpression((NameExpressionSyntax)syntax);
            case SyntaxKind.DeclaredVariableExpression:
                return BindDeclaredVariableExpression((DeclaredVariableExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.ParenthesizedExpression:
                return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
            default:
                throw new NotSupportedException(syntax.Kind.ToString());
        }
    }

    private partial BoundExpression BindDeclaredVariableExpression(DeclaredVariableExpressionSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        var declaredVariable = _sessionVariables.Keys.FirstOrDefault(x => x.Name == name);

        if (declaredVariable is not null)
        {
            _diagnostics.MakeIssue($"Local variable is already defined", name, syntax.Identifier.StartPosition, IssueKind.Problem);
            var variableSymbol = new VariableSymbol(name, typeof(int));
            return new BoundDeclaredVariableExpression(variableSymbol);
        }

        var variable = new VariableSymbol(name, null);
        return new BoundDeclaredVariableExpression(variable);
    }

    private partial BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        var varReference = _sessionVariables.Keys.FirstOrDefault(x => x.Name == name);
        if (varReference is null)
        {
            _diagnostics.MakeIssue($"Undefined local variable", name, syntax.Identifier.StartPosition, IssueKind.Problem);
            return new BoundLiteralExpression(0);
        }
        return new BoundNameExpression(varReference);
    }

    private partial BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        return new BoundParenthesizedExpression(boundExpression);
    }
    private partial BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var operand = BindExpression(syntax.Operand);
        var operatorToken = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);

        if (operatorToken is null)
        {
            _diagnostics.MakeIssue($"Unary operator {syntax.OperatorToken.Text} is not defined for type {operand.Type}", _sourceProgram[syntax.StartPosition..syntax.EndPosition], syntax.StartPosition);
            return operand;
        }

        return new BoundUnaryExpression(operatorToken, operand);
    }
    private partial BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);
        var operatorToken = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);

        if (operatorToken is null)
        {
            _diagnostics.MakeIssue($"Unknown operator `{syntax.OperatorToken.Kind}` for types `{left.Type}` and `{right.Type}`", _sourceProgram[syntax.StartPosition..syntax.EndPosition], syntax.StartPosition);
            return left;
        }

        return new BoundBinaryExpression(left, operatorToken, right);
    }

    private partial BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

}
