using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public abstract TextSpan Span { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}