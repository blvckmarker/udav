namespace CodeAnalysis.Binder.BoundExpressions;
public sealed class BoundVariableExpression : BoundExpression
{
    public BoundVariableExpression(VariableSymbol variableReference)
    {
        Reference = variableReference;
    }

    public VariableSymbol Reference { get; }
    public override Type Type => Reference.Type;
    public override BoundNodeKind Kind => BoundNodeKind.NameExpression;
}
