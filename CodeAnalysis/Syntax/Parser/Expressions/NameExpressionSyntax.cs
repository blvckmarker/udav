namespace CodeAnalysis.Syntax.Parser.Expressions;
public class NameExpressionSyntax : ExpressionSyntax
{
    public NameExpressionSyntax(SyntaxToken literalToken)
    {
        Identifier = literalToken;
    }

    public SyntaxToken Identifier { get; }
    public override SyntaxKind Kind => SyntaxKind.NameExpression;
    public override int StartPosition => Identifier.StartPosition;
    public override int EndPosition => Identifier.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
    }
}
