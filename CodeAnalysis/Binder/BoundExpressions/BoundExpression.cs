namespace CodeAnalysis.Binder.BoundExpressions;

public abstract class BoundExpression : BoundNode
{
    public abstract Type Type { get; }
}