using Gamma.Parsing;

public class TokenStreamException : Exception
{
    public TokenStreamException(string message) : base(message) {}
}

public class TokenStream
{
    private Token _current = null;

    private CharacterStream _characterStream;

    public TokenStream(CharacterStream characterStream)
    {
        _characterStream = characterStream;
    }

    public Token Peek() 
    {
        if (_current == null) 
        {
            _current = ReadNext();
        }
        return _current;
    }

    public Token Next() 
    {
        var current = Peek();
        _current = ReadNext();
        return current;
    }

    public bool IsEndOfStream() 
    {
        return Peek() == null;
    }

    private Token ReadNext() 
    {
        ReadWhile(IsWhitespace);
        if (_characterStream.IsEndOfStream()) throw new TokenStreamException("Trying to consume token on end of stream.");
        var character = _characterStream.Peek();
        if (character == '/' && _characterStream.PeekAhead() == '/') {
            SkipLine();
            return ReadNext();
        }
        if (character == '"') return ReadString();
        if (IsDigit(character)) return ReadNumber();
        if (IsIdentifierStart(character)) return ReadIdentifier();
        if (IsPunctuation(character)) return new Token(_characterStream.Next(), TokenType.Punctuation);
        if (IsOperator(character)) return new Token(_characterStream.Next(), TokenType.Operator);
        throw _characterStream.Terminate($"Can't handle character: {character}");
    }

    private Token ReadNumber() 
    {
        var hasDecimalPoint = false;
        var number = ReadWhile(character => {
            if (character == '.') 
            {
                if (hasDecimalPoint) return false;
                hasDecimalPoint = true;
                return true;
            }
            return IsDigit(character);
        });

        return new Token(number, TokenType.Number);
    }

    private Token ReadIdentifier() 
    {
        var identifier = ReadWhile(IsIdentifier);
        var type = IsKeyword(identifier) ? TokenType.Keyword : TokenType.Identifier;
        return new Token(identifier, type);
    }

    private string ReadEscaped(char end) 
    {
        var escaped = false;
        var str = "";
        _characterStream.Next();
        while (!_characterStream.IsEndOfStream()) 
        {
            var character = _characterStream.Next();
            if (escaped) 
            {
                str += character;
                escaped = false;
            } 
            else if (character == '\\') 
            {
                escaped = true;
            }
            else if (character == end)
            {
                break;
            }
            else {
                str += character;
            }
        }
        return str;
    }


    private Token ReadString() 
    {
        var str = ReadEscaped('"');
        return new Token(str, TokenType.String);
    }

    private void SkipLine()
    {
        ReadWhile(character => character != '\n');
        _characterStream.Next();
    }

    private void SkipPunctuation(char token)
    {
        if (_current.Value != token.ToString()) 
        {
            throw _characterStream.Terminate($"Unexpected punctuation character - Actual={_current.Value}, Expected={token}");
        }
        Next();
    }

    public Token[] ParseBetween(char start, char stop, char separator) 
    {
        var tokens = new List<Token>();
        var depth = 0;
        SkipPunctuation(start);
        while (!IsEndOfStream())
        {
            if (depth == 0 && _current.Value == stop.ToString()) break;
            if (_current.Value == stop.ToString()) depth--;
            else if (_current.Value == start.ToString()) depth++;
            else if(_current.Value == separator.ToString()) Next();
            tokens.Add(Next());
        }
        SkipPunctuation(stop);
        return tokens.ToArray();
    }

    private static bool IsWhitespace(char character) => char.IsWhiteSpace(character);
    private static bool IsDigit(char character) => char.IsDigit(character);

    private static readonly HashSet<char> Operators = new() { '+', '-', '*', '/', '%', '=', '&', '|', '<', '>', '!' };
    public static bool IsOperator(char character) => Operators.Contains(character);

    private static readonly HashSet<char> Punctuation = new() { ',', ';', '(', ')', '{', '}', '[', ']', '.' };
    private static bool IsPunctuation(char character) => Punctuation.Contains(character);
    private static bool IsIdentifierStart(char character) => char.IsLetter(character) || character == '_';

    private static bool IsIdentifier(char character) => IsIdentifierStart(character) || IsDigit(character);

    private static readonly HashSet<string> Keywords = new () { "if", "else", "var", "const", "true", "false", "let", "function" };
    private static bool IsKeyword(string identifier) => Keywords.Contains(identifier);
   


    private string ReadWhile(Func<char, bool> predicate) 
    {
        var str = "";
        while (!_characterStream.IsEndOfStream() && predicate(_characterStream.Peek()))
            str += _characterStream.Next();
        return str;
    }
}
