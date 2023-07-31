namespace CodeAnalysis.Binder.BoundStatements
{
    public abstract class BoundStatement : BoundNode
    {
        public abstract Type Type { get; }
    }
}