namespace CodeAnalysis.Binder.BoundExpressions;

public class BoundDeclaredVariableExpression : BoundExpression
{
    public BoundDeclaredVariableExpression(VariableSymbol variable)
    {
        Variable = variable;
    }

    public VariableSymbol Variable { get; }
    public override Type Type => Variable.Type;
    public override BoundNodeKind Kind => BoundNodeKind.DeclaredVariableExpression;
}
