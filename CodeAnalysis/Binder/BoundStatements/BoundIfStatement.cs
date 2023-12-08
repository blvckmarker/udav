using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundExpressions;
using CodeAnalysis.Binder.BoundStatements;

public class BoundIfStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    public BoundExpression Expression { get; set; }
    public BoundStatement Statement { get; set; }
    public BoundElseStatement? ElseStatement { get; set; }

    public BoundIfStatement(BoundExpression expression, BoundStatement statement, BoundElseStatement? elseStatement)
    {
        Expression = expression;
        Statement = statement;
        ElseStatement = elseStatement;
    }
}
