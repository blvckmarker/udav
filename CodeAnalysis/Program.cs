using CodeAnalysis;
using CodeAnalysis.Compilation;
using CodeAnalysis.Diagnostic;
using System.Text;

var showVariables = false;
var showTree = false;
var sessionVariables = new Dictionary<VariableSymbol, object>();
var compiler = new Compiler(sessionVariables);

var textBuilder = new StringBuilder();
//> let i = 3
//| let a = 3

//>
//
//

while (true)
{
    if (textBuilder.Length == 0)
        Console.Write("> ");
    else
        Console.Write("| ");

    var line = Console.ReadLine();
    var isBlank = string.IsNullOrWhiteSpace(line);

    if (textBuilder.Length == 0)
    {
        if (isBlank)
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

    }

    textBuilder = textBuilder.AppendLine(line);
    var program = textBuilder.ToString();

    var environment = new EnvironmentVariables(showTree, showVariables);
    var compilationResult = compiler.Compile(program, environment);

    if (compilationResult.Kind is CompilationResultKind.SyntaxError)
        continue;

    PrintDiagnostics(compilationResult.Diagnostics);
    Console.WriteLine(compilationResult.ReturnResult);

    textBuilder.Clear();
}

void PrintDiagnostics(DiagnosticsBase diagnostics)
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
            var lineIndex = diagnostics.SourceProgram.GetLineIndex(diagnostic.Span.Start);

            Console.ForegroundColor = foregroundColor;
            Console.Write($"{diagnostic.Kind}. At {lineIndex}:{diagnostic.Span.Start} {diagnostic.Message} ");
            Console.ResetColor();

            Console.WriteLine(diagnostic.ProblemText);
        }
    }
}