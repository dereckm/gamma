// using Gamma.Parsing.Javascript.Syntax;

// namespace Gamma.Parsing.Javascript;
// public class JavascriptParser
// {
//     private TokenStream _tokens;

//     public JavascriptParser(TokenStream tokenStream)
//     {
//         _tokens = tokenStream;
//     }

//     #region helpers

//     public bool Is(TokenType type, string value) 
//     {
//         var token = _tokens.Peek();
//         return token != null 
//             && Is(type) 
//             && token.Value == value;
//     }

//     public bool Is(TokenType type)
//     {
//         var token = _tokens.Peek();
//         return token.Type == type;
//     }

//     public void Skip(TokenType type, string value)
//     {
//         if (Is(type, value)) _tokens.Next();
//         else throw _tokens.Throw($"Expected {type}: {value}");
//     }

//     private static Dictionary<string, int> Precendence = new ()
//     {
//         { "=", 1 },
//         { "||", 2 },
//         { "&&", 3 },
//         { "<", 7 }, { ">", 7 }, { "<=", 7 }, { ">=", 7 }, { "==", 7 }, { "!=", 7 },
//         { "+", 10 }, { "-", 10 },
//         { "*", 20 }, { "/", 20 }, { "%", 20 } 
//     };
//     #endregion

//     public AST Parse()
//     {
//         var program = new List<AST>();
//         while (!_tokens.IsEndOfStream())
//         {
//             program.Add(ParseExpression());
//             if (!_tokens.IsEndOfStream()) Skip(TokenType.Punctuation, ";");
//         }
//         return new Syntax.Program("program", program.ToArray());
//     }

//     public AST MaybeBinary(AST left, int myPrecendence) 
//     {
//         var isOperator = Is(TokenType.Operator);
//         if (isOperator)
//         {
//             var token = _tokens.Peek();
//             var otherPrecendence = Precendence[token.Value];
//             if (otherPrecendence > myPrecendence) 
//             {
//                 _tokens.Next();
//                 return MaybeBinary(new BinaryExpression(
//                     token.Value == "=" ? "assign" : "binary",
//                     token.Value,
//                     left,
//                     MaybeBinary(ParseAtom(), otherPrecendence)
//                 ), myPrecendence);
//             }
//         }
//         return left;
//     }

//     public IEnumerable<AST> Delimited(char start, char stop, char separator, Func<AST> parser)
//     {
//         var contents = new List<AST>();
//         var first = true;
//         Skip(TokenType.Punctuation, start.ToString());
//         while (_tokens.IsEndOfStream())
//         {
//             if (Is(TokenType.Punctuation, stop.ToString())) break;
//             if (first) first = false;
//             else Skip(TokenType.Punctuation, separator.ToString());
//             if (Is(TokenType.Punctuation, stop.ToString())) break;
//             contents.Add(parser());
//         }
//         return contents;
//     }

//     public AST MaybeCall(Func<AST> exp)
//     {
//         var expression = exp();
//         return Is(TokenType.Punctuation, "(") ? ParseCall(expression) : expression;
//     }

//     public AST ParseCall(AST expression)
//     {
//         var arguments = Delimited('(', ')', ',', ParseExpression).ToArray();
//         return new FunctionCall(
//             "call",
//             expression,
//             arguments
//         );
//     }


//     public AST ParseExpression()
//     {
//         return MaybeCall(() => MaybeBinary(ParseAtom(), 0));
//     }


//     public AST ParseAtom()
//     {
//         return MaybeCall(() => {
//             if (Is(TokenType.Punctuation, "(")) {
//                 _tokens.Next();
//                 var exp = ParseExpression();
//                 Skip(TokenType.Punctuation, ")");
//                 return exp;
//             }
//             if (Is(TokenType.Punctuation, "{")) return ParseProgram();
//             if (Is(TokenType.Keyword, "if")) return ParseIf();
//             if (Is(TokenType.Keyword, "true") || Is(TokenType.Keyword, "false")) return ParseBool();
//             var token = _tokens.Next();
//             if (token.Type == TokenType.Number) return new Literal("number", token.Value);
//             if (token.Type == TokenType.String) return new Literal("string", token.Value);
//             throw new JavascriptParserException($"Unexpected token: {_tokens.Peek().Value}");
//         });
//     }

//     public AST ParseBool()
//     {
//         var token = _tokens.Peek();
//         return new Literal("bool", token.Value);
//     }

//     public AST ParseIf()
//     {
//         Skip(TokenType.Keyword, "if");
//         var condition = ParseExpression();
//         var then = ParseExpression();
//         var ret = new Conditional("if", condition, then, null);
//         if (Is(TokenType.Keyword, "else")) {
//             _tokens.Next();
//             ret = ret with { Else = ParseExpression() };
//         }
//         return ret;
//     }

//     public AST ParseProgram() 
//     {
//         var programs = Delimited('{', '}', ';', ParseExpression).ToArray();
//         if (programs.Length == 0) throw new JavascriptParserException("No program");
//         if (programs.Length == 1) return programs[0];
//         return new Syntax.Program("program", programs);
//     }
// }

// public class JavascriptParserException : Exception
// {
//     public JavascriptParserException(string message) : base(message) {}
// }
