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
        var statements = new List<AstNode>();
        while (!_tokens.IsEndOfStream()) 
        {
            var expression = ParseExpression();
            if (expression is not DeadNode) 
                statements.Add(expression);
        }
        if (statements.Count == 1)
        {
            return statements[0];
        }

        return new ProgramNode("program", statements);
    }

    public AstNode ParseExpression() 
    {
        var token = _tokens.Peek();
        if (token.Type == TokenType.Keyword)
        {
            if (token.Value == "let" || token.Value == "const" || token.Value == "var")
                return ParseVariableDeclaration();
            if (token.Value == "if")
                return ParseIfStatement();
            if (token.Value == "true" || token.Value == "false")
                return MaybeBinary(ParseBool(), 0);
            if (token.Value == "for")
                return ParseForLoop();
            if (token.Value == "function")
                return ParseFunctionDeclaration();
            if (token.Value == "return") 
                return ParseFunctionReturn();
        }
        if (token.Type == TokenType.Number)
        {
            return MaybeBinary(ParseNumber(), 0);
        }
        if (token.Type == TokenType.String) 
        {
            return MaybeBinary(ParseString(), 0);
        }
        if (token.Type == TokenType.Identifier) 
        {
            var identifier = MaybeMember(ParseVariable());
            return MaybeIndexer(MaybeCall(identifier));
        }
        if (token.Is(TokenType.Punctuation, "("))
        {
            _tokens.Consume(new Token("(", TokenType.Punctuation));
            var expression = ParseExpression();
            _tokens.Consume(new Token(")", TokenType.Punctuation));
            return MaybeBinary(expression, 0);
        }
        if (token.Is(TokenType.Operator) 
            && token.Value is "++" or "--")
        {
            var operatorToken = _tokens.Next();
            var type = operatorToken.Value == "++" ? "inc" : "dec";
            var operand = ParseExpression();
            return new UnaryExpressionNode(type, operand, operatorToken.Value);
        }
        if (token.Is(TokenType.Operator) && token.Value is "-")
        {
            var operatorToken = _tokens.Next();
            var operand = ParseExpression();
            return new UnaryExpressionNode("minus", operand, operatorToken.Value);
        }
        if (token.Is(TokenType.Operator) && token.Value is "!")
        {
            var operatorToken = _tokens.Next();
            var operand = ParseExpression();
            return new UnaryExpressionNode("not", operand, operatorToken.Value);
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
        if (token.Is(TokenType.Punctuation, "["))
        {
            return ParseArray();
        }

        throw new NotImplementedException();
    }

    public AstNode ParseArray()
    {
        var items = new List<AstNode>();
        _tokens.Consume(new Token("[", TokenType.Punctuation));
        while (!_tokens.Peek().Is(TokenType.Punctuation, "]"))
        {
            var expression = ParseExpression();
            items.Add(expression);

            if (_tokens.Peek().Is(TokenType.Punctuation, ","))
                _tokens.Consume(_tokens.Peek());
        }
        _tokens.Consume(new Token("]", TokenType.Punctuation));

        return new ArrayNode(items);
    }

    public AstNode MaybeMember(AstNode node) 
    {
        var next = _tokens.Peek();
        if (next.Is(TokenType.Punctuation, ".") && node is IdentifierNode @object) 
        {
            _tokens.Consume(next);
            var member = ParseExpression();
            return new MemberExpression(@object, member);
        }

        return node;
    }

    public AstNode ParseFunctionDeclaration()
    {
        _tokens.Consume(new Token("function", TokenType.Keyword));
        var identifier = ParseIdentifier();
        _tokens.Consume(new Token("(", TokenType.Punctuation));
        var parameters = new List<AstNode>();
        while (!_tokens.Peek().Is(TokenType.Punctuation, ")")) 
        {
            var expression = ParseExpression();
            parameters.Add(expression);
            
            if (_tokens.Peek().Is(TokenType.Punctuation, ","))
                _tokens.Consume(_tokens.Peek());
        }
        _tokens.Consume(new Token(")", TokenType.Punctuation));
        var body = ParseExpression();

        return new FunctionDeclarationNode(
            "function_declaration",
            identifier,
            parameters,
            body);
    }

    public AstNode MaybeIndexer(AstNode node)
    {
        var next = _tokens.Peek();
        if (next.Is(TokenType.Punctuation, "[") && node is IdentifierNode identifier)
        {
            _tokens.Consume(next);
            var argument = ParseExpression();
            _tokens.Consume(new Token("]", TokenType.Punctuation));
            var indexerCall = new IndexerCallNode(identifier, argument);
            return MaybeBinary(MaybeAssignment(indexerCall), 0);
        }

        return node;
    }

    public AstNode MaybeCall(AstNode node) 
    {
        var next = _tokens.Peek();
        var arguments = new List<AstNode>();
        if (next.Is(TokenType.Punctuation, "(") && node is IdentifierNode identifier)
        {
            _tokens.Consume(next);
            while (!_tokens.Peek().Is(TokenType.Punctuation, ")")) 
            {
                var expression = ParseExpression();
                if (_tokens.Peek().Is(TokenType.Punctuation, ","))
                    _tokens.Consume(_tokens.Peek());
                if (expression is not DeadNode)
                    arguments.Add(expression);
            }
            _tokens.Consume(new Token(")", TokenType.Punctuation));
            var functionCall = new FunctionCallNode(
                "function_call",
                identifier,
                arguments
                );
            return MaybeBinary(functionCall, 0);
        }

        return MaybeBinary(MaybeAssignment(node), 0);
    }

    public AstNode ParseForLoop()
    {
        var init = AstNode.Dead;
        var test = AstNode.Dead;
        var update = AstNode.Dead;

        _tokens.Consume(new Token("for", TokenType.Keyword));
        _tokens.Consume(new Token("(", TokenType.Punctuation));

        var next = _tokens.Peek();
        if (!next.Is(TokenType.Punctuation, ";")) {
            init = ParseExpression();
        }
        _tokens.Consume(new Token(";", TokenType.Punctuation));

        next = _tokens.Peek();
        if (!next.Is(TokenType.Punctuation, ";")) {
            test = ParseExpression();
        }
        _tokens.Consume(new Token(";", TokenType.Punctuation));

        next = _tokens.Peek();
        if (!next.Is(TokenType.Punctuation, ")")) {
            update = ParseExpression();
        }

        _tokens.Consume(new Token(")", TokenType.Punctuation));
        var body = ParseExpression();


        return new ForStatementNode(
            "for",
            init,
            test,
            update,
            body
            );
    }

    public AstNode ParseFunctionReturn() 
    {
        _tokens.Consume(new Token("return", TokenType.Keyword));
        var expression = ParseExpression();
        return new FunctionReturn("return", expression);
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

    public AstNode ParseString()
    {
        var token = _tokens.Next();
        return new LiteralNode("string", token.Value);
    }

    public IdentifierNode ParseVariable()
    {
        var identifierToken = _tokens.Next();
        return new IdentifierNode("identifier", identifierToken.Value);
    }

    public AstNode ParseStatementBlock()
    {
        _tokens.Consume(new Token("{", TokenType.Punctuation));
        var statements = new List<AstNode>();
        while (!_tokens.Peek().Is(TokenType.Punctuation, "}")) 
        {
            var expression = ParseExpression();
            if (expression is not DeadNode)
                statements.Add(expression);
        }
        _tokens.Consume(new Token("}", TokenType.Punctuation));
        return statements.Count == 1 
            ? statements[0] 
            : new BlockStatementNode("block", statements);
    }


    public AstNode ParseIfStatement()
    {
        _tokens.Consume(new Token("if", TokenType.Keyword));
        var test = ParseExpression();

        AstNode alternate = AstNode.Dead;

        var consequent = ParseExpression();
        var next = _tokens.Peek();
        if (next != null && next.Is(TokenType.Keyword, "else"))
        {
            _tokens.Consume(next);
            alternate = ParseExpression();
        }
        
        return new IfStatementNode("if", test, consequent)
        {
            Alternate = alternate
        };
    }

    public AstNode MaybeBinary(AstNode left, int myPrecendence) 
    {
        var token = _tokens.Peek();
        if (token.Is(TokenType.Operator) &&
            token.Value is "++" or "--")
        {
            var operatorToken = _tokens.Next();
            var type = operatorToken.Value == "++" ? "inc" : "dec";
            return new UnaryExpressionNode(
                type,
                left,
                operatorToken.Value
            );
        }

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

    public AstNode MaybeAssignment(AstNode left)
    {
        var token = _tokens.Peek();
        if (token.Is(TokenType.Operator, "=")) 
        {
            _tokens.Consume(token);
            var right = ParseExpression();

            return new BinaryExpressionNode(
                "assignment",
                left,
                "=",
                right
            );
        }

        return left;
    }

    public AstNode ParseVariableDeclaration()
    {
        var keyword = _tokens.Next();
        var type = keyword.Value;
        var left = ParseIdentifier();

        if (_tokens.Peek().Is(TokenType.Operator, "=")) 
        {
            var right = MaybeAssignment(left);

            return new VariableDeclarationNode("variable_declaration", type, new [] { right });
        }

        return left;
    }

    public IdentifierNode ParseIdentifier() 
    {
        var identifierToken = _tokens.Next();
        if (identifierToken.Type != TokenType.Identifier) throw _tokens.Throw("Expected identifier token.");
        return new IdentifierNode("identifier", identifierToken.Value);
    }
}