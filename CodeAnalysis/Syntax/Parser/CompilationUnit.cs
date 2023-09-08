using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser;

public sealed class CompilationUnit : SyntaxNode
{
    public CompilationUnit(StatementSyntax statement)
    {
        Statement = statement;
    }

    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    public override TextSpan Span => Statement.Span;
    public StatementSyntax Statement { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Statement;
    }
}
