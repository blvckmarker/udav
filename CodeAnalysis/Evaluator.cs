using System.ComponentModel;
using System.Reflection.Metadata;
using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;

namespace CodeAnalysis;

public class Evaluator
{
    private readonly BoundNode _root;
    private object lastValue;

    public IDictionary<VariableSymbol, object> Variables { get; }

    public Evaluator(BoundNode root, IDictionary<VariableSymbol, object> _variables)
    {
        _root = root;
        Variables = _variables;
    }

    public object Evaluate()
    {
        EvaluateCompilationUnit((BoundCompilationUnit)_root);
        return lastValue;
    }

    private void EvaluateCompilationUnit(BoundCompilationUnit unit) => EvaluateStatement(unit.Statement);

    // statements
    private void EvaluateStatement(BoundNode node)
    {
        switch (node.Kind)
        {
            case BoundNodeKind.AssignmentStatement:
                {
                    var assign = (BoundAssignmentStatement)node;
                    EvaluateAssignmentStatement(assign);
                    break;
                }
            case BoundNodeKind.AssignmentExpressionStatement:
                {
                    var assignWrap = (BoundAssignmentExpressionStatement)node;
                    EvaluateAssignmentExpressionStatement(assignWrap);
                    break;
                }
            case BoundNodeKind.BlockStatement:
                {
                    var block = (BoundBlockStatement)node;
                    foreach (var statement in block.Statements)
                        EvaluateStatement(statement);
                    break;
                }
            case BoundNodeKind.ForStatement:
                {
                    var forStatement = (BoundForStatement)node;
                    EvaluateForStatement(forStatement);
                    break;
                }
            case BoundNodeKind.WhileStatement:
                {
                    var whileStatement = (BoundWhileStatement)node;
                    EvaluateWhileStatement(whileStatement);
                    break;
                }
            case BoundNodeKind.IfStatement:
                {
                    var ifStatement = (BoundIfStatement)node;
                    EvaluateIfStatement(ifStatement);
                    break;
                }
            default:
                throw new Exception("Unexpected statement " + node.Kind);
        }
    }

    private void EvaluateIfStatement(BoundIfStatement ifStatement)
    {
        var condition = (bool)EvaluateExpression(ifStatement.Expression);
        if (condition)
            EvaluateStatement(ifStatement.Statement);
        else if (ifStatement.ElseStatement is { })
            EvaluateStatement(ifStatement.ElseStatement.Statement);
    }

    private void EvaluateWhileStatement(BoundWhileStatement whileStatement)
    {
        var condition = (bool)EvaluateExpression(whileStatement.Condition);
        while (condition)
        {
            EvaluateStatement(whileStatement.Statement);
            condition = (bool)EvaluateExpression(whileStatement.Condition);
        }
    }

    private void EvaluateForStatement(BoundForStatement forStatement)
    {
        EvaluateStatement(forStatement.DeclarationStatement);
        var condition = (bool)EvaluateExpression(forStatement.Expression);

        while (condition)
        {
            EvaluateStatement(forStatement.Statement);

            EvaluateStatement(forStatement.AssignmentStatement);
            condition = (bool)EvaluateExpression(forStatement.Expression);
        }
    }

    private void EvaluateAssignmentExpressionStatement(BoundAssignmentExpressionStatement assignWrap)
    {
        var value = EvaluateExpression(assignWrap.BoundExpression);
        Variables[assignWrap.BoundIdentifier.Reference] = value;
        lastValue = value;
    }

    private void EvaluateAssignmentStatement(BoundAssignmentStatement assign)
    {
        var newVariable = assign.BoundIdentifier;
        var value = EvaluateExpression(assign.BoundExpression);
        Variables[newVariable] = value;
        lastValue = value;
    }

    // expressions
    private object EvaluateExpression(BoundNode node)
    {
        if (node is BoundLiteralExpression literal)
            return EvaluateLiteralExpression(literal);

        if (node is BoundVariableExpression name)
            return EvaluateVariableExpression(name);

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
        Variables[assignment.BoundIdentifier.Reference] = rightExpression;

        return rightExpression;
    }

    private object EvaluateVariableExpression(BoundVariableExpression name)
    {
        return Variables[name.Reference];
    }

    private object EvaluateLiteralExpression(BoundLiteralExpression literal)
    {
        return literal.Value;
    }
}