using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;

public sealed class BoundAssignmentExpressionStatement : BoundStatement
{
    public BoundAssignmentExpressionStatement(BoundVariableExpression boundIdentifier, BoundExpression boundExpression)
    {
        BoundIdentifier = boundIdentifier;
        BoundExpression = boundExpression;
    }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpressionStatement;

    public BoundVariableExpression BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }
}
