namespace CodeAnalysis.Syntax.Parser.Expressions;
public class NameExpressionSyntax : ExpressionSyntax
{
    public NameExpressionSyntax(SyntaxToken literalToken)
    {
        Name = literalToken;
    }

    public SyntaxToken Name { get; }
    public override SyntaxKind Kind => SyntaxKind.NameExpression;
    public override int StartPosition => Name.StartPosition;
    public override int EndPosition => Name.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Name;
    }
}
