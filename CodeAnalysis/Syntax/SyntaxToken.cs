#region

using CodeAnalysis.Syntax.Parser;

#endregion

namespace CodeAnalysis.Syntax;

public class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        : this(kind, position, position + text.Length, text, value) { }

    public SyntaxToken(SyntaxKind kind, int startPosition, int endPosition, string text, object? value)
    {
        Kind = kind;
        StartPosition = startPosition;
        EndPosition = endPosition;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }

    public override int StartPosition { get; }
    public override int EndPosition { get; }

    public string Text { get; set; }
    public object? Value { get; set; }
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
}