namespace CodeAnalysis.Binder.BoundExpressions;

internal abstract class BoundExpression : BoundNode
{
    public abstract Type Type { get; }
}