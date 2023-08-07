#region

using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Text;

#endregion

namespace CodeAnalysis.Syntax;

public class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        : this(kind, position, position + text.Length, text, value) { }

    public SyntaxToken(SyntaxKind kind, int startPosition, int endPosition, string text, object? value)
    {
        Kind = kind;
        Text = text;
        Value = value;
        Span = TextSpan.FromBounds(startPosition, endPosition);
    }

    public SyntaxToken(SyntaxKind kind, TextSpan span, string text, object? value)
    {
        Kind = kind;
        Span = span;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }

    public override TextSpan Span { get; }

    public string Text { get; set; }
    public object? Value { get; set; }
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

}