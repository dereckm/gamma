using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;
public class JavascriptInterpreter
{
    private Parser _parser = new();

    public object Evaluate(AstNode ast)
    {
        var env = new InterpreterEnvironment();
        Prepare(env);
        var evaluator = new Evaluator(env);
        var result = evaluator.Evaluate(ast);
        return result;
    }

    internal void Prepare(InterpreterEnvironment env)
    {
        // var parseInt = _parser.Parse("""
        //     function parseInt(str) {
        //         const digits = []
        //         for(const c of str) {
        //             if (c === '0') digits.push(0);
        //             else if (c === '1') digits.push(1)
        //         }
        //     }
        // """);

        // env.Def(nameof(parseInt), parseInt, "const");
    }
}
