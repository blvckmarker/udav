using CodeAnalysis.Diagnostic;

namespace CodeAnalysis.Compilation;
public class CompilationResult
{
    public CompilationResult(CompilationResultKind kind, object? returnResult, DiagnosticsBase diagnostics)
    {
        Kind = kind;
        ReturnResult = returnResult;
        Diagnostics = diagnostics;
    }

    public CompilationResultKind Kind { get; }
    public object? ReturnResult { get; }
    public DiagnosticsBase Diagnostics { get; }
}