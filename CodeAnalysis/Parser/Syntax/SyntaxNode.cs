#region

using CodeAnalysis.Scanner.Syntax;

#endregion

namespace CodeAnalysis.Parser.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public abstract int StartPosition { get; }
    public abstract int EndPosition { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}