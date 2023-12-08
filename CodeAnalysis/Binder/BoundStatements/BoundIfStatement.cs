using CodeAnalysis.Binder;
using CodeAnalysis.Binder.BoundStatements;

class BoundIfStatement : BoundStatement
{
    public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    

}