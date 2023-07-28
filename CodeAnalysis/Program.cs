#region

using CodeAnalysis;
using CodeAnalysis.Binder;
using CodeAnalysis.Parser.Expressions.AST;
using CodeAnalysis.Scanner.Model;
using CodeAnalysis.Scanner.Shared;
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
    var expressionTree = SyntaxTree.Parse(lexer);

    var binder = new Binder();
    var boundExpression = binder.BindExpression(expressionTree.Root);

    var diagnostics = expressionTree.Diagnostics.Concat(binder.Diagnostics);
    if (showTree)
        PrettySyntaxPrint(expressionTree.Root);

    if (diagnostics.Any())
        foreach (var diagnostic in diagnostics)
            Console.WriteLine(diagnostic);
    else
    {
        var e = new Evaluator(boundExpression);
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