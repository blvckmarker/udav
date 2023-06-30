using System.Collections.Immutable;
using CodeAnalysis.Lexer.Model;

namespace CodeAnalysis.Parser.Expressions.AST;

public sealed class SyntaxTree
{
    public ImmutableArray<string> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EofToken { get; }

    public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken eofToken)
    {
        Diagnostics = diagnostics.ToImmutableArray();
        Root = root;
        EofToken = eofToken;
    }

    public static SyntaxTree Parse(string text) => new Parser(text).Parse();
}