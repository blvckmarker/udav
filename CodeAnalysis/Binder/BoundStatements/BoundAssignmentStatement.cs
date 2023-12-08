using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;
public sealed class BoundAssignmentStatement : BoundStatement
{
    public BoundAssignmentStatement(VariableSymbol identifierName, BoundExpression boundExpression)
    {
        BoundIdentifier = identifierName;
        BoundExpression = boundExpression;
    }

    public VariableSymbol BoundIdentifier { get; }
    public BoundExpression BoundExpression { get; }

    public override BoundNodeKind Kind => BoundNodeKind.AssignmentStatement;
}


