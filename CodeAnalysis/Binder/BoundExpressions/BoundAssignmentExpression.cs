namespace CodeAnalysis.Binder.BoundExpressions;

public class BoundAssignmentExpression : BoundExpression
{
    public BoundAssignmentExpression(BoundNameExpression boundIdentifier, BoundExpression boundExpression)
    {
        BoundIdentifier = boundIdentifier;
        BoundExpression = boundExpression;
    }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

    public BoundNameExpression BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }
}
