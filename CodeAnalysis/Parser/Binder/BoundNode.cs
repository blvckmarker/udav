namespace CodeAnalysis.Parser.Binder;

internal abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
}