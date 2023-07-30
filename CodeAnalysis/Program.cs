#region

using CodeAnalysis;
using CodeAnalysis.Binder;
using CodeAnalysis.Parser.Syntax;
using CodeAnalysis.Scanner.Shared;
using CodeAnalysis.Scanner.Syntax;
#endregion

var showTree = false;
while (true)
{
    Console.Write(">");
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line))
        break;
    if (line == "#showtree")
    {
        showTree = !showTree;
        continue;
    }
    var lexer = new Lexer(line);
    var syntaxTree = SyntaxTree.Parse(lexer);

    var binder = new Binder(syntaxTree.Diagnostics);
    var boundTree = binder.BindExpression(syntaxTree.Root);
    var diagnostics = binder.Diagnostics;

    if (showTree)
        PrettySyntaxPrint(syntaxTree.Root);

    if (diagnostics.Where(x => x.Kind == CodeAnalysis.Text.IssueKind.Problem).Any())
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.ProblemText is { } text)
            {
                Console.Write($"At:{diagnostic.StartPosition} {diagnostic.Message}: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            else
                Console.WriteLine(diagnostic.Message);
        }
    else
    {
        foreach (var diagnostic in diagnostics.Where(x => x.Kind == CodeAnalysis.Text.IssueKind.Warning))
        {
            if (diagnostic.ProblemText is { } text)
            {
                Console.Write($"At:{diagnostic.StartPosition} {diagnostic.Message}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            else
                Console.WriteLine(diagnostic.Message);
        }

        var e = new Evaluator(boundTree);
        Console.WriteLine(e.Evaluate());
    }
}

static void PrettySyntaxPrint(SyntaxNode node, string shift = "")
{
    Console.Write(shift);
    Console.Write(node.Kind);

    if (node is SyntaxToken { Value: { } } t)
        Console.Write(" " + t.Value);

    Console.WriteLine();
    shift += "    ";

    foreach (var child in node.GetChildren())
        PrettySyntaxPrint(child, shift);
}