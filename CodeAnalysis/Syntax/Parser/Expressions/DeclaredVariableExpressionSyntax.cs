namespace CodeAnalysis.Syntax.Parser.Expressions;
public class DeclaredVariableExpressionSyntax : ExpressionSyntax
{
    public DeclaredVariableExpressionSyntax(SyntaxToken identifierToken, SyntaxKind identifierType)
    {
        Identifier = identifierToken;
        IdentifierType = identifierType;
    }

    public SyntaxKind IdentifierType { get; }
    public SyntaxToken Identifier { get; }

    public override SyntaxKind Kind => SyntaxKind.DeclaredVariableExpression;
    public override int StartPosition => Identifier.StartPosition;
    public override int EndPosition => Identifier.EndPosition;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
    }
}
