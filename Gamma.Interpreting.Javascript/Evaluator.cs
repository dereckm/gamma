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
    private HashSet<InterpreterEnvironment> _breakTracker = new();

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

    public override void Visit(AstNode node)
    {
        if (node is Evaluation evaluation)
            VisitEvaluation(evaluation);
        else 
            base.Visit(node);
    }

    public override void VisitProgram(Program node)
    {
        _result = false;
        foreach(var exp in node.Body)
        {
            Visit(exp);
            _result = _stack.Pop();    
        }
    }

    public override void VisitBlockStatement(BlockStatement node)
    {
        var env = _env;
        _env = _env!.Extend();
        _returnTracker = new();
        object result = new Undefined();
        foreach(var expression in node.Body)
        {
            Visit(expression);
            if (_stack.Count > 0) {
                result = _stack.Pop();
            }
            if (_returnTracker.Contains(_env!)) 
            {
                _stack.Push(result);
                return;
            }
        }
        _env = env;
        _stack.Push(result);
    }

    public override void VisitLiteral(Literal node)
    {
        _stack.Push(node.Value);
    }

    public override void VisitIdentifier(Identifier node)
    {
        var value = _env!.Get(node.Name);
        _stack.Push(value);
    }

    public override void VisitFunctionCall(FunctionCall node)
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
            var paramIdentifier = function.Parameters[i].As<Identifier>();
            _env.Def(paramIdentifier.Name, argumentValue, "let");
        }
        object result = new Undefined();
        Visit(function.Body);
        if (_stack.Count > 0)
            result = _stack.Pop();
        _stack.Push(result);
        _env = env;
    }

    public override void VisitFunctionReturn(FunctionReturn node)
    {
        _returnTracker.Add(_env!);
        Visit(node.Expression);
    }

    public override void VisitNamedFunctionDeclaration(NamedFunctionDeclaration node)
    {
        _env!.Def(node.Identifier.Name, node, "const");
        _stack.Push(node);
    }

    public override void VisitIfStatement(IfStatement node)
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

    public override void VistiVariableDeclaration(VariableDeclaration node)
    {
        foreach(var declaration in node.Declarations)
        {
            if (declaration is BinaryExpression binary)
            {
                Visit(binary.Right);
                var result = _stack.Peek();
                switch (binary.Left)
                {
                    case Identifier identifierNode:
                        _env!.Def(identifierNode.Name, result, node.Kind);
                        break;
                    case IndexerCall indexerCallNode:
                        var identifier = indexerCallNode.Identifier.Name;
                        var obj = (JavascriptArray)_env!.Get(identifier);
                        Visit(indexerCallNode.Argument);
                        var indexObj = _stack.Pop();
                        var index = GetIndex(indexObj);
                        obj[index] = result;
                        break;
                        
                }
            }
            else if (declaration is Identifier identifierNode)
            {
                _env!.Def(identifierNode.Name, new Undefined(), node.Kind);
                _stack.Push(new Undefined());
            }
        }
    }

    public override void VisitBinaryExpression(BinaryExpression node)
    {
        if (node.Type == "assignment")
        {
            Visit(node.Right);
            var result = _stack.Peek();
            switch (node.Left)
            {
                case Identifier identifierNode:
                    _env!.Set(identifierNode.Name, result);
                    return;
                case IndexerCall indexerCallNode:
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
            var identifier = node.Left.As<Identifier>();
            _env!.Set(identifier.Name, incrementedValue);
            _stack.Push(incrementedValue);
            return;
        }

        var newValue = ApplyOperator(node.Operator, left, right);       
        _stack.Push(newValue);
    }

    public object GetNewValue(string @operator, object value) 
    {
        return @operator switch
        {
            "++" => (int)value + 1,
            "--" => (int)value - 1,
            "!" => !(bool)value,
            _ => throw new NotImplementedException($"Unimplemented unary operator for type ({nameof(Identifier)})"),
        };
    }

    public override void VisitUnaryExpression(UnaryExpression node)
    {
        if (node.Operand is Identifier identifier) 
        {
            object value = _env!.Get(node.Operand.As<Identifier>().Name);
            var newValue = GetNewValue(node.Operator, value);
            _env.Set(identifier.Name, newValue);
            _stack.Push(newValue);
        } 
        else if (node.Operand is FunctionCall fnCall) 
        {
            Visit(fnCall);
            var value = _stack.Pop();
            var newValue = GetNewValue(node.Operator, value);
            _stack.Push(newValue);
        }
        else 
        {
            throw new NotImplementedException($"Unimplemented unary operator type.");
        }
    }

    public override void VisitArray(ArrayNode node)
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

    public override void VisitMemberExpression(MemberExpression node)
    {
        object @object = new Undefined();
        if (node.Object is Identifier identifierNode) 
        {
            @object = _env!.Get(identifierNode.Name);
        }
        else if (node.Object is Literal literalNode)
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
        else if (node.Property is FunctionCall fnCall)
        {
            Visit(node.Object);
            var value = _stack.Pop();
            _env!.Redef("this", value, "const");
            Visit(fnCall);
        }
    }

    public override void VisitIndexerCall(IndexerCall node)
    {
        var identifier = node.Identifier.Name;
        var array = (JavascriptArray)_env!.Get(identifier);
        Visit(node.Argument);
        var indexObject = _stack.Pop();
        var index = GetIndex(indexObject);
        _stack.Push(array[index]);
    }   

    public override void VisitForStatement(ForStatement node)
    {
        Visit(node.Init);
        if (_stack.Count > 0) 
            _ = _stack.Pop(); // consume init
        Visit(node.Test);
        var shouldContinue = (bool)_stack.Pop();
        var parentEnv = _env;
        _env!.Extend();
        object result = new Undefined();
        while (shouldContinue)
        {
            Visit(node.Body);
            if (_stack.Count > 0)
                result = _stack.Pop();
            Visit(node.Update); 
            if (_stack.Count > 0) 
                _ = _stack.Pop(); // consume the update
            if (_breakTracker.Contains(_env)) 
                break;
            Visit(node.Test);
            shouldContinue = (bool)_stack.Pop();
        }
        _stack.Push(result);
        _breakTracker.Remove(_env);
        _env = parentEnv;
    }

    public override void VisitForOfStatement(ForOfStatement node)
    {
        var declaration = node.Left.Declarations[0];
        var identifier = declaration.As<Identifier>();
        var iteratorVarName = $"@@iterator_{Guid.NewGuid()}";
        Visit(node.Right);
        var iteratorValue = _stack.Pop();
        _env!.Def(iteratorVarName, iteratorValue, "const");
        var iteratorMember = new MemberExpression(new Identifier(iteratorVarName), new Identifier("@@iterator"));
        Visit(iteratorMember);
        var enumerator = (IEnumerator)_stack.Pop();
        
        var parentEnv = _env;
        _env = _env.Extend();
        object result = new Undefined();
        while (enumerator.MoveNext())
        {
            _env.Redef(identifier.Name, enumerator.Current, node.Left.Kind);
            Visit(node.Body);
            if (_breakTracker.Contains(_env))
                break;
            result = _stack.Pop();
        }
        _breakTracker.Remove(_env);
        _env = parentEnv;
        _stack.Push(result);
    }

    public override void VisitAnonymousFunctionDeclaration(AnonymousFunctionDeclaration node)
    {
        _stack.Push(node);
    }

    public override void VisitBreakStatement(BreakStatement node)
    {
        _breakTracker.Add(_env!);
    }

    protected override bool ShouldVisit(AstNode node)
    {
        return _breakTracker.Count == 0;
    }

    public void VisitEvaluation(Evaluation evaluation)
    {
        evaluation.Body(_stack, _env!);
    }

    private object ApplyOperator(string op, object a, object b)
    {
        if (op == "**" && a is int int5 && b is int int6)
            return (int)Math.Pow(int5, int6);
        if (a is int int1 && b is int int2)
            return ApplyOperator(op, int1, int2);
        if (a is int int3 && b is double double1)
            return ApplyOperator(op, int3, double1);
        if (a is double double2 && b is int int4)
            return ApplyOperator(op, double2, int4);
        if (a is double double3 && b is double double4)
            return ApplyOperator(op, double3, double4);
        if (a is string s1 && b is char c1 && op == "+")
            return s1 + c1;
        if (a is string s2 && b is string s3 && op == "+")
            return s2 + s3;
        if (a is char c2 && b is string s4 && (op == "===" || op == "=="))
            return c2.ToString() == s4;
        if (a is string s5 && b is char s6 && (op == "===" || op == "=="))
            return s5 == s6.ToString();
        
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

internal class Evaluation(Action<Stack<object>, InterpreterEnvironment> body) : AstNode("evaluation")
{
    public Action<Stack<object>, InterpreterEnvironment> Body { get; } = body;
}