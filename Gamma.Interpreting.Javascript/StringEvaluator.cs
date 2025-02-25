using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;

internal partial class Evaluator
{
    public class StringEvaluator
    {
        private string _str;
        private MemberExpression _node;
        private Evaluator _evaluator;

        internal StringEvaluator(string str, MemberExpression node, Evaluator evaluator)
        {
            _str = str;
            _node = node;
            _evaluator = evaluator;
        }

        public void Evaluate()
        {
            switch(_node.Property)
                {
                    case Identifier identifier:
                        EvaluateAccessor(identifier);
                        break;
                    case FunctionCall:
                        EvaluateMethodCall();
                        break;
                }
        }

        private void EvaluateMethodCall()
        {
            var identifier = _node.Property.As<FunctionCall>().Identifier.Name;
            switch(identifier)
            {
                case "split":
                    EvaluateSplit();
                    break;
                case "replaceAll":
                    EvaluateReplaceAll();
                    break;
                default:
                    throw new NotImplementedException($"Method not found on type string: {identifier}");
            }
        }

        private void EvaluateReplaceAll()
        {
            var fnCall = _node.Property.As<FunctionCall>();
            var arg = fnCall.Arguments[0];
            _evaluator.Visit(arg);
            var text = (string)_evaluator._stack.Pop();
            var arg2 = fnCall.Arguments[1];
            _evaluator.Visit(arg2);
            var replacement = (string)_evaluator._stack.Pop();
            var newString = _str.Replace(text, replacement);
            _evaluator._stack.Push(newString);
        }

        private void EvaluateSplit()
        {
            var fnCall = _node.Property.As<FunctionCall>();
            var arg = fnCall.Arguments[0];
            _evaluator.Visit(arg);
            var separator = (string)_evaluator._stack.Pop();
            var results = new JavascriptArray(_str.Split(separator));
            _evaluator._stack.Push(results);
        }

        private void EvaluateAccessor(Identifier identifier)
        {
            switch(identifier.Name)
            {
                case "length":
                    _evaluator._stack.Push(_str.Length);
                    break;
                case "@@iterator":
                    _evaluator._stack.Push(_str.GetEnumerator());
                    break;
                default:
                    throw new NotImplementedException($"Member not found on type string: {identifier.Name}");
            }
        }
    }
}
