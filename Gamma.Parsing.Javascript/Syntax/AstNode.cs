namespace Gamma.Parsing.Javascript.Syntax;

// Base class for all AST nodes
public abstract class AstNode(string type)
{
    public static AstNode Dead => new DeadNode("dead");

    public string Type { get; } = type;

    public T As<T>() where T : AstNode => (T)this;
}

public class FunctionReturn(string type, AstNode expression) : AstNode(type)
{
    public AstNode Expression { get; } = expression;
}

public class MemberExpression(IdentifierNode @object, AstNode property) : AstNode("member")
{
    public IdentifierNode Object { get; } = @object;
    public AstNode Property { get; } = property;
}

public class DeadNode(string type) : AstNode(type) 
{
}

// Program-related nodes
public class ProgramNode : AstNode
{
    public ProgramNode(string type, IEnumerable<AstNode> statements) : base(type) 
    {
        Body.AddRange(statements);
    }

    public List<AstNode> Body { get; } = [];
}

public class ArrayNode : AstNode
{
    public ArrayNode(IEnumerable<AstNode> items) : base("array") 
    {
        Items.AddRange(items);
    }

    public List<AstNode> Items { get; } = [];
}

public class BlockStatementNode : AstNode
{
    public BlockStatementNode(string type, IEnumerable<AstNode> nodes) : base(type) 
    {
        Body.AddRange(nodes);
    }

    public List<AstNode> Body { get; } = [];
}

// Declaration nodes
public class VariableDeclarationNode : AstNode
{
    public VariableDeclarationNode(string type, string kind, IEnumerable<AstNode> declarations) : base(type) 
    {
        Kind = kind;
        Declarations.AddRange(declarations);
    }
    public string Kind { get; } // e.g., "let", "const", "var"
    public List<AstNode> Declarations { get; } = [];
}

public class AnonymousFunctionDeclaration(
    IEnumerable<AstNode> parameters,
    AstNode body
        ) : FunctionDeclaration("anonymous_function_declaration", parameters, body)
{
}

public abstract class FunctionDeclaration : AstNode
{
    protected FunctionDeclaration(
        string type,
        IEnumerable<AstNode> parameters,
        AstNode body) : base(type)
    {
        Parameters.AddRange(parameters);
        Body = body;
    }

    public List<AstNode> Parameters { get; } = [];
    public AstNode Body { get; }
}

public class NamedFunctionDeclarationNode(
    string type,
    IdentifierNode identifier,
    IEnumerable<AstNode> parameters,
    AstNode body
        ) : FunctionDeclaration(type, parameters, body)
{
    public IdentifierNode Identifier { get; } = identifier;
}

// Expression nodes
public class IdentifierNode(string type, string name) : AstNode(type)
{
    public string Name { get; } = name;
}

public class LiteralNode(string type, object value) : AstNode(type)
{
    public object Value { get; set; } = value;
}

public class BinaryExpressionNode(string type, AstNode left, string @operator, AstNode right) : AstNode(type)
{
    public string Operator { get; } = @operator;
    public AstNode Left { get; } = left;
    public AstNode Right { get; } = right;
}

// Control Flow nodes
public class IfStatementNode(string type, AstNode test, AstNode consequent) : AstNode(type)
{
    public AstNode Test { get; } = test;
    public AstNode Consequent { get; } = consequent;
    public AstNode Alternate { get; set; } = Dead;
}

public class FunctionCallNode : AstNode
{
    public FunctionCallNode(
        string type,
        IdentifierNode identifierNode,
        IEnumerable<AstNode> arguments) : base(type)
    {
        Identifier = identifierNode;
        Arguments.AddRange(arguments);
    }

    public IdentifierNode Identifier { get; }

    public List<AstNode> Arguments { get; } = [];
}

public class IndexerCallNode(IdentifierNode identifierNode, AstNode argument) : AstNode("indexer_call")
{
    public IdentifierNode Identifier { get; } = identifierNode;

    public AstNode Argument { get; } = argument;
}

// Loop and Iteration nodes
public class ForStatementNode(
    string type,
    AstNode init,
    AstNode test,
    AstNode update,
    AstNode body) : AstNode(type)
{
    public AstNode Init { get; } = init;
    public AstNode Test { get; } = test;
    public AstNode Update { get; } = update;
    public AstNode Body { get; } = body;
}

public class UnaryExpressionNode(
    string type,
    AstNode operand,
    string @operator) : AstNode(type)
{
    public AstNode Operand { get; } = operand;
    public string Operator { get; } = @operator;
    public bool IsSuffix { get; } = false;
}

// // Exception Handling nodes
// public class TryStatementNode : AstNode
// {
//     public AstNode Block { get; set; }
//     public AstNode Handler { get; set; }
// }

// public class CatchClauseNode : AstNode
// {
//     public AstNode Param { get; set; }
//     public AstNode Body { get; set; }
// }