using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;

internal class Evaluator : AstVisitor
{
    private InterpreterEnvironment? _env;
    private object _result;
    private Stack<object> _stack = new();

    public Evaluator(InterpreterEnvironment env)
    {
        _env = env;
        _result = false;
    }

    public object Evaluate(AstNode astNode)
    {
        _stack = new();
        Visit(astNode);
        return _result;
    }

    public override void Visit(ProgramNode node)
    {
        _result = false;
        foreach(var exp in node.Body)
        {
            Visit(exp);
            _result = _stack.Pop();    
        }
    }

    public override void Visit(LiteralNode node)
    {
        _stack.Push(node.Value);
    }

    public override void Visit(IdentifierNode node)
    {
        _stack.Push(_env!.Get(node.Name));
    }

    public override void Visit(IfStatementNode node)
    {
        Visit(node.Test);
        var test = (bool)_stack.Pop();
        if (test) 
        {
            Visit(node.Consequent);
        } 
        else if (node.Alternate != AstNode.Dead) 
        {
            Visit(node.Alternate);
        }
    }

    public override void Visit(BinaryExpressionNode node)
    {
        if (node.Type.StartsWith("var_declaration_") || node.Type == "assignment")
        {
            Visit(node.Right);
            var result = _stack.Peek();
            _env!.Set(node.Left.As<IdentifierNode>().Name, result);
            return;
        }

        Visit(node.Left);
        Visit(node.Right);
        var right = _stack.Pop();
        var left = _stack.Pop();

        
        if (node.Operator == "+=") 
        {
            var incrementedValue = ApplyOperator("+", left, right);
            var identifier = node.Left.As<IdentifierNode>();
            _env!.Set(identifier.Name, incrementedValue);
            return;
        }

        var newValue = ApplyOperator(node.Operator, left, right);       
        _stack.Push(newValue);
    }

    public override void Visit(UnaryExpressionNode node)
    {
        var identifier = node.Operand.As<IdentifierNode>().Name;
        object value = _env!.Get(node.Operand.As<IdentifierNode>().Name);
        switch(node.Operator) 
        {
            case "++":
                _env.Set(identifier, (int)value + 1);
                break;
            case "--":
                _env.Set(identifier, (int)value - 1);
                break;
            default: 
                throw new NotImplementedException($"Unimplemented unary operator: {node.Operator}");
        }
    }

    public override void Visit(ForStatementNode node)
    {
        Visit(node.Init);
        Visit(node.Test);
        var shouldContinue = (bool)_stack.Pop();
        while (shouldContinue)
        {
            Visit(node.Body);
            Visit(node.Update);
            Visit(node.Test);
            shouldContinue = (bool)_stack.Pop();
        }
    }

    private object ApplyOperator(string op, object a, object b)
    {

        if (a is int int1 && b is int int2)
        {
            switch (op)
            {
                case "+" : return int1 + int2;
                case "-" : return int1 - int2;
                case "*" : return int1 * int2;
                case "/" : return int1 / int2;
                case "%" : return int1 % int2;
                case "<" : return int1 < int2;
                case "<=" : return int1 <= int2;
                case ">" : return int1 > int2;
                case ">=" : return int1 >= int2;
            }
        }
        else if (a is double d1 && b is int int3)
        {
            switch (op)
            {
                case "+" : return d1 + int3;
                case "-" : return d1 - int3;
                case "*" : return d1 * int3;
                case "/" : return d1 / int3;
                case "%" : return d1 % int3;
            }
        }
        else if (a is int int4 && b is double d2)
        {
            switch (op)
            {
                case "+" : return int4 + d2;
                case "-" : return int4 - d2;
                case "*" : return int4 * d2;
                case "/" : return int4 / d2;
                case "%" : return int4 % d2;
            }
        }
        throw new NotImplementedException();
    }
}