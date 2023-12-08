using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundStatements;

public class BoundElseStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.ElseStatement;
    public BoundStatement Statement { get; set; }

    public BoundElseStatement(BoundStatement statement)
    {
        Statement = statement;
    }
}