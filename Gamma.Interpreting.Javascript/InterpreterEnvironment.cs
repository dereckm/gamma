namespace Gamma.Interpreting.Javascript;

internal class InterpreterEnvironment
{
    private Dictionary<string, Variable> _variables;
    private InterpreterEnvironment? _parent;

    public InterpreterEnvironment(InterpreterEnvironment? parent = null)
    {

        _variables = new();
        if (parent != null) 
        {
            _variables = new Dictionary<string, Variable>(parent._variables);
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
            return _variables[name].Value;
        throw new Exception($"Undefined variable {name}");
    }

    public void Set(string name, object value)
    {
        var scope = Lookup(name);
        if (scope == null && _parent != null)
            throw new Exception($"Undefined variable {name}");

        
        var validScope = scope ?? this;
        var variable = validScope._variables[name];
        if (variable.Type == "const")
            throw new Exception($"Illegal assingment on const variable: \"{name}\"");
        validScope._variables[name] = new Variable { Value = value, Type = variable.Type };
    }

    public void Def(string name, object value, string type)
    {
        _variables[name] = new Variable { Value = value, Type = type };
    }

    private class Variable 
    {
        public object Value { get; set; } = new object();
        public string Type { get; set; } = "let";
    }
}
