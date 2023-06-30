namespace CodeAnalysis.Parser.Binder;

internal sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryExpression(BoundUnaryOperatorKind? operatorToken, BoundExpression operand)
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public BoundUnaryOperatorKind? OperatorToken { get; }
    public BoundExpression Operand { get; }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operand.Type;
}