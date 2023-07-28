namespace CodeAnalysis.Binder.BoundExpressions;

public sealed class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator operatorToken, BoundExpression right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public BoundExpression Left { get; }
    public BoundBinaryOperator OperatorToken { get; }
    public BoundExpression Right { get; }

    public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    public override Type Type => Left.Type;
}