using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binder.BoundStatements;
public sealed class BoundAssignmentStatement : BoundStatement
{
    public BoundAssignmentStatement(SyntaxToken identifierName, BoundExpression boundExpression)
    {
        IdentifierName = identifierName;
        BoundExpression = boundExpression;
    }

    public SyntaxToken IdentifierName { get; }
    public BoundExpression BoundExpression { get; }

    public override Type Type => BoundExpression.Type;
    public override BoundNodeKind Kind => BoundNodeKind.AssignmentStatement;
}


