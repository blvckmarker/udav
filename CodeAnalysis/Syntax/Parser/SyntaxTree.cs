using CodeAnalysis.Syntax.Parser.Expressions;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser;

public sealed class SyntaxTree
{
    public DiagnosticsBase Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EofToken { get; }

    public SyntaxTree(DiagnosticsBase diagnostics, ExpressionSyntax root, SyntaxToken eofToken)
    {
        Diagnostics = diagnostics;
        Root = root;
        EofToken = eofToken;
    }

    public static SyntaxTree Parse(string text) => new Parser(text).Parse();
    public static SyntaxTree Parse(Lexer lexer) => new Parser(lexer).Parse();
}