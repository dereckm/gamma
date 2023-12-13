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
        Indented(() => _print($"name: {node.Name}"));
    }

    public override void Visit(LiteralNode node)
    {
        _print("literal:");
        _indentLevel++;
        _print($"type: {node.Type}");
        _print($"value: {node.Value}");
        _indentLevel--;
    }

    public override void Visit(VariableDeclarationNode node)
    {
        _print("variable_decl:");
        _indentLevel++;
        _print($"type: {node.Kind}");
        foreach(var declaration in node.Declarations)
        {
            Visit(declaration);
        }
        _indentLevel--;
    }

    public override void Visit(ArrayNode node)
    {
        _print("array:");
        _print("[");
        _indentLevel++;
        foreach(var item in node.Items)
        {
            Visit(item);
        }
        _indentLevel--;
        _print("]");
    }

    public override void Visit(IndexerCallNode node)
    {
        _print("indexer_call:");
        Indented(() => {
            Visit(node.Identifier);
            Visit(node.Argument);
        });
    }

    public override void Visit(FunctionCallNode node)
    {
        _print("fn_call:");
        _indentLevel++;
        Visit(node.Identifier);
        _print("arguments:");
        _indentLevel++;
        foreach(var argument in node.Arguments)
        {
            Visit(argument);
        }
        _indentLevel--;
        _indentLevel--;
    }

    private void Indented(Action action)
    {
        _indentLevel++;
        action();
        _indentLevel--;
    }

    public override void Visit(AnonymousFunctionDeclaration node)
    {
        _print("anonymous_fn_declaration:");
        Indented(() => {
            _print("parameters:");
            Indented(() => {
                foreach(var parameter in node.Parameters)
                {
                    Visit(parameter);
                }
            });
            _print("body:");
            Indented(() => Visit(node.Body));
        });
    }

    public override void Visit(FunctionReturn node)
    {
        _print("return_statement:");
        Indented(() => Visit(node.Expression));
    }

    public override void Visit(NamedFunctionDeclarationNode node)
    {
        _print("fn_declaration:");
        Indented(() => {
            Visit(node.Identifier);
            _print("parameters:");
            Indented(() => {
                foreach(var parameter in node.Parameters)
                {
                    Visit(parameter);
                }
            });
            _print("body:");
            Indented(() => Visit(node.Body));
        });
    }

    public override void Visit(MemberExpression node)
    {
        _print("member:");
        Indented(() => base.Visit(node));
    }
}