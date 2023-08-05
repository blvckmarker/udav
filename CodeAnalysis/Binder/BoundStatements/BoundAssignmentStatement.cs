using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;
public sealed class BoundAssignmentStatement : BoundStatement
{
    public BoundAssignmentStatement(BoundIdentifierType boundIdentifierType, VariableSymbol identifierName, BoundExpression boundExpression)
    {
        BoundIdentifierType = boundIdentifierType;
        BoundIdentifier = identifierName;
        BoundExpression = boundExpression;
    }

    public BoundIdentifierType BoundIdentifierType { get; }
    public VariableSymbol BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentStatement;
}


