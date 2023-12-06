namespace Gamma.Parsing.Javascript;
public class JavascriptParser
{
    private TokenStream _tokens;

    public JavascriptParser(TokenStream tokenStream)
    {
        _tokens = tokenStream;
    }

    public Function ParseFunction()
    {
        var identifier = _tokens.Next();
        var parameterTokens = _tokens.ParseBetween('(', ')', ',');
        var parameters = parameterTokens.Select(p => new Parameter(p.Value)).ToArray();
        return new Function(identifier.Value, parameters);
    }
}


public record AST();
public record Function(string Identifier, Parameter[] Parameters);
public record Parameter(string Identifier);





