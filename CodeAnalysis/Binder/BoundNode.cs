namespace CodeAnalysis.Binder;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
}