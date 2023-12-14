using System.Collections;
using System.Numerics;
using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;

internal partial class Evaluator : AstVisitor
{
    private InterpreterEnvironment? _env;
    private object _result;
    private Stack<object> _stack = new();
    private HashSet<InterpreterEnvironment> _returnTracker = new();

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

    public override void Visit(BlockStatementNode node)
    {
        _returnTracker = new();
        foreach(var expression in node.Body)
        {
            Visit(expression);
            if (_returnTracker.Contains(_env!))
             return;
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
        var env = _env;
        _env = _env!.Extend();
        var function = (FunctionDeclaration)_env.Get(node.Identifier.Name);
        if (node.Arguments.Count > function.Parameters.Count)
            throw new InvalidOperationException("Trying to pass too many arguments..");
        
        for(var i = 0; i < node.Arguments.Count; i++)
        {
            var argument = node.Arguments[i];
            Evaluate(argument);
            var argumentValue = _stack.Pop();
            var paramIdentifier = function.Parameters[i].As<IdentifierNode>();
            _env.Def(paramIdentifier.Name, argumentValue, "let");
        }
        Visit(function.Body);
        _env = env;
    }

    public override void Visit(FunctionReturn node)
    {
        _returnTracker.Add(_env!);
        Visit(node.Expression);
    }

    public override void Visit(NamedFunctionDeclarationNode node)
    {
        _env!.Def(node.Identifier.Name, node, "const");
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
            if (declaration is BinaryExpressionNode binary)
            {
                Visit(binary.Right);
                var result = _stack.Peek();
                switch (binary.Left)
                {
                    case IdentifierNode identifierNode:
                        _env!.Def(identifierNode.Name, result, node.Kind);
                        break;
                    case IndexerCallNode indexerCallNode:
                        var identifier = indexerCallNode.Identifier.Name;
                        var obj = (JavascriptArray)_env!.Get(identifier);
                        Visit(indexerCallNode.Argument);
                        var indexObj = _stack.Pop();
                        var index = GetIndex(indexObj);
                        obj[index] = result;
                        break;
                        
                }
            }
        }
    }

    public override void Visit(BinaryExpressionNode node)
    {
        if (node.Type == "assignment")
        {
            Visit(node.Right);
            var result = _stack.Peek();
            switch (node.Left)
            {
                case IdentifierNode identifierNode:
                    _env!.Set(identifierNode.Name, result);
                    return;
                case IndexerCallNode indexerCallNode:
                    var identifier = indexerCallNode.Identifier.Name;
                    var obj = (JavascriptArray)_env!.Get(identifier);
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
            _stack.Push(incrementedValue);
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
        var values = new JavascriptArray();
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
        object @object = new Undefined();
        if (node.Object is IdentifierNode identifierNode) 
        {
            @object = _env!.Get(identifierNode.Name);
        }
        else if (node.Object is LiteralNode literalNode)
        {
            @object = literalNode.Value;
        }
        
        if (@object is JavascriptArray list) 
        {
            var evaluator = new ArrayEvaluator(list, node, this);
            evaluator.Evaluate();
        }
        else if (@object is string str)
        {
            var evaluator = new StringEvaluator(str, node, this);
            evaluator.Evaluate();
        }
    }

    public override void Visit(IndexerCallNode node)
    {
        var identifier = node.Identifier.Name;
        var array = (JavascriptArray)_env!.Get(identifier);
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

    public override void Visit(ForOfStatement node)
    {
        var declaration = node.Left.Declarations[0];
        var identifier = declaration.As<IdentifierNode>();
        var iteratorVarName = $"@@iterator_{Guid.NewGuid()}";
        Visit(node.Right);
        var iteratorValue = _stack.Pop();
        _env!.Def(iteratorVarName, iteratorValue, "const");
        var iteratorMember = new MemberExpression(new IdentifierNode(iteratorVarName), new IdentifierNode("@@iterator"));
        Visit(iteratorMember);
        var enumerator = (IEnumerator)_stack.Pop();
        
        var parentEnv = _env;
        _env.Extend();
        object result = new Undefined();
        while (enumerator.MoveNext())
        {
            _env.Redef(identifier.Name, enumerator.Current, node.Left.Kind);
            Visit(node.Body);
            result = _stack.Pop();
        }
        _env = parentEnv;
        _stack.Push(result);
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
        if (a is string s1 && b is char c1)
            return s1 + c1;
        if (a is string s2 && b is string s3)
            return s2 + s3;
        throw new NotImplementedException($"Type combinaison not supported: ({a.GetType().Name}, {b.GetType().Name})");
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
            "==" => a == b,
            "===" => a == b,
            _ => throw new NotImplementedException(),
        };
    }
}