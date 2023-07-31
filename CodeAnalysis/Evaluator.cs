using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public class Evaluator
{
    private readonly BoundStatement _root;
    private readonly IDictionary<string, object> _localVariables;
    public Evaluator(BoundStatement root, IDictionary<string, object> localVariables)
    {
        _root = root;
        _localVariables = localVariables;
    }

    public object Evaluate() => EvaluateStatement(_root);

    private object EvaluateStatement(BoundNode node)
    {
        if (node is BoundAssignmentStatement assign)
        {
            var name = assign.IdentifierName.Text;
            var value = EvaluateExpression(assign.BoundExpression);
            if (!_localVariables.TryAdd(name, value))
                return new DiagnosticsBag($"Local variable `{name}` is already defined", IssueKind.Problem);
            return value;
        }
        throw new Exception("Unexpected bound node " + node.Kind);
    }

    private object EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression literal)
        {
            if (_localVariables.TryGetValue(literal.Value.ToString(), out var value))
                return value;
            return literal.Value;
        }

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            return u.OperatorToken.BoundKind switch
            {
                BoundUnaryOperatorKind.Negation => -(int)operand,
                BoundUnaryOperatorKind.Identity => (int)operand,
                BoundUnaryOperatorKind.LogicalNot => !(bool)operand,
                _ => throw new InvalidOperationException($"Unexpected unary expression {u.OperatorToken}"),
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

                BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
                BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,

                BoundBinaryOperatorKind.BitwiseAnd => (int)left & (int)right,
                BoundBinaryOperatorKind.BitwiseOr => (int)left | (int)right,
                BoundBinaryOperatorKind.BitwiseXor => (int)left ^ (int)right,

                _ => throw new InvalidOperationException($"Unexpected binary operator {b.OperatorToken}")
            };
        }

        if (node is BoundParenthesizedExpression p)
            return EvaluateExpression(p.BoundExpression);

        throw new Exception($"Unexpected node {node.Kind}");
    }
}