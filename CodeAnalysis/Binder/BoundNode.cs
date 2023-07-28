namespace CodeAnalysis.Binder;

internal abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
}