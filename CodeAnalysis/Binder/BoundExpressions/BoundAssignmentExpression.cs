namespace CodeAnalysis.Binder.BoundExpressions;

public class BoundAssignmentExpression : BoundExpression
{
    public BoundAssignmentExpression(BoundVariableExpression boundIdentifier, BoundExpression boundExpression)
    {
        BoundIdentifier = boundIdentifier;
        BoundExpression = boundExpression;
    }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

    public BoundVariableExpression BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }
}
