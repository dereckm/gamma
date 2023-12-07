
using System.Text;
using System.Text.Json;
using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

var code = "if (x > 10) { y = 5; } else if (x < 0) { y = -5; } else { y = 0; }";

var parser = new Parser();
var ast = parser.Parse(code);

var printer = new AstPrinter();
printer.Print(ast);

Console.WriteLine("Done.");

public class AstPrinter : AstVisitor
{
    private int _indentLevel = 0;
    private Action<string> _print = (_) => {};

    public void Print(AstNode node)
    {
        var sb = new StringBuilder();
        _print = (str) => sb.AppendLine(str.PadLeft(str.Length + _indentLevel * 2));
        Visit(node);
        Console.WriteLine(sb.ToString());
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
}