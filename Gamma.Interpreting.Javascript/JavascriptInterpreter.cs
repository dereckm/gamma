using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;
public class JavascriptInterpreter
{
    public object Evaluate(AstNode ast)
    {
        var evaluator = new Evaluator(new InterpreterEnvironment());
        var result = evaluator.Evaluate(ast);
        return result;
    }
}
