using System.Collections;
using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;
public class JavascriptInterpreter
{
    private Parser _parser = new();
    private InterpreterEnvironment _env;

    public JavascriptInterpreter()
    {
        _env = new InterpreterEnvironment();
        Prepare();
    }

    public object Evaluate(AstNode ast)
    {
        var evaluator = new Evaluator(_env);
        var result = evaluator.Evaluate(ast);
        return result;
    }

    internal void Prepare()
    {
        NumberNaN();
        IsNaN();
        ParseInt();
        ToJsString();
    }

    private void ToJsString()
    {
        CreateFunction("toString", (stack, env) => {
            var obj = env.Get("this");
            stack.Push(obj.ToString() ?? "");
        }, ["this"]);
    }

    private void NumberNaN() 
    {
        _env.Def("NaN", new NaN(), "const");
    }

    private void IsNaN() 
    {
        CreateFunction("isNaN", (stack, env) => {
            var maybeNumber = env.Get("number");
            stack.Push(maybeNumber is NaN);
        }, ["number"]);
    }

    private void ParseInt() {
        CreateFunction("parseInt", (stack, env) => {
            var digits = new List<int>();
            object strObj = env.Get("str");
            if (strObj is int alreadyInteger) {
                stack.Push(alreadyInteger);
                return;
            }
            string str = "";
            if (strObj is string myString) 
                str = myString;
            else if (strObj is char c)
                str += c;
            foreach(var c in str) 
            {
                if (char.IsDigit(c))
                    digits.Add((int)char.GetNumericValue(c));
                else
                    break;
            }
            if (digits.Count == 0) 
            {
                stack.Push(new NaN());
                return;
            }
            var maxIndex = digits.Count - 1;
            var integer = 0;
            for(var i = maxIndex; i >= 0; i--) {
                integer += digits[i] * (int)Math.Pow(10, maxIndex - i);
            }
            stack.Push(integer);
        }, ["str"]);
    }

    private void CreateFunction(string name, Action<Stack<object>, InterpreterEnvironment> action)
    {
        var fn = new AnonymousFunctionDeclaration([], new Evaluation(action));
        _env.Def(name, fn, "const");
    }

    private void CreateFunction(string name, Action<Stack<object>, InterpreterEnvironment> action, IEnumerable<string> args)
    {
        var fn = new AnonymousFunctionDeclaration(args.Select(arg => new Identifier(arg)), new Evaluation(action));
        _env.Def(name, fn, "const");
    }
}
