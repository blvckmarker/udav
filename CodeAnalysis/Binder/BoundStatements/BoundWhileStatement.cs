using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;

public class BoundWhileStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

    public BoundExpression Condition { get; }
    public BoundStatement Statement { get; }

    public BoundWhileStatement(BoundExpression condition, BoundStatement statement)
    {
        Condition = condition;
        Statement = statement;
    }
}