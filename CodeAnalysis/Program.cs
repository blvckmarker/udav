using CodeAnalysis;
using CodeAnalysis.Compilation;
using CodeAnalysis.Diagnostic;
using CodeAnalysis.Text;
using System.Text;

var showVariables = false;
var showTree = false;
var sessionVariables = new Dictionary<VariableSymbol, object>();
var compiler = new Compiler(sessionVariables);

var textBuilder = new StringBuilder();

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

    if (compilationResult.Kind is CompilationResultKind.SyntaxError && !isBlank)
        continue;

    PrintDiagnostics(program, compilationResult.Diagnostics);
    Console.WriteLine(compilationResult.ReturnResult);

    textBuilder.Clear();
}

static void PrintDiagnostics(string source, DiagnosticsBase diagnostics)
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
            var lineIndex = SourceText.From(source).GetLineIndex(diagnostic.Span.Start);

            // for (int i = 0; i < diagnostics.SourceProgram.Lines.Length; ++i)
            // {
            //     var line = diagnostics.SourceProgram.Lines[i];
            //     Console.WriteLine($"{i} {line.Span} {line.Text}");
            // }


            Console.ForegroundColor = foregroundColor;
            Console.Write($"{diagnostic.Kind}. At {lineIndex}:{diagnostic.Span.Start} {diagnostic.Message} ");
            Console.ResetColor();

            Console.WriteLine(diagnostic.ProblemText);
        }
    }
}