namespace CodeAnalysis.Parser.Binder;

internal sealed class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind? operatorToken, BoundExpression right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public BoundExpression Left { get; }
    public BoundBinaryOperatorKind? OperatorToken { get; }
    public BoundExpression Right { get; }

    public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    public override Type Type => Left.Type;
}