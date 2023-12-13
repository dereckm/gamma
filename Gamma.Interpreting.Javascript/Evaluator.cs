using System.Numerics;
using System.Runtime.InteropServices;
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
        if (_stack.Count == 1) return _stack.Peek();
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
        var value = _env!.Get(node.Name);
        _stack.Push(value);
    }

    public override void Visit(FunctionCallNode node)
    {
        var function = (FunctionDeclaration)_env.Get(node.Identifier.Name);
        if (node.Arguments.Count > function.Parameters.Count)
            throw new InvalidOperationException("Trying to pass too many arguments..");
        
        for(var i = 0; i < node.Arguments.Count; i++)
        {
            var argument = node.Arguments[i];
            Evaluate(argument);
            var argumentValue = _stack.Pop();
            var paramIdentifier = function.Parameters[i].As<IdentifierNode>();
            _env.Def(paramIdentifier.Name, argumentValue);
        }
        Visit(function.Body);
    }

    public override void Visit(FunctionReturn node)
    {
        Visit(node.Expression);
    }

    public override void Visit(NamedFunctionDeclarationNode node)
    {
        _env.Set(node.Identifier.Name, node);
        _stack.Push(node);
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

    public override void Visit(VariableDeclarationNode node)
    {
        foreach(var declaration in node.Declarations)
        {
            Visit(declaration);
        }
    }

    public override void Visit(BinaryExpressionNode node)
    {
        if (node.Type == "assignment")
        {
            Visit(node.Right);
            var result = _stack.Peek();
            if (node.Left is IdentifierNode identifierNode)
            {
                _env!.Set(identifierNode.Name, result);
                return; 
            }
            if (node.Left is IndexerCallNode indexerCallNode)
            {
                var identifier = indexerCallNode.Identifier.Name;
                var obj = (List<object>)_env.Get(identifier);
                Visit(indexerCallNode.Argument);
                var indexObj = _stack.Pop();
                var index = GetIndex(indexObj);
                obj[index] = result;
                return;
            }
        }

        Visit(node.Left);
        var left = _stack.Pop();
        Visit(node.Right);
        var right = _stack.Pop();

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
                _stack.Push((int)value + 1);
                break;
            case "--":
                _env.Set(identifier, (int)value - 1);
                _stack.Push((int)value - 1);
                break;
            default: 
                throw new NotImplementedException($"Unimplemented unary operator: {node.Operator}");
        }
    }

    public override void Visit(ArrayNode node)
    {
        var values = new List<object>();
        foreach(var item in node.Items)
        {
            Visit(item);
            var itemValue = _stack.Pop();
            values.Add(itemValue);
        }
        _stack.Push(values);
    }

    private static int GetIndex(object indexObject)
    {
        if (indexObject is int integer) 
            return integer;
        else if (indexObject is string str && int.TryParse(str, out var stringInteger))
            return stringInteger;
        throw new NotImplementedException($"Type of indexer is not supported! Type={indexObject.GetType().Name}");
    }

    public override void Visit(MemberExpression node)
    {
        var objectName = node.Object.Name;
        var @object = _env.Get(objectName);

        var property = node.Property.As<IdentifierNode>();
        var propertyName = property.Name;
        
        if (@object is List<object> list) 
        {
            switch (propertyName)
            {
                case "length":
                    _stack.Push(list.Count);
                    break;
                default:
                    throw new NotImplementedException($"Member doesn't exist on array ([]), Member={propertyName}");
            }
        }
    }

    public override void Visit(IndexerCallNode node)
    {
        var identifier = node.Identifier.Name;
        var array = (List<object>)_env.Get(identifier);
        Visit(node.Argument);
        var indexObject = _stack.Pop();
        var index = GetIndex(indexObject);
        _stack.Push(array[index]);
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
            var _ = _stack.Pop(); // consume the update
            Visit(node.Test);
            shouldContinue = (bool)_stack.Pop();
        }
    }

    public override void Visit(AnonymousFunctionDeclaration node)
    {
        _stack.Push(node);
    }

    private object ApplyOperator(string op, object a, object b)
    {
        if (a is int int1 && b is int int2)
            return ApplyOperator(op, int1, int2);
        if (a is int int3 && b is double double1)
            return ApplyOperator(op, int3, double1);
        if (a is double double2 && b is int int4)
            return ApplyOperator(op, double2, int4);
        if (a is double double3 && b is double double4)
            return ApplyOperator(op, double3, double4);
        throw new NotImplementedException("Type combinaison not supported: ({a.GetType().Name}, {b.GetType().Name})");
    }

    private object ApplyOperator<T>(string op, T a, T b)
        where T: INumber<T>
    {
        return op switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => a / b,
            "%" => a % b,
            "<" => a < b,
            "<=" => a <= b,
            ">" => a > b,
            ">=" => a >= b,
            _ => throw new NotImplementedException(),
        };
    }
}