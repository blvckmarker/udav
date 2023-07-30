using CodeAnalysis.Syntax.Parser.Statements;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Parser;

public sealed class SyntaxTree
{
    public DiagnosticsBase Diagnostics { get; }
    public StatementSyntax Root { get; }
    public SyntaxToken EofToken { get; }

    public SyntaxTree(DiagnosticsBase diagnostics, StatementSyntax root, SyntaxToken eofToken)
    {
        Diagnostics = diagnostics;
        Root = root;
        EofToken = eofToken;
    }

    public static SyntaxTree Parse(string text) => new Parser(text).ParseTree();
    public static SyntaxTree Parse(Lexer lexer) => new Parser(lexer).ParseTree();
}