namespace Gamma.Parsing.Javascript.Syntax;

public class Precendences 
{
    private static Dictionary<string, int> _precendences = new ()
    {
        { ",", 1 },
        { "=", 2 }, { "+=", 2 }, { "-=", 2 }, { "**=", 2 },
        { "*=", 2 }, { "/=", 2 }, { "%=", 2 }, { "<<=", 2 },
        { ">>=", 2 }, { ">>>=", 2 }, { "&=", 2 }, { "^=", 2 },
        { "|=", 2 }, { "&&=", 2 }, { "||=", 2 }, { "??=", 2 },
        { "...", 2 }, { "=>", 2 },
        { "||", 3 }, { "??", 3 },  // logical OR, nullish coalescing
        { "&&", 4 }, // logical AND
        { "|", 5 }, // bitwise OR
        { "^", 6 }, // bitwise XOR
        { "&", 7 }, // bitwise AND
        { "==", 8 }, { "!=", 8}, { "===", 8 }, { "!==", 8 }, // equality
        { "<", 9 }, { "<=", 9 }, { ">", 9 }, { ">=", 9 }, // relational
        { "<<", 10 }, { ">>", 10 }, { ">>>", 10 }, // bitshift
        { "+", 11 }, { "-", 11 }, // additive
        { "*", 12 }, { "/", 12 }, { "%", 12 }, // multiplicative
        { "**", 13 }, // exponentiation
    };

    public int this[string @operator] => _precendences[@operator];
}

