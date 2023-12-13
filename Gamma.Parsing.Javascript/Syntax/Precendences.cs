namespace Gamma.Parsing.Javascript.Syntax;

public class Precendences 
{
    private static Dictionary<string, int> _precendences = new ()
    {
        { "=", 1 }, { "+=", 1 },
        { "||", 2 },
        { "&&", 3 },
        { "<", 7 }, { ">", 7 }, { "<=", 7 }, { ">=", 7 }, { "==", 7 }, { "===", 7 }, { "!=", 7 }, { "!==", 7 },
        { "+", 10 }, { "-", 10 },
        { "*", 20 }, { "/", 20 }, { "%", 20 } 
    };

    public int this[string @operator] => _precendences[@operator];
}

