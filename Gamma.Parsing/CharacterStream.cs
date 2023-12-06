public class CharacterStream 
{
    private ReadOnlyMemory<char> _code;
    private int _position = 0;
    private int _line = 1;
    private int _column = 0;

    public CharacterStream(string code) 
    {
        _code = code.AsMemory();
    }

    public char Next() 
    {
        var character = _code.Span[_position++];
        if (character == '\n') 
        {
            _line++;
            _column = 0;
        } 
        else 
        {
            _column++;
        }
        return character;
    }

    public char Peek() 
    {
        return _code.Span[_position];
    }

    public char PeekAhead() 
    {
        return _code.Span[_position + 1];
    }

    public bool IsEndOfStream()
    {
        return _position > _code.Length - 1;
    }

    public ParserException Terminate(string message) 
    {
        return new ParserException($"{message} ({_line}:{_column})");
    }
}

public class ParserException : Exception
{
    public ParserException(string message) : base(message) {}
}