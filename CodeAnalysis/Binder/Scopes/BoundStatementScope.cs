namespace CodeAnalysis.Binder.Scopes;

public sealed class BoundStatementScope : BoundScope
{
    public override BoundScope Previous { get; }

    public BoundStatementScope(BoundScope parent)
    {
        Previous = parent;
    }
}