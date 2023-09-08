using CodeAnalysis;
using CodeAnalysis.Binder.Core;
using CodeAnalysis.Binder.Scopes;
using CodeAnalysis.Compilation;
using CodeAnalysis.Diagnostic;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Parser;
using CodeAnalysis.Syntax.Scanner;
using CodeAnalysis.Text;

public sealed class Compiler
{
    private BoundGlobalScope _previous;

    public IDictionary<VariableSymbol, object> Variables { get; private set; }


    public Compiler(IDictionary<VariableSymbol, object> variables)
    {
        Variables = variables;
    }
    public CompilationResult Compile(string source, EnvironmentVariables environmentVariables)
    {
        var sourceProgram = SourceText.From(source);

        var lexer = new Lexer(sourceProgram);
        var tokens = lexer.LexAll();
        if (hasProblem(lexer.Diagnostics))
            return new CompilationResult(CompilationResultKind.SyntaxError, null, lexer.Diagnostics);

        var parser = new Parser(tokens, lexer.Diagnostics);
        var syntaxTree = parser.ParseTree();
        if (environmentVariables.ShowTree)
            showTree(syntaxTree.Root);
        if (hasProblem(syntaxTree.Diagnostics))
            return new CompilationResult(CompilationResultKind.SyntaxError, null, syntaxTree.Diagnostics);

        var globalScope = Binder.BindGlobalScope(_previous, syntaxTree);
        var boundRoot = globalScope.BoundRoot;
        if (hasProblem(globalScope.Diagnostics))
            return new CompilationResult(CompilationResultKind.SemanticError, null, globalScope.Diagnostics);

        var evaluator = new Evaluator(boundRoot, Variables);
        object? result;
        try
        {
            result = evaluator.Evaluate();
        }
        catch (Exception exception)
        {
            var diagnostics = new Diagnostics();
            diagnostics.MakeIssue(exception.Message);

            return new CompilationResult(CompilationResultKind.RuntimeError, null, diagnostics);
        }

        Variables = evaluator.Variables;

        if (environmentVariables.ShowVariables)
            showVariables(Variables);

        var warningDiagnostics = lexer.Diagnostics
                                      .Extend(syntaxTree.Diagnostics)
                                      .Extend(globalScope.Diagnostics);
        _previous = globalScope;
        return new CompilationResult(CompilationResultKind.Success, result, warningDiagnostics);
    }
    private void showVariables(IDictionary<VariableSymbol, object> variables)
    {
        foreach (var variable in variables)
            Console.WriteLine($"{variable.Key.Name}:{variable.Key.Type}:{variable.Value}");
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
