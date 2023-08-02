namespace CodeAnalysis.Binder.BoundExpressions;
public class BoundNameExpression : BoundExpression
{
    public BoundNameExpression(VariableSymbol variableReference)
    {
        Reference = variableReference;
    }

    public VariableSymbol Reference { get; }
    public override Type Type => Reference.Type;
    public override BoundNodeKind Kind => BoundNodeKind.NameExpression;
}
