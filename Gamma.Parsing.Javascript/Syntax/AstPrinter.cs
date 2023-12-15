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

    public override void VisitIfStatement(IfStatement node)
    {
        _print("if:");
        _indentLevel++;
        base.VisitIfStatement(node);
        _indentLevel--;
    }

    public override void VisitBinaryExpression(BinaryExpression node)
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

    public override void VisitUnaryExpression(UnaryExpression node)
    {
        _print("unary:");
        _indentLevel++;
        _print($"operator: {node.Operator}");
        _print($"is_suffix: {node.IsSuffix}");
        Visit(node.Operand);
        _indentLevel--;
    }

    public override void VisitIdentifier(Identifier node)
    {
        _print("identifier:");
        Indented(() => _print($"name: {node.Name}"));
    }

    public override void VisitLiteral(Literal node)
    {
        _print("literal:");
        _indentLevel++;
        _print($"type: {node.Type}");
        _print($"value: {node.Value}");
        _indentLevel--;
    }

    public override void VistiVariableDeclaration(VariableDeclaration node)
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

    public override void VisitArray(ArrayNode node)
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

    public override void VisitIndexerCall(IndexerCall node)
    {
        _print("indexer_call:");
        Indented(() => {
            VisitIdentifier(node.Identifier);
            Visit(node.Argument);
        });
    }

    public override void VisitFunctionCall(FunctionCall node)
    {
        _print("fn_call:");
        _indentLevel++;
        VisitIdentifier(node.Identifier);
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

    public override void VisitAnonymousFunctionDeclaration(AnonymousFunctionDeclaration node)
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

    public override void VisitFunctionReturn(FunctionReturn node)
    {
        _print("return_statement:");
        Indented(() => Visit(node.Expression));
    }

    public override void VisitNamedFunctionDeclaration(NamedFunctionDeclaration node)
    {
        _print("fn_declaration:");
        Indented(() => {
            VisitIdentifier(node.Identifier);
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

    public override void VisitMemberExpression(MemberExpression node)
    {
        _print("member:");
        Indented(() => base.VisitMemberExpression(node));
    }

    public override void VisitForOfStatement(ForOfStatement node)
    {
        _print("for_of_statement:");
        Indented(() => {
            _print("left:");
            Indented(() => VistiVariableDeclaration(node.Left));
            _print("right:");
            Indented(() => Visit(node.Right));
            _print("body:");
            Indented(() => Visit(node.Body));
        });
    }
}