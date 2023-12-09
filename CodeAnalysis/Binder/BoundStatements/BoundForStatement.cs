using CodeAnalysis.Binder.BoundExpressions;

namespace CodeAnalysis.Binder.BoundStatements;

public class BoundForStatement : BoundStatement
{
    public BoundForStatement(BoundStatement boundFirstStatement, BoundExpression boundExpression, BoundStatement boundSecondStatement, BoundStatement boundStatement)
    {
        DeclarationStatement = boundFirstStatement;
        Expression = boundExpression;
        AssignmentStatement = boundSecondStatement;
        Statement = boundStatement;
    }

    public BoundStatement DeclarationStatement { get; }
    public BoundExpression Expression { get; }
    public BoundStatement AssignmentStatement { get; }
    public BoundStatement Statement { get; }

    public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
}