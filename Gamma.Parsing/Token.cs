using System.Diagnostics;

namespace Gamma.Parsing;

[DebuggerDisplay("{Value}, Type={Type}")]
public class Token
{
    public string Value { get; }
    public TokenType Type { get; }

    public Token(string value, TokenType type) 
    {
        Value = value;
        Type = type;
    }

    public Token(char value, TokenType type) : this(value.ToString(), type) {}
  
}

public enum TokenType 
{
    Number,
    Identifier,
    String,
    Punctuation,
    Operator,
    Keyword
}
