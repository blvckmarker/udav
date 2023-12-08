namespace CodeAnalysis.Binder.BoundStatements;

public class BoundBlockStatement : BoundStatement
{
    public BoundBlockStatement(IEnumerable<BoundStatement> statements)
    {
        Statements = statements;
    }

    public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
    public IEnumerable<BoundStatement> Statements { get; }
}