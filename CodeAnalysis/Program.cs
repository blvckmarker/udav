using CodeAnalysis;
using CodeAnalysis.Compilation;
using CodeAnalysis.Diagnostic;

var showVariables = false;
var showTree = false;
var sessionVariables = new Dictionary<VariableSymbol, object>();
var compiler = new Compiler(sessionVariables);

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
        showVariables = !showVariables;
        continue;
    }

    var environment = new EnvironmentVariables(showTree, showVariables);
    var compilationResult = compiler.Compile(line, environment);

    if (compilationResult.Kind is not CompilationResultKind.Success)
    {
        Console.WriteLine(compilationResult.Kind);
        PrintDiagnostics(compilationResult.Diagnostics);
        continue;
    }

    PrintDiagnostics(compilationResult.Diagnostics);
    Console.WriteLine(compilationResult.ReturnResult);
}

void PrintDiagnostics(IEnumerable<DiagnosticsBag> diagnostics)
{
    foreach (var diagnostic in diagnostics)
    {
        var foregroundColor = diagnostic.Kind switch
        {
            IssueKind.Problem => ConsoleColor.Red,
            IssueKind.Warning => ConsoleColor.Yellow,
            _ => ConsoleColor.White,
        };

        if (diagnostic.ProblemText is null)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine($"{diagnostic.Kind}. {diagnostic.Message}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = foregroundColor;
            Console.Write($"{diagnostic.Kind}. At:{diagnostic.Span.Start} {diagnostic.Message} ");
            Console.ResetColor();

            Console.WriteLine(diagnostic.ProblemText);
        }
    }
}