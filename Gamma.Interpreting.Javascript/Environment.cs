namespace Gamma.Interpreting.Javascript;

internal class InterpreterEnvironment
{
    private Dictionary<string, object> _variables;
    private InterpreterEnvironment? _parent;

    public InterpreterEnvironment(InterpreterEnvironment? parent = null)
    {

        _variables = new();
        if (parent != null) 
        {
            _variables = new Dictionary<string, object>(parent._variables);
        }
        _parent = parent;
    }

    public InterpreterEnvironment Extend()
    {
        return new InterpreterEnvironment(this);
    }

    public InterpreterEnvironment? Lookup(string name)
    {
        var scope = this;
        while (scope != null)
        {
            if (scope._variables.ContainsKey(name))
                return scope;
            scope = scope._parent;
        }
        return null;
    }

    public object Get(string name)
    {
        if (_variables.ContainsKey(name))
            return _variables[name];
        throw new Exception($"Undefined variable {name}");
    }

    public void Set(string name, object value)
    {
        var scope = Lookup(name);
        if (scope == null && _parent != null)
            throw new Exception($"Undefined variable {name}");
        (scope ?? this)._variables[name] = value;
    }

    public void Def(string name, object value)
    {
        _variables[name] = value;
    }
}
