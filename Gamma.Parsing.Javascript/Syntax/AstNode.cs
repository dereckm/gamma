using System.Text.Json.Serialization;

namespace Gamma.Parsing.Javascript.Syntax;

// Base class for all AST nodes
public abstract class AstNode 
{
    protected AstNode(string type)
    {
        Type = type;
    }

    public static AstNode Dead => new DeadNode("dead");

    public string Type { get; } = "Unknown";

    public T As<T>() where T : AstNode => (T)this;
}

public class FunctionReturn : AstNode
{
    public FunctionReturn(string type, AstNode expression) : base(type)
    {
        Expression = expression;
    }

    public AstNode Expression { get; }
}

public class MemberExpression : AstNode
{
    public MemberExpression(IdentifierNode @object, AstNode property) : base("member")
    {
        Object = @object;
        Property = property;
    }

    public IdentifierNode Object { get; }
    public AstNode Property { get; }
}

public class DeadNode : AstNode 
{ 
    public DeadNode(string type) : base(type) {}
}

// Program-related nodes
public class ProgramNode : AstNode
{
    public ProgramNode(string type, IEnumerable<AstNode> statements) : base(type) 
    {
        Body.AddRange(statements);
    }

    public List<AstNode> Body { get; } = new List<AstNode>();
}

public class ArrayNode : AstNode
{
    public ArrayNode(IEnumerable<AstNode> items) : base("array") 
    {
        Items.AddRange(items);
    }

    public List<AstNode> Items { get; } = new ();
}

public class BlockStatementNode : AstNode
{
    public BlockStatementNode(string type, IEnumerable<AstNode> nodes) : base(type) 
    {
        Body.AddRange(nodes);
    }

    public List<AstNode> Body { get; } = new List<AstNode>();
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
    public List<AstNode> Declarations { get; } = new List<AstNode>();
}

public class FunctionDeclarationNode : AstNode
{
    public FunctionDeclarationNode(
        string type,
        IdentifierNode identifier,
        IEnumerable<AstNode> parameters,
        AstNode body
        ) : base(type) 
    {
        Identifier = identifier;
        Parameters.AddRange(parameters);
        Body = body;
    }
    public IdentifierNode Identifier { get; }
    public List<AstNode> Parameters { get; } = new List<AstNode>();
    public AstNode Body { get; }
}

// Expression nodes
public class IdentifierNode : AstNode
{
    public IdentifierNode(string type, string name) : base(type) 
    {
        Name = name;
    }
    public string Name { get; }
}

public class LiteralNode : AstNode
{
    public LiteralNode(string type, object value) : base(type) 
    {
        Value = value;
    }
    public object Value { get; set; }
}

public class BinaryExpressionNode : AstNode
{
    public BinaryExpressionNode(string type, AstNode left, string @operator, AstNode right) : base(type)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
    
    public string Operator { get; }
    public AstNode Left { get; }
    public AstNode Right { get; }
}

// Control Flow nodes
public class IfStatementNode : AstNode
{
    public IfStatementNode(string type, AstNode test, AstNode consequent) : base(type)
    {
        Test = test;
        Consequent = consequent;
    }

    public AstNode Test { get; }
    public AstNode Consequent { get; }
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

    public List<AstNode> Arguments { get; } = new();
}

public class IndexerCallNode : AstNode
{
    public IndexerCallNode(IdentifierNode identifierNode, AstNode argument) : base("indexer_call")
    {
        Identifier = identifierNode;
        Argument = argument;
    }

    public IdentifierNode Identifier { get; }

    public AstNode Argument { get; }
}

// // Function-related nodes
// public class FunctionExpressionNode : AstNode
// {
//     public List<AstNode> Params { get; set; } = new List<AstNode>();
//     public AstNode Body { get; set; }
// }

// // Object and Array nodes
// public class ObjectExpressionNode : AstNode
// {
//     public List<AstNode> Properties { get; set; } = new List<AstNode>();
// }

// Loop and Iteration nodes
public class ForStatementNode : AstNode
{
    public ForStatementNode(
        string type,
        AstNode init,
        AstNode test,
        AstNode update,
        AstNode body) : base(type)
    {
        Init = init;
        Test = test;
        Update = update;
        Body = body;
    }

    public AstNode Init { get; }
    public AstNode Test { get;}
    public AstNode Update { get; }
    public AstNode Body { get; }
}

public class UnaryExpressionNode : AstNode
{
    public UnaryExpressionNode(
        string type, 
        AstNode operand,
        string @operator) : base(type)
    {
        Operand = operand;
        Operator = @operator;
    }

    public AstNode Operand { get; }
    public string Operator { get; }
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