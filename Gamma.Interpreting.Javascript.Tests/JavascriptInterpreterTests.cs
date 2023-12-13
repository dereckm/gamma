using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Interpreting.Javascript.Tests;

    [TestFixture]
    public class JavascriptInterpreterTests
    {
        [Test]
        public void TestEvaluateSimpleExpression()
        {
            var code = "1 + 2;";
            var ast = RunTest(code, "BinaryExpressionNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void TestEvaluateVariableDeclaration()
        {
            var code = "let x = 5;";
            var ast = RunTest(code, "VariableDeclarationNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(5, Is.EqualTo(result));
        }

        [Test]
        public void TestEvaluateFunctionCall()
        {
            var code = "function add(a, b) { return a + b; } add(3, 4);";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(7, Is.EqualTo(result));
        }

        [Test]
        public void TestEvaluateFunctionCallEarlyReturn()
        {
            var code = """
            let x = 0;
            function add(a, b) { 
                if (a > 2) return 5;
                x = 10;
                return a + b; 
            }
            add(3, 4);
            x;
            """;
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(0, Is.EqualTo(result));
        }

        [Test]
        public void TestEvaluateForLoop()
        {
            var code = "let sum = 0; for (let i = 1; i <= 5; i++) { sum += i; } sum;";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(15, Is.EqualTo(result));
        }

        [Test]
        public void TestEvaluateIfElseStatement()
        {
            var code = "let x = 10; let result = 'a'; if (x > 5) { result = 'greater'; } else { result = 'less or equal'; } result;";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That("greater",  Is.EqualTo(result));
        }

        [Test]
        public void TestTestEvaluateArrayDeclaration()
        {
            var code = "let myArray = [1, 2, 3]; myArray;";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 1, 2, 3 }));
        }

        [Test]
        public void TestEvaluateArrayAccess()
        {
            var code = "let myArray = [1, 2, 3]; myArray[1];";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void TestEvaluateArrayModification()
        {
            var code = "let myArray = [1, 2, 3]; myArray[1] = 10; myArray;";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 1, 10, 3 }));
        }

        [Test]
        public void TestEvaluateArrayLength()
        {
            var code = "let myArray = [1, 2, 3]; myArray.length;";
            var ast = RunTest(code, "ProgramNode");

            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void NestedFunctionCallAndForLoop()
        {
            string code = """
                    let myArray = [1, 2, 3]; 
                    myArray[1] = 10;
                    function pow2(n) {
                        return n * n;
                    }
                    function sumArray(arr) {
                        let sum = 0;
                        for(var i = 0; i < arr.length; i++)
                        {
                            sum += pow2(arr[i]);
                        }
                        return sum;
                    }
                    sumArray(myArray);
                    """;
            var ast = RunTest(code, "ProgramNode");
            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);
            Assert.That(110, Is.EqualTo(result));
        }

        private static AstNode RunTest(string code, string expectedNodeType)
        {
            var parser = new Parser();
            var ast = parser.Parse(code);
            Assert.That(expectedNodeType, Is.EqualTo(ast.GetType().Name));
            return ast;
        }

        private static T RunTest<T>(string code, string expectedNodeType) where T : AstNode
        {
            var ast = RunTest(code, expectedNodeType);
            return (T)ast;
        }

        // Add more tests for different types of expressions and statements
    }