using CodeAnalysis.Diagnostic;

namespace CodeAnalysis.Binder.Scopes;

public sealed class BoundGlobalScope : BoundScope
{
    public override BoundScope Previous { get; }
    public BoundNode BoundRoot { get; }
    public DiagnosticsBase Diagnostics { get; }

    public BoundGlobalScope(BoundScope previous, BoundNode root, DiagnosticsBase diagnostics, IDictionary<string, VariableSymbol> variables)
    {
        Previous = previous;
        BoundRoot = root;
        Diagnostics = diagnostics;
        _variables = variables;
    }
}
