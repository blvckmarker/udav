namespace CodeAnalysis.Compilation
{
    public enum CompilationResultKind
    {
        Success,
        SyntaxError,
        SemanticError,
        RuntimeError
    }
}