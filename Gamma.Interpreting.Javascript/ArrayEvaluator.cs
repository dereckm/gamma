using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript;

internal partial class Evaluator
{
    private class ArrayEvaluator
    {
        private JavascriptArray _list;
        private MemberExpression _node;
        private Evaluator _evaluator;

        internal ArrayEvaluator(JavascriptArray list, MemberExpression node, Evaluator evaluator)
        {
            _list = list;
            _node = node;
            _evaluator = evaluator;
        }

        public void Evaluate()
        {
            switch(_node.Property)
            {
                case IdentifierNode identifier:
                    EvaluateAccessor(identifier);
                    break;
                case FunctionCallNode:
                    EvaluateMethodCall();
                    break;
            }
        }

        private void EvaluateMethodCall()
        {
            var identifier = _node.Property.As<FunctionCallNode>().Identifier.Name;
            switch (identifier)
            {
                case "push":
                    EvaluatePush();
                    break;
                case "pop":
                    EvaluatePop();
                    break;
                case "some":
                    EvaluateSome();
                    break;
                case "map":
                    EvaluateMap();
                    break;
                case "reverse":
                    EvaluateReverse();
                    break;
                case "shift":
                    EvaluateShift();
                    break;
                default:
                    throw new NotImplementedException($"Member doesn't exist on array ([]), Member={identifier}");
            }
        }

        private void EvaluateShift()
        {
            var slicedItem = _list[0];
            _list.RemoveAt(0);
            _evaluator._stack.Push(slicedItem);
        }

        private void EvaluateReverse()
        {
            _list.Reverse();
            _evaluator._stack.Push(_list);
        }

        private void EvaluatePush()
        {
            var fnCall = _node.Property.As<FunctionCallNode>();
            var arg = fnCall.Arguments[0];
            _evaluator.Visit(arg);
            var item = _evaluator._stack.Pop();
            _list.Add(item);
            _evaluator._stack.Push(_list.Count);
        }

        private void EvaluatePop()
        {
            var poppedItem = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            _evaluator._stack.Push(poppedItem);
        }

        private void EvaluateSome()
        {
            var fnCall = _node.Property.As<FunctionCallNode>();
            var predicate = fnCall.Arguments[0];
            var tempVar = Guid.NewGuid().ToString();
            _evaluator._env!.Def(tempVar, predicate, "const");
            foreach(var item in _list)
            {
                var arg = new LiteralNode("object", item);
                var predicateCall = new FunctionCallNode("anonymous_fn_call", new IdentifierNode(tempVar), new [] { arg });
                _evaluator.Visit(predicateCall);
                var result = (bool)_evaluator._stack.Pop();
                if (result) 
                {
                    _evaluator._stack.Push(true);
                    return;
                }
            }
            _evaluator._stack.Push(false);
        }

        private void EvaluateMap()
        {
            var fnCall = _node.Property.As<FunctionCallNode>();
            var callback = fnCall.Arguments[0];
            var tempVar = Guid.NewGuid().ToString();
            _evaluator._env!.Def(tempVar, callback, "const");
            var mappedValues = new JavascriptArray();
            for(var i = 0; i < _list.Count; i++)
            {
                var item = _list[i];
                var arg = new LiteralNode("object", item);
                var predicateCall = new FunctionCallNode("anonymous_fn_call", new IdentifierNode(tempVar), new [] { arg });
                _evaluator.Visit(predicateCall);
                var mappedValue = _evaluator._stack.Pop();
                mappedValues.Add(mappedValue);
            }
            _evaluator._stack.Push(mappedValues);
        }

        private void EvaluateAccessor(IdentifierNode identifier)
        {
            switch (identifier.Name)
            {
                case "length":
                    _evaluator._stack.Push(_list.Count);
                    break;
                case "@@iterator":
                    _evaluator._stack.Push(_list.GetEnumerator());
                    break;
                default:
                    throw new NotImplementedException($"Member doesn't exist on array ([]), Member={identifier.Name}");
            }
        }
    }
}

