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

public class DeadNode : AstNode 
{ 
    public DeadNode(string type) : base(type) {}
}

// Program-related nodes
public class ProgramNode : AstNode
{
    public ProgramNode(string type) : base(type) {}
    public List<AstNode> Body { get; } = new List<AstNode>();
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
    public VariableDeclarationNode(string type, string kind) : base(type) 
    {
        Kind = kind;
    }
    public string Kind { get; } // e.g., "let", "const", "var"
    public List<AstNode> Declarations { get; } = new List<AstNode>();
}

public class FunctionDeclarationNode : AstNode
{
    public FunctionDeclarationNode(string type, string identifier, AstNode body) : base(type) 
    {
        Identifier = identifier;
        Body = body;
    }
    public string Identifier { get; }
    public List<AstNode> Params { get; } = new List<AstNode>();
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
// public class ForStatementNode : AstNode
// {
//     public AstNode Init { get; set; }
//     public AstNode Test { get; set; }
//     public AstNode Update { get; set; }
//     public AstNode Body { get; set; }
// }

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