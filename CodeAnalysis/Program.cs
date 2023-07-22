#region

using CodeAnalysis;
using CodeAnalysis.Lexer;
using CodeAnalysis.Lexer.Model;
using CodeAnalysis.Parser.Binder;
using CodeAnalysis.Parser.Expressions.AST;

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
    var lexer = Lexer.LexTokens(line);

    foreach (var token in lexer)
        Console.WriteLine(token.Text + " " + token.Kind);

    var expressionTree = SyntaxTree.Parse(line);
    var binder = new Binder();
    var boundExpression = binder.BindExpression(expressionTree.Root);

    var diagnostics = expressionTree.Diagnostics.Concat(binder.Diagnostics);
    if (showTree)
        PrettyPrint(expressionTree.Root);

    if (diagnostics.Any())
        foreach (var diagnostic in expressionTree.Diagnostics)
            Console.WriteLine(diagnostic);
    else
    {
        var e = new Evaluator(boundExpression);
        Console.WriteLine(e.Evaluate());
    }
}

static void PrettyPrint(SyntaxNode node, string shift = "")
{
    Console.Write(shift);
    Console.Write(node);

    if (node is SyntaxToken { Value: { } } t)
        Console.Write(" " + t.Value);

    Console.WriteLine();
    shift += "    ";

    foreach (var child in node.GetChildren())
        PrettyPrint(child, shift);
}