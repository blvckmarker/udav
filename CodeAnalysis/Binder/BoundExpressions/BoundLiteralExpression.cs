namespace CodeAnalysis.Binder.BoundExpressions;

public sealed class BoundLiteralExpression : BoundExpression
{
    public BoundLiteralExpression(object value)
    {
        Value = value;
    }

    public object Value { get; }
    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    public override Type Type => Value.GetType();

}