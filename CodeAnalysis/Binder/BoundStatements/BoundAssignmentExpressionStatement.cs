using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;

public class BoundAssignmentExpressionStatement : BoundStatement
{
    public BoundAssignmentExpressionStatement(BoundNameExpression boundIdentifier, BoundExpression boundExpression)
    {
        BoundIdentifier = boundIdentifier;
        BoundExpression = boundExpression;
    }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpressionStatement;

    public BoundNameExpression BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }
}
