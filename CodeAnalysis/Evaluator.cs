using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;

namespace CodeAnalysis;

public class Evaluator
{
    private readonly BoundStatement _root;
    private IDictionary<VariableSymbol, object> _localVariables;

    public IDictionary<VariableSymbol, object> LocalVariables => _localVariables;

    public Evaluator(BoundStatement root, IDictionary<VariableSymbol, object> sessionVariables)
    {
        _root = root;
        _localVariables = sessionVariables;
    }

    public object Evaluate() => EvaluateStatement(_root);

    // statements
    private object EvaluateStatement(BoundNode node)
    {
        if (node is BoundAssignmentStatement assign)
            return EvaluateAssignmentStatement(assign);
        if (node is BoundAssignmentExpressionStatement assignWrap)
            return EvaluateAssignmentExpressionStatement(assignWrap);

        throw new Exception("Unexpected statement " + node.Kind);
    }

    private object EvaluateAssignmentExpressionStatement(BoundAssignmentExpressionStatement assignWrap)
    {
        var value = EvaluateExpression(assignWrap.BoundExpression);
        _localVariables[assignWrap.BoundIdentifier.Reference] = value;

        return value;
    }

    private object EvaluateAssignmentStatement(BoundAssignmentStatement assign)
    {
        var newVariable = assign.BoundIdentifier;
        var value = EvaluateExpression(assign.BoundExpression);
        _localVariables.Add(newVariable, value);

        return value;
    }

    // expressions
    private object EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression literal)
            return EvaluateLiteralExpression(literal);

        if (node is BoundNameExpression name)
            return EvaluateNameExpression(name);

        if (node is BoundAssignmentExpression assignment)
            return EvaluateAssignmentExpression(assignment);

        if (node is BoundUnaryExpression unary)
            return EvaluateUnaryExpression(unary);

        if (node is BoundBinaryExpression binary)
            return EvaluateBinaryExpression(binary);

        if (node is BoundParenthesizedExpression parenExpression)
            return EvaluateParenthesizedExpression(parenExpression);

        throw new Exception($"Unexpected expression {node.Kind}");
    }

    private object EvaluateParenthesizedExpression(BoundParenthesizedExpression parenExpression)
    {
        return EvaluateExpression(parenExpression.BoundExpression);
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression binary)
    {
        var left = EvaluateExpression(binary.Left);
        var right = EvaluateExpression(binary.Right);

        return binary.OperatorToken.BoundKind switch
        {
            BoundBinaryOperatorKind.Addition => (int)left + (int)right,
            BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
            BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
            BoundBinaryOperatorKind.Division => (int)left / (int)right,
            BoundBinaryOperatorKind.DivisionRemainder => (int)left % (int)right,

            BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
            BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
            BoundBinaryOperatorKind.Equals => left.Equals(right),
            BoundBinaryOperatorKind.NotEqual => !left.Equals(right),
            BoundBinaryOperatorKind.GreaterOrEqual => (int)left >= (int)right,
            BoundBinaryOperatorKind.GreaterThan => (int)left > (int)right,
            BoundBinaryOperatorKind.LessOrEqual => (int)left <= (int)right,
            BoundBinaryOperatorKind.LessThan => (int)left < (int)right,

            BoundBinaryOperatorKind.BitwiseAnd => (int)left & (int)right,
            BoundBinaryOperatorKind.BitwiseOr => (int)left | (int)right,
            BoundBinaryOperatorKind.BitwiseXor => (int)left ^ (int)right,

            _ => throw new InvalidOperationException($"Unexpected binary operator {binary.OperatorToken}")
        };
    }

    private object EvaluateUnaryExpression(BoundUnaryExpression unary)
    {
        var operand = EvaluateExpression(unary.Operand);

        return unary.OperatorToken.BoundKind switch
        {
            BoundUnaryOperatorKind.Negation => -(int)operand,
            BoundUnaryOperatorKind.Identity => (int)operand,
            BoundUnaryOperatorKind.BitwiseNot => ~(int)operand,

            BoundUnaryOperatorKind.LogicalNot => !(bool)operand,

            _ => throw new InvalidOperationException($"Unexpected unary operator {unary.OperatorToken}"),
        };
    }

    private object EvaluateAssignmentExpression(BoundAssignmentExpression assignment)
    {
        var rightExpression = EvaluateExpression(assignment.BoundExpression);
        _localVariables[assignment.BoundIdentifier.Reference] = rightExpression;

        return rightExpression;
    }

    private object EvaluateNameExpression(BoundNameExpression name)
    {
        return _localVariables[name.Reference];
    }

    private object EvaluateLiteralExpression(BoundLiteralExpression literal)
    {
        return literal.Value;
    }
}