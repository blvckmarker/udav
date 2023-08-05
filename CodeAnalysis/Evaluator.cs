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

    private object EvaluateStatement(BoundNode node)
    {
        if (node is BoundAssignmentStatement assign)
        {
            var newVariable = assign.BoundIdentifier;
            var value = EvaluateExpression(assign.BoundExpression);
            _localVariables.Add(newVariable, value);

            return value;
        }
        if (node is BoundAssignmentExpressionStatement assignWrap)
        {
            var value = EvaluateExpression(assignWrap.BoundExpression);
            _localVariables[assignWrap.BoundIdentifier.Reference] = value;

            return value;
        }

        throw new Exception("Unexpected bound node " + node.Kind);
    }

    private object EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression literal)
            return literal.Value;

        if (node is BoundNameExpression name)
            return _localVariables[name.Reference];

        if (node is BoundAssignmentExpression assignment)
        {
            var rightExpression = EvaluateExpression(assignment.BoundExpression);
            _localVariables[assignment.BoundIdentifier.Reference] = rightExpression;

            return rightExpression;
        }

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            return u.OperatorToken.BoundKind switch
            {
                BoundUnaryOperatorKind.Negation => -(int)operand,
                BoundUnaryOperatorKind.Identity => (int)operand,
                BoundUnaryOperatorKind.BitwiseNot => ~(int)operand,

                BoundUnaryOperatorKind.LogicalNot => !(bool)operand,

                _ => throw new InvalidOperationException($"Unexpected unary operator {u.OperatorToken}"),
            };
        }

        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            return b.OperatorToken.BoundKind switch
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

                _ => throw new InvalidOperationException($"Unexpected binary operator {b.OperatorToken}")
            };
        }

        if (node is BoundParenthesizedExpression p)
            return EvaluateExpression(p.BoundExpression);

        throw new Exception($"Unexpected bound node {node.Kind}");
    }
}