using CodeAnalysis.Binder.BoundStatements;

namespace CodeAnalysis.Binder;

public class BoundCompilationUnit : BoundNode
{
    public BoundCompilationUnit(BoundStatement statement)
    {
        Statement = statement;
    }

    public override BoundNodeKind Kind => BoundNodeKind.CompilationUnit;
    public BoundStatement Statement { get; }
}