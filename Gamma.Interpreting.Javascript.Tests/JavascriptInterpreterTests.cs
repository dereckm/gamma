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

        [TestFixture]
        public class Arrays 
        {
            [Test]
            public void TestEvaluateArrayDeclaration()
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
            public void TestEvaluateArrayPush()
            {  
                var code = """
                    let arr = [1, 2, 3];
                    arr.push(4);
                    arr;
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 1, 2, 3, 4 }));
            }

            [Test]
            public void TestEvaluateArrayPop()
            {  
                var code = """
                    let arr = [1, 2, 3];
                    arr.pop();
                    arr;
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 1, 2 }));
            }

            [Test]
            public void TestEvaluateArraySomeTrue()
            {  
                var code = """
                    let arr = [1, 2, 3];
                    arr.some((n) => n % 2 === 0);
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((bool)result, Is.True);
            }

            [Test]
            public void TestEvaluateArraySomeFalse()
            {  
                var code = """
                    let arr = [1, 3, 5];
                    arr.some((n) => n % 2 === 0);
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((bool)result, Is.False);
            }

            [Test]
            public void TestEvaluateArrayMap()
            {
                var code = """
                    let arr = [1, 2, 3];
                    arr.map((n) => n * 2);
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 2, 4, 6 }));
            }

            [Test]
            public void TestEvaluateArrayReverse()
            {
                var code = """
                    let arr = [1, 2, 3];
                    arr.reverse();
                    arr;
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 3, 2, 1 }));
            }

            [Test]
            public void TestEvaluateArraySlice()
            {
                var code = """
                    let arr = [1, 2, 3];
                    arr.shift();
                    arr;
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That((List<object>)result, Is.EquivalentTo(new object[] { 2, 3 }));
            }

            [Test]
            public void TestEvaluateArrayIterator()
            {
                var code = """
                    let arr = [1, 2, 3];
                    let sum = 0;
                    for(const n of arr) {
                        sum += n;
                    }
                    sum;
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That(result, Is.EqualTo(6));
            }
        }

        public class Strings 
        {
            [Test]
            public void TestEvaluateStringLength()
            {
                var code = """
                    let str = 'hello world';
                    str.length;
                """;    
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That(result, Is.EqualTo(11));
            }

            [Test]
            public void TestEvaluateStringSplit()
            {
                var code = """
                    let str = 'hello world';
                    let x = str.split(' ');
                """;
                var ast = RunTest(code, "ProgramNode");

                var interpreter = new JavascriptInterpreter();
                var result = interpreter.Evaluate(ast);
                Assert.That(result, Is.EquivalentTo(new List<object> { "hello", "world" }));
            }
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

        [Test]
        public void PreventConstantRedefinition()
        {
            string code = """
                const x = 5;
                x = 3;
            """;
            var ast = RunTest(code, "ProgramNode");
            var interpreter = new JavascriptInterpreter();
            var exception = Assert.Throws<Exception>(() => interpreter.Evaluate(ast));
           Assert.That(exception!.Message, Is.EqualTo("Illegal assignment on const variable: \"x\""));
        }

        [Test]
        public void PreventFunctionRedefinition()
        {
            string code = """
                function a() { return 1; }
                function a() { return 3; }
            """;
            var ast = RunTest(code, "ProgramNode");
            var interpreter = new JavascriptInterpreter();
            var exception = Assert.Throws<Exception>(() => interpreter.Evaluate(ast));
           Assert.That(exception!.Message, Is.EqualTo("Already defined in scope: \"a\""));
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