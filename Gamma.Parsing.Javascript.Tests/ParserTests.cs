using Gamma.Parsing.Javascript.Syntax;
using NUnit.Framework;

namespace Gamma.Parsing.Javascript.Tests;

[TestFixture]
public class ParserTests
{
    [TestFixture]
    public class Assignments
    {
        [Test]
        public void Parse_VariableAssignment_ReturnsVariableDeclarationNode()
        {
            var ast = RunTest<VariableDeclaration>("let x = 42;", nameof(VariableDeclaration));
            var declaration = ast.Declarations[0].As<BinaryExpression>();
            Assert.That("assignment", Is.EqualTo(declaration.Type));
            Assert.That("identifier", Is.EqualTo(declaration.Left.Type));
            Assert.That("number", Is.EqualTo(declaration.Right.Type));
            Assert.That("x", Is.EqualTo(declaration.Left.As<Identifier>().Name));
            Assert.That(42, Is.EqualTo(declaration.Right.As<Literal>().Value));
        }

        [Test]
        public void Parse_ExpressionWithVariable_ReturnsVariableDeclarationNode()
        {
            var ast = RunTest<VariableDeclaration>("let y = x + 10;", nameof(VariableDeclaration));
            var declaration = ast.Declarations[0].As<BinaryExpression>();
            Assert.That("assignment", Is.EqualTo(declaration.Type));
            Assert.That("identifier", Is.EqualTo(declaration.Left.Type));
            Assert.That("binary", Is.EqualTo(declaration.Right.Type));

            Assert.That("y", Is.EqualTo(declaration.Left.As<Identifier>().Name));
            var right = declaration.Right.As<BinaryExpression>();
            Assert.That("identifier", Is.EqualTo(right.Left.Type));
            Assert.That("+", Is.EqualTo(right.Operator));
            Assert.That("number", Is.EqualTo(right.Right.Type));
            var variable = right.Left.As<Identifier>();
            Assert.That("x", Is.EqualTo(variable.Name));
            var literal = right.Right.As<Literal>();
            Assert.That(10, Is.EqualTo(literal.Value));
        }

        [Test]
        public void Parse_ExpressionWithParentheses_ReturnsVariableDeclarationNode()
        {
            RunTest<VariableDeclaration>("let z = (y * 2) + x;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_SimpleVariableAssignment_ReturnsVariableDeclarationNode()
        {
            RunTest("let a = 5;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ComplexExpression_ReturnsVariableDeclarationNode()
        {
            RunTest("let b = (a * 3) + 7;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ExpressionWithDivision_ReturnsVariableDeclarationNode()
        {
            RunTest("let c = b / (a + 2);", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ExpressionWithSubtraction_ReturnsVariableDeclarationNode()
        {
            RunTest("let d = 2 * (c - b);", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ExpressionWithMultiplication_ReturnsVariableDeclarationNode()
        {
            RunTest("let e = d / (c + 1) * 4;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ExpressionWithMultipleOperators_ReturnsVariableDeclarationNode()
        {
            RunTest("let f = (e + 3) / 2;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_ExpressionWithComparisonOperator_ReturnsVariableDeclarationNode()
        {
            RunTest("let result = f >= 10;", nameof(VariableDeclaration));
        }

        [Test]
        public void Parse_Expression_RespectsPrecendence()
        {
            var result = RunTest<VariableDeclaration>("let result = 10 % 2 + 3;", nameof(VariableDeclaration));
            var binary = result.Declarations[0]
                .As<BinaryExpression>()
                .Right.As<BinaryExpression>();
            Assert.That(binary.Left.Type, Is.EqualTo("binary"));
        }
    }

    [TestFixture]
    public class Increments 
    {
        [Test]
        public void Parse_PostIncrement_ReturnsUnaryExpressionNode()
        {
            RunTest("x++;", nameof(UnaryExpression));
        }

        [Test]
        public void Parse_PreIncrement_ReturnsUnaryExpressionNode()
        {
            RunTest("++x;", nameof(UnaryExpression));
        }

        [Test]
        public void Parse_PostDecrement_ReturnsUnaryExpressionNode()
        {
            RunTest("y--;", nameof(UnaryExpression));
        }

        [Test]
        public void Parse_PreDecrement_ReturnsUnaryExpressionNode()
        {
            RunTest("--y;", nameof(UnaryExpression));
        }

        [Test]
        public void Parse_CompoundAssignment_ReturnsVariableDeclarationNode()
        {
            RunTest("z += 10;", nameof(BinaryExpression));
        }

        [Test]
        public void Parse_CompoundAssignmentWithVariable_ReturnsVariableDeclarationNode()
        {
            RunTest("a += b;", nameof(BinaryExpression));
        }
    }

    [TestFixture]
    public class IfElseStatements
    {
        [Test]
        public void Parse_IfStatementWithLiteralCondition_ReturnsIfStatementNode()
        {
            RunTest("if (true) { x = 42; }", nameof(IfStatement));
        }

        [Test]
        public void Parse_IfElseStatement_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { x = 42; } else { x = 0; }", nameof(IfStatement));
        }

        [Test]
        public void Parse_NestedIfStatements_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { if (y < 5) { z = 42; } }", nameof(IfStatement));
        }

        [Test]
        public void Parse_IfStatementWithoutBraces_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) x = 42;", nameof(IfStatement));
        }

        [Test]
        public void Parse_IfStatementWithBinaryExpressionCondition_ReturnsIfStatementNode()
        {
            RunTest("if (x + y >= 20) { z = 42; }", nameof(IfStatement));
        }

        [Test]
        public void Parse_IfElseIfElseStatement_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { y = 5; } else if (x < 0) { y = -5; } else { y = 0; }", nameof(IfStatement));
        }
    }

    [TestFixture]
    public class ForLoops
    {
        [Test]
        public void Parse_ForLoopWithLiteralCondition_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < 5; i++) { console.log(i); }", nameof(ForStatement));
        }

        [Test]
        public void Parse_ForLoopWithVariableCondition_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < x; i++) { console.log(i); }", nameof(ForStatement));
        }

        [Test]
        public void Parse_ForLoopWithMultipleStatements_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < 3; i++) { console.log(i); x = x + i; }", nameof(ForStatement));
        }

        [Test]
        public void Parse_ForLoopWithoutInitializer_ReturnsProgramtNode()
        {
            RunTest("let i = 0; for (; i < 3; i++) { console.log(i); }", nameof(Program));
        }

        [Test]
        public void Parse_ForLoopWithoutCondition_ReturnsProgramNode()
        {
            RunTest("let i = 0; for (; ; i++) { console.log(i); }", nameof(Program));
        }

        [Test]
        public void Parse_ForLoopWithoutIncrementor_ReturnsProgramNode()
        {
            RunTest("let i = 0; for (i; i < 3;) { console.log(i); }", nameof(Program));
        }

        [Test]
        public void ParseForOfLoop()
        {
            var code = """
                let x = [1, 2, 3];
                for(const a of x) {
                }
            """;

            var program = RunTest<Program>(code, nameof(Program));
        }
    }

    [TestFixture]
    public class FunctionDeclarations
    {
        [Test]
        public void TestSimpleFunctionDeclaration()
        {
            var code = "function simpleFunction() { var x; }";
            var ast = RunTest(code, nameof(NamedFunctionDeclaration));

            // Additional assertions based on your AST structure
            Assert.That("simpleFunction", Is.EqualTo(((NamedFunctionDeclaration)ast).Identifier.Name));
            Assert.That(0, Is.EqualTo(((NamedFunctionDeclaration)ast).Parameters.Count));
            Assert.That(ast.As<NamedFunctionDeclaration>().Body.Type, Is.EqualTo("variable_declaration"));
        }

        [Test]
        public void TestFunctionDeclarationWithParameters()
        {
            var code = "function functionWithParams(param1, param2) { var x; }";
            var ast = RunTest(code, nameof(NamedFunctionDeclaration));

            // Additional assertions based on your AST structure
            Assert.That("functionWithParams", Is.EqualTo(((NamedFunctionDeclaration)ast).Identifier.Name));
            Assert.That(2, Is.EqualTo(((NamedFunctionDeclaration)ast).Parameters.Count));
            Assert.That("param1", Is.EqualTo(((Identifier)((NamedFunctionDeclaration)ast).Parameters[0]).Name));
            Assert.That("param2", Is.EqualTo(((Identifier)((NamedFunctionDeclaration)ast).Parameters[1]).Name));
            Assert.That(ast.As<NamedFunctionDeclaration>().Body.Type, Is.EqualTo("variable_declaration"));
        }
    }

    [TestFixture]
    public class AnonymousFunctionDeclarations
    {
        [Test]
        public void ShouldParseSingleArgumentSingleExpressionAnonymousFunction()
        {
            var code = "let x = (n) => n * n;";
            var ast = RunTest<VariableDeclaration>(code, nameof(VariableDeclaration));
            var binaryExp = ast.Declarations[0].As<BinaryExpression>();
            var anonymousFn = binaryExp.Right.As<AnonymousFunctionDeclaration>();

            Assert.That(anonymousFn.Parameters.Count, Is.EqualTo(1));
            Assert.That(anonymousFn.Parameters[0].As<Identifier>().Name, Is.EqualTo("n"));
        }
        
        [Test]
        public void ShouldParseMultiArgumentsSingleExpressionAnonymousFunction()
        {
            var code = "let x = (a, b) => a * b;";
            var ast = RunTest<VariableDeclaration>(code, nameof(VariableDeclaration));
            var binaryExp = ast.Declarations[0].As<BinaryExpression>();
            var anonymousFn = binaryExp.Right.As<AnonymousFunctionDeclaration>();

            Assert.That(anonymousFn.Parameters.Count, Is.EqualTo(2));
            Assert.That(anonymousFn.Parameters[0].As<Identifier>().Name, Is.EqualTo("a"));
            Assert.That(anonymousFn.Parameters[1].As<Identifier>().Name, Is.EqualTo("b"));
        }

        [Test]
        public void ShouldParseSingleArgumentsNoParenthesisSingleExpressionAnonymousFunction()
        {
            var code = "let x = n => n * n;";
            var ast = RunTest<VariableDeclaration>(code, nameof(VariableDeclaration));
            var binaryExp = ast.Declarations[0].As<BinaryExpression>();
            var anonymousFn = binaryExp.Right.As<AnonymousFunctionDeclaration>();

            Assert.That(anonymousFn.Parameters.Count, Is.EqualTo(1));
            Assert.That(anonymousFn.Parameters[0].As<Identifier>().Name, Is.EqualTo("n"));
        }
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
}