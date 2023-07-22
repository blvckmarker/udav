#region

using CodeAnalysis.Scanner.Model;

#endregion

namespace CodeAnalysis.Parser.Expressions.AST;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}