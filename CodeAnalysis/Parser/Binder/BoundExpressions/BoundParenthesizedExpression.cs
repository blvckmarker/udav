namespace CodeAnalysis.Parser.Binder;

internal sealed class BoundParenthesizedExpression : BoundExpression
{
    public BoundExpression BoundExpression { get; }
    public override BoundNodeKind Kind => BoundNodeKind.ParenthesizedExpression;
    public override Type Type => BoundExpression.Type;

    public BoundParenthesizedExpression(BoundExpression boundExpression)
    {
        BoundExpression = boundExpression;
    }
}