using CodeAnalysis.Diagnostic;

namespace CodeAnalysis.Binder.Scopes;

public sealed class BoundGlobalScope : BoundScope
{
    public override BoundScope Parent { get; }
    public BoundNode BoundRoot { get; }
    public DiagnosticsBase Diagnostics { get; }

    public BoundGlobalScope(BoundScope parent, BoundNode root, DiagnosticsBase diagnostics, IDictionary<string, VariableSymbol> variables)
    {
        Parent = parent;
        BoundRoot = root;
        Diagnostics = diagnostics;
        _variables = variables;
    }
}
