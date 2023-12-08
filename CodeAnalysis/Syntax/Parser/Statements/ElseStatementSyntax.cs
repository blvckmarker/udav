using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

public class ElseStatementSyntax : StatementSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ElseStatement;
    public SyntaxToken ElseToken { get; set; }
    public StatementSyntax Statement { get; set; }
    public override TextSpan Span => TextSpan.FromBounds(ElseToken.Span.Start, Statement.Span.End);

    public ElseStatementSyntax(SyntaxToken elseToken, StatementSyntax statement)
    {
        ElseToken = elseToken;
        Statement = statement;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return ElseToken;
        yield return Statement;
    }
}