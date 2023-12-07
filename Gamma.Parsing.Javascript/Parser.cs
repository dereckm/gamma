using Gamma.Parsing;
using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Parsing.Javascript;

public class Parser
{
    private TokenStream _tokens = TokenStream.Empty;

    private Precendences _precendences = new();

    public AstNode Parse(string code)
    {
        _tokens = new TokenStream(new CharacterStream(code));
        return ParseExpression();
        throw new NotImplementedException();
    }

    public AstNode ParseBool()
    {
        var token = _tokens.Next();
        return new LiteralNode("bool", bool.Parse(token.Value));
    }

    public AstNode ParseNumber()
    {
        var numberToken = _tokens.Next();
        return new LiteralNode ("number", int.Parse(numberToken.Value));
    }

    public AstNode ParseVariable()
    {
        var identifierToken = _tokens.Next();
        return new IdentifierNode("variable", identifierToken.Value);
    }

    public AstNode ParseStatementBlock()
    {
        _tokens.Consume(new Token("{", TokenType.Punctuation));
        var statements = new List<AstNode>();
        while (!_tokens.Peek().Is(TokenType.Punctuation, "}")) 
        {
            statements.Add(ParseExpression());
        }
        _tokens.Consume(new Token("}", TokenType.Punctuation));
        return statements.Count == 1 
            ? statements[0] 
            : new BlockStatementNode("block", statements);
    }


    public AstNode ParseExpression() 
    {
        var token = _tokens.Peek();
        if (token.Type == TokenType.Keyword)
        {
            if (token.Value == "let" || token.Value == "const" || token.Value == "var")
                return ParseAssignmentExpression();
            if (token.Value == "if")
                return ParseIfStatement();
            if (token.Value == "true" || token.Value == "false")
                return MaybeBinary(ParseBool(), 0);
        }
        if (token.Type == TokenType.Number)
        {
            return MaybeBinary(ParseNumber(), 0);
        }
        if (token.Type == TokenType.Identifier) 
        {
            return MaybeBinary(ParseVariable(), 0);
        }
        if (token.Is(TokenType.Punctuation, "("))
        {
            _tokens.Consume(new Token("(", TokenType.Punctuation));
            var expression = ParseExpression();
            _tokens.Consume(new Token(")", TokenType.Punctuation));
            return expression;
        }
        if (token.Is(TokenType.Punctuation, "{"))
        {
           return ParseStatementBlock();
        }
        if (token.Is(TokenType.Punctuation, ";"))
        {
            _tokens.Consume(token);
            return AstNode.Dead;
        }

        throw new NotImplementedException();
    }

    public AstNode ParseIfStatement()
    {
        _tokens.Consume(new Token("if", TokenType.Keyword));
        var test = ParseExpression();

        var next = _tokens.Peek();
        AstNode consequent = AstNode.Dead;
        AstNode alternate = AstNode.Dead;
        if (next.Is(TokenType.Punctuation, "{"))
        {
            consequent = ParseExpression();
            next = _tokens.Peek();
            if (null != null && next.Is(TokenType.Keyword, "else"))
            {
                alternate = ParseExpression();
            }
        }

        return new IfStatementNode("if", test, consequent)
        {
            Alternate = alternate
        };
    }

    public AstNode MaybeBinary(AstNode left, int myPrecendence) 
    {
        var token = _tokens.Peek();
        if (token.Is(TokenType.Operator)) 
        {
            var operatorToken = _tokens.Next();
            var otherPrecendence = _precendences[token.Value];
            if (otherPrecendence > myPrecendence) 
            {
                
                return MaybeBinary(
                    new BinaryExpressionNode(
                        "binary",
                        left,
                        operatorToken.Value,
                        MaybeBinary(ParseExpression(), otherPrecendence))
                    , myPrecendence);
            }
        }

        return left;
    }


    public AstNode ParseAssignmentExpression()
    {
        var keyword = _tokens.Next();
        var type = keyword.Value;
        var left = ParseIdentifier();

        if (_tokens.Peek().Is(TokenType.Operator, "=")) 
        {
            _tokens.Next();
            var right = ParseExpression();

            return new BinaryExpressionNode(
                $"assignment_{type}",
                left,
                "=",
                right
            );
        }

        return left;
    }

    public AstNode ParseIdentifier() 
    {
        var identifierToken = _tokens.Next();
        if (identifierToken.Type != TokenType.Identifier) throw _tokens.Throw("Expected identifier token.");
        return new IdentifierNode("variable_declaration", identifierToken.Value);
    }
}