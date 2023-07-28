namespace CodeAnalysis.Binder.BoundExpressions;

internal sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryExpression(BoundUnaryOperator? operatorToken, BoundExpression operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public BoundUnaryOperator? OperatorToken { get; }
    public BoundExpression Operand { get; }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operand.Type;
}