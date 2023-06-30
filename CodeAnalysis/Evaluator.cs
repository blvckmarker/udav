#region

using CodeAnalysis.Parser.Binder;
using CodeAnalysis.Parser.Expressions;

#endregion

namespace CodeAnalysis;

internal class Evaluator
{
    private readonly BoundExpression _root;

    public Evaluator(BoundExpression root)
    {
        _root = root;
    }

    public int Evaluate() => EvaluateExpression(_root);

    private int EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression number)
            return (int)number.Value;

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.OperatorToken)
            {
                case BoundUnaryOperatorKind.Negation:
                    return -operand;
                case BoundUnaryOperatorKind.Identity:
                    return operand;
                default:
                    throw new InvalidOperationException($"Unexpected unary expression {u.OperatorToken}");
            }
        }

        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            return b.OperatorToken switch
            {
                BoundBinaryOperatorKind.Addition => left + right,
                BoundBinaryOperatorKind.Subtraction => left - right,
                BoundBinaryOperatorKind.Multiplication => left * right,
                BoundBinaryOperatorKind.Division => left / right,
                _ => throw new InvalidOperationException($"Unexpected binary operator {b.OperatorToken}")
            };
        }

        if (node is BoundParenthesizedExpression p)
            return EvaluateExpression(p.BoundExpression);

        throw new Exception($"Unexpected node {node.Kind}");
    }
}