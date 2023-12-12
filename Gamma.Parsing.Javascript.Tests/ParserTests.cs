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
        public void Parse_VariableAssignment_ReturnsBinaryExpressionNode()
        {
            var ast = RunTest<VariableDeclarationNode>("let x = 42;", "VariableDeclarationNode");
            var declaration = ast.Declarations[0].As<BinaryExpressionNode>();
            Assert.That("assignment", Is.EqualTo(declaration.Type));
            Assert.That("identifier", Is.EqualTo(declaration.Left.Type));
            Assert.That("number", Is.EqualTo(declaration.Right.Type));
            Assert.That("x", Is.EqualTo(declaration.Left.As<IdentifierNode>().Name));
            Assert.That(42, Is.EqualTo(declaration.Right.As<LiteralNode>().Value));
        }

        [Test]
        public void Parse_ExpressionWithVariable_ReturnsBinaryExpressionNode()
        {
            var ast = RunTest<VariableDeclarationNode>("let y = x + 10;", "VariableDeclarationNode");
            var declaration = ast.Declarations[0].As<BinaryExpressionNode>();
            Assert.That("assignment", Is.EqualTo(declaration.Type));
            Assert.That("identifier", Is.EqualTo(declaration.Left.Type));
            Assert.That("binary", Is.EqualTo(declaration.Right.Type));

            Assert.That("y", Is.EqualTo(declaration.Left.As<IdentifierNode>().Name));
            var right = declaration.Right.As<BinaryExpressionNode>();
            Assert.That("identifier", Is.EqualTo(right.Left.Type));
            Assert.That("+", Is.EqualTo(right.Operator));
            Assert.That("number", Is.EqualTo(right.Right.Type));
            var variable = right.Left.As<IdentifierNode>();
            Assert.That("x", Is.EqualTo(variable.Name));
            var literal = right.Right.As<LiteralNode>();
            Assert.That(10, Is.EqualTo(literal.Value));
        }

        [Test]
        public void Parse_ExpressionWithParentheses_ReturnsBinaryExpressionNode()
        {
            RunTest<VariableDeclarationNode>("let z = (y * 2) + x;", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_SimpleVariableAssignment_ReturnsBinaryExpressionNode()
        {
            RunTest("let a = 5;", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ComplexExpression_ReturnsBinaryExpressionNode()
        {
            RunTest("let b = (a * 3) + 7;", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ExpressionWithDivision_ReturnsBinaryExpressionNode()
        {
            RunTest("let c = b / (a + 2);", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ExpressionWithSubtraction_ReturnsBinaryExpressionNode()
        {
            RunTest("let d = 2 * (c - b);", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ExpressionWithMultiplication_ReturnsBinaryExpressionNode()
        {
            RunTest("let e = d / (c + 1) * 4;", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ExpressionWithMultipleOperators_ReturnsBinaryExpressionNode()
        {
            RunTest("let f = (e + 3) / 2;", "VariableDeclarationNode");
        }

        [Test]
        public void Parse_ExpressionWithComparisonOperator_ReturnsBinaryExpressionNode()
        {
            RunTest("let result = f >= 10;", "VariableDeclarationNode");
        }
    }

    [TestFixture]
    public class Increments 
    {
        [Test]
        public void Parse_PostIncrement_ReturnsUnaryExpressionNode()
        {
            RunTest("x++;", "UnaryExpressionNode");
        }

        [Test]
        public void Parse_PreIncrement_ReturnsUnaryExpressionNode()
        {
            RunTest("++x;", "UnaryExpressionNode");
        }

        [Test]
        public void Parse_PostDecrement_ReturnsUnaryExpressionNode()
        {
            RunTest("y--;", "UnaryExpressionNode");
        }

        [Test]
        public void Parse_PreDecrement_ReturnsUnaryExpressionNode()
        {
            RunTest("--y;", "UnaryExpressionNode");
        }

        [Test]
        public void Parse_CompoundAssignment_ReturnsBinaryExpressionNode()
        {
            RunTest("z += 10;", "BinaryExpressionNode");
        }

        [Test]
        public void Parse_CompoundAssignmentWithVariable_ReturnsBinaryExpressionNode()
        {
            RunTest("a += b;", "BinaryExpressionNode");
        }
    }

    [TestFixture]
    public class IfElseStatements
    {
        [Test]
        public void Parse_IfStatementWithLiteralCondition_ReturnsIfStatementNode()
        {
            RunTest("if (true) { x = 42; }", "IfStatementNode");
        }

        [Test]
        public void Parse_IfElseStatement_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { x = 42; } else { x = 0; }", "IfStatementNode");
        }

        [Test]
        public void Parse_NestedIfStatements_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { if (y < 5) { z = 42; } }", "IfStatementNode");
        }

        [Test]
        public void Parse_IfStatementWithoutBraces_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) x = 42;", "IfStatementNode");
        }

        [Test]
        public void Parse_IfStatementWithBinaryExpressionCondition_ReturnsIfStatementNode()
        {
            RunTest("if (x + y >= 20) { z = 42; }", "IfStatementNode");
        }

        [Test]
        public void Parse_IfElseIfElseStatement_ReturnsIfStatementNode()
        {
            RunTest("if (x > 10) { y = 5; } else if (x < 0) { y = -5; } else { y = 0; }", "IfStatementNode");
        }
    }

    [TestFixture]
    public class ForLoops
    {
        [Test]
        public void Parse_ForLoopWithLiteralCondition_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < 5; i++) { console.log(i); }", "ForStatementNode");
        }

        [Test]
        public void Parse_ForLoopWithVariableCondition_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < x; i++) { console.log(i); }", "ForStatementNode");
        }

        [Test]
        public void Parse_ForLoopWithMultipleStatements_ReturnsForStatementNode()
        {
            RunTest("for (let i = 0; i < 3; i++) { console.log(i); x = x + i; }", "ForStatementNode");
        }

        [Test]
        public void Parse_ForLoopWithoutInitializer_ReturnsProgramtNode()
        {
            RunTest("let i = 0; for (; i < 3; i++) { console.log(i); }", "ProgramNode");
        }

        [Test]
        public void Parse_ForLoopWithoutCondition_ReturnsProgramNode()
        {
            RunTest("let i = 0; for (; ; i++) { console.log(i); }", "ProgramNode");
        }

        [Test]
        public void Parse_ForLoopWithoutIncrementor_ReturnsProgramNode()
        {
            RunTest("let i = 0; for (i; i < 3;) { console.log(i); }", "ProgramNode");
        }
    }

    [TestFixture]
    public class FunctionDeclarations
    {
        [Test]
        public void TestSimpleFunctionDeclaration()
        {
            var code = "function simpleFunction() { var x; }";
            var ast = RunTest(code, "FunctionDeclarationNode");

            // Additional assertions based on your AST structure
            Assert.That("simpleFunction", Is.EqualTo(((FunctionDeclarationNode)ast).Identifier.Name));
            Assert.That(0, Is.EqualTo(((FunctionDeclarationNode)ast).Parameters.Count));
            Assert.That("identifier", Is.EqualTo(ast.As<FunctionDeclarationNode>().Body.Type));
        }

        [Test]
        public void TestFunctionDeclarationWithParameters()
        {
            var code = "function functionWithParams(param1, param2) { var x; }";
            var ast = RunTest(code, "FunctionDeclarationNode");

            // Additional assertions based on your AST structure
            Assert.That("functionWithParams", Is.EqualTo(((FunctionDeclarationNode)ast).Identifier.Name));
            Assert.That(2, Is.EqualTo(((FunctionDeclarationNode)ast).Parameters.Count));
            Assert.That("param1", Is.EqualTo(((IdentifierNode)((FunctionDeclarationNode)ast).Parameters[0]).Name));
            Assert.That("param2", Is.EqualTo(((IdentifierNode)((FunctionDeclarationNode)ast).Parameters[1]).Name));
            Assert.That("identifier", Is.EqualTo(ast.As<FunctionDeclarationNode>().Body.Type));
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