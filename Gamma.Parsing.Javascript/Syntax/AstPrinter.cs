using System.Text;

namespace Gamma.Parsing.Javascript.Syntax;

public class AstPrinter : AstVisitor
{
    private int _indentLevel = 0;
    private Action<string> _print = (_) => {};
    private StringBuilder _sb = new();

    public string Print(AstNode node)
    {
        _sb = new StringBuilder();
        _print = (str) => _sb.AppendLine(str.PadLeft(str.Length + _indentLevel * 2));
        Visit(node);
        return _sb.ToString();
    }

    public override void Visit(IfStatementNode node)
    {
        _print("if:");
        _indentLevel++;
        base.Visit(node);
        _indentLevel--;
    }

    public override void Visit(BinaryExpressionNode node)
    {
        _print("binary:");
        _indentLevel++;
        _print($"type: {node.Type}");
        _print("left:");
        _indentLevel++;
        base.Visit(node.Left);
        _indentLevel--;
        _print($"operator: {node.Operator}");
        _print("right:");
        _indentLevel++;
        base.Visit(node.Right);
        _indentLevel--;
        _indentLevel--;

    }

    public override void Visit(UnaryExpressionNode node)
    {
        _print("unary:");
        _indentLevel++;
        _print($"operator: {node.Operator}");
        _print($"is_suffix: {node.IsSuffix}");
        Visit(node.Operand);
        _indentLevel--;
    }

    public override void Visit(IdentifierNode node)
    {
        _print("identifier:");
        _indentLevel++;
        _print($"name: {node.Name}");
        _indentLevel--;
        base.Visit(node);
    }

    public override void Visit(LiteralNode node)
    {
        _print("literal");
        _indentLevel++;
        _print($"type: {node.Type}");
        _print($"value: {node.Value}");
        _indentLevel--;
    }

    public override void Visit(VariableDeclarationNode node)
    {
        _print("variable_declaration");
        _indentLevel++;
        _print($"type: {node.Kind}");
        foreach(var declaration in node.Declarations)
        {
            Visit(declaration);
        }
        _indentLevel--;
    }
}