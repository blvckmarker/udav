#region

using CodeAnalysis.Parser.Expressions.AST;

#endregion

namespace CodeAnalysis.Scanner.Model;

public class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string? text, object? value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }

    public int Position { get; set; }
    public string? Text { get; set; }
    public object? Value { get; set; }
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
}