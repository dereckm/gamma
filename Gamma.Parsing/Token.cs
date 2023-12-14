using System.Diagnostics;

namespace Gamma.Parsing;

[DebuggerDisplay("{Value}, Type={Type}")]
public class Token
{
    public static Token OpenParenthesis => new("(", TokenType.Punctuation);
    public static Token CloseParenthesis => new(")", TokenType.Punctuation);
    public static Token Comma => new(",", TokenType.Punctuation);
    public static Token OpenBracket => new("[", TokenType.Punctuation);
    public static Token CloseBracket => new("]", TokenType.Punctuation);
    public static Token OpenBrace => new("{", TokenType.Punctuation);
    public static Token CloseBrace => new("}", TokenType.Punctuation);

    public static Token Keyword(string symbol) => new(symbol, TokenType.Keyword);

    public string Value { get; }
    public TokenType Type { get; }

    public Token(string value, TokenType type) 
    {
        Value = value;
        Type = type;
    }

    public Token(char value, TokenType type) : this(value.ToString(), type) {}
  
    public bool Is(TokenType type) => Type == type;
    public bool Is(TokenType type, string value) => Is(type) && Value == value;
    public bool Is(Token token) => Is(token.Type, token.Value);

    public override string ToString()
    {
        return $"{Value} ({Type})";
    }
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
