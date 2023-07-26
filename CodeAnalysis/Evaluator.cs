#region

using CodeAnalysis.Parser.Binder;

#endregion

namespace CodeAnalysis;

internal class Evaluator
{
    private readonly BoundExpression _root;

    public Evaluator(BoundExpression root)
    {
        _root = root;
    }

    public object Evaluate() => EvaluateExpression(_root);

    private object EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression number)
            return number.Value;

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.OperatorToken)
            {
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                default:
                    throw new InvalidOperationException($"Unexpected unary expression {u.OperatorToken}");
            }
        }

        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            return b.OperatorToken.Kind switch
            {
                BoundBinaryOperatorKind.Addition => (int)left - (int)right,
                BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
                BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
                BoundBinaryOperatorKind.Division => (int)left / (int)right,
                _ => throw new InvalidOperationException($"Unexpected binary operator {b.OperatorToken}")
            };
        }

        if (node is BoundParenthesizedExpression p)
            return EvaluateExpression(p.BoundExpression);

        throw new Exception($"Unexpected node {node.Kind}");
    }
}