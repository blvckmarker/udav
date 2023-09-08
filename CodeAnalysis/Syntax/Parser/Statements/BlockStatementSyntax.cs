
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

public class BlockStatementSyntax : StatementSyntax
{
    public BlockStatementSyntax(SyntaxToken openBrace, IEnumerable<StatementSyntax> statements, SyntaxToken closeBrace)
    {
        OpenBrace = openBrace;
        Statements = statements;
        CloseBrace = closeBrace;
    }

    public override SyntaxKind Kind => SyntaxKind.BlockStatement;
    public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span.Start, CloseBrace.Span.End);

    public SyntaxToken OpenBrace { get; }
    public IEnumerable<StatementSyntax> Statements { get; }
    public SyntaxToken CloseBrace { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        var children = new List<SyntaxNode> { OpenBrace };

        foreach (var statement in Statements)
            children.Add(statement);

        children.Add(CloseBrace);

        return children;
    }
}
