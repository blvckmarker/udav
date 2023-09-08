using System.Collections.Immutable;

namespace CodeAnalysis.Binder.Scopes;

public abstract class BoundScope
{
    protected IDictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

    public abstract BoundScope Previous { get; }
    public IDictionary<string, VariableSymbol> Variables => _variables;


    public bool TryDeclareVariable(VariableSymbol variableSymbol)
    {
        if (_variables.ContainsKey(variableSymbol.Name))
            return false;

        _variables.Add(variableSymbol.Name, variableSymbol);
        return true;
    }


    public bool TryGetValueOf(string identifier, out VariableSymbol variable)
    {
        if (_variables.TryGetValue(identifier, out variable))
            return true;

        if (Previous == null)
            return false;

        return Previous.TryGetValueOf(identifier, out variable);
    }
}