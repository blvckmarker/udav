namespace CodeAnalysis.Binder.Scopes;

public sealed class BoundStatementScope : BoundScope
{
    public override BoundScope Parent { get; }

    public BoundStatementScope(BoundScope parent)
    {
        Parent = parent;
    }
}