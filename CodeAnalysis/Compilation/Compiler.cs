using CodeAnalysis;
using CodeAnalysis.Binder;
using CodeAnalysis.Compilation;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

public sealed class Compiler
{
    public IDictionary<string, object> SessionVariables { get; private set; }

    public Compiler(IDictionary<string, object> sessionVariables)
    {
        SessionVariables = sessionVariables;
    }
    public CompilationResult Compile(string source, EnvironmentVariables environmentVariables)
    {
        var lexer = new Lexer(source);
        if (hasProblem(lexer.Diagnostics))
            return new CompilationResult(CompilationResultKind.SyntaxError, null, lexer.Diagnostics);

        var syntaxTree = SyntaxTree.Parse(lexer);
        if (environmentVariables.ShowTree)
            showTree(syntaxTree.Root);
        if (hasProblem(syntaxTree.Diagnostics))
            return new CompilationResult(CompilationResultKind.SyntaxError, null, syntaxTree.Diagnostics);

        var binder = new Binder(syntaxTree, SessionVariables);
        var boundTree = binder.BindTree();
        if (hasProblem(binder.Diagnostics))
            return new CompilationResult(CompilationResultKind.SemanticError, null, binder.Diagnostics);

        var evaluator = new Evaluator(boundTree, SessionVariables);
        object? result;
        try
        {
            result = evaluator.Evaluate();
        }
        catch (Exception exception)
        {
            var diagnostics = new Diagnostics(source);
            diagnostics.MakeIssue(exception.Message);

            return new CompilationResult(CompilationResultKind.RuntimeError, null, diagnostics);
        }

        SessionVariables = evaluator.LocalVariables;

        if (environmentVariables.ShowVariables)
            showVariables(evaluator.LocalVariables);

        var warningDiagnostics = lexer.Diagnostics
                                      .Extend(syntaxTree.Diagnostics)
                                      .Extend(binder.Diagnostics);

        return new CompilationResult(CompilationResultKind.Success, result, warningDiagnostics);
    }
    private void showVariables(IDictionary<string, object> variables)
    {
        foreach (var variable in variables)
            Console.WriteLine($"{variable.Key}:{variable.Value}");
    }
    private void showTree(SyntaxNode node, string shift = "")
    {
        Console.Write(shift);
        Console.Write(node.Kind);

        if (node is SyntaxToken { Value: { } } t)
            Console.Write(" " + t.Value);

        Console.WriteLine();
        shift += "    ";

        foreach (var child in node.GetChildren())
            showTree(child, shift);
    }
    private static bool hasProblem(DiagnosticsBase diagnostics)
    => diagnostics.Where(x => x.Kind == IssueKind.Problem).Any();
}

public record struct EnvironmentVariables(bool ShowTree = false, bool ShowVariables = false);
