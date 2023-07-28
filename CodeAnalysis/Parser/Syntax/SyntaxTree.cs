using CodeAnalysis.Parser.Expressions;
using CodeAnalysis.Scanner.Shared;
using CodeAnalysis.Scanner.Syntax;
using System.Collections.Immutable;

namespace CodeAnalysis.Parser.Syntax;

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
    public static SyntaxTree Parse(Lexer lexer) => new Parser(lexer).Parse();
}