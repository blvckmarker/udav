using CodeAnalysis;
using CodeAnalysis.Binder;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

var showTree = false;
var localVariables = new Dictionary<string, object>();
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
    if (line == "#showvars")
    {
        foreach (var variable in localVariables)
            Console.WriteLine($"{variable.Key}:{variable.Value}");
        continue;
    }

    var lexer = new Lexer(line);
    var syntaxTree = SyntaxTree.Parse(lexer);
    if (HasIssue(syntaxTree.Diagnostics))
        continue;

    var binder = new Binder(syntaxTree, localVariables);
    var boundTree = binder.BindTree();

    if (HasIssue(binder.Diagnostics))
        continue;

    if (showTree)
        PrettySyntaxPrint(syntaxTree.Root);

    var e = new Evaluator(boundTree, localVariables);
    Console.WriteLine(e.Evaluate());
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

static bool HasIssue(DiagnosticsBase diagnostics)
{
    foreach (var diagnostic in diagnostics)
    {
        if (diagnostic.ProblemText is { } text)
        {
            Console.Write($"At:{diagnostic.StartPosition} {diagnostic.Message} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        else
            Console.WriteLine(diagnostic.Message);
    }
    return diagnostics.Count() != 0;
}