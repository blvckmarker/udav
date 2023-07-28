namespace CodeAnalysis.Binder.BoundExpressions;

public sealed class BoundParenthesizedExpression : BoundExpression
{
    public BoundExpression BoundExpression { get; }
    public override BoundNodeKind Kind => BoundNodeKind.ParenthesizedExpression;
    public override Type Type => BoundExpression.Type;

    public BoundParenthesizedExpression(BoundExpression boundExpression)
    {
        BoundExpression = boundExpression;
    }
}