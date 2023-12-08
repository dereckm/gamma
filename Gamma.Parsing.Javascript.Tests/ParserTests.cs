using Gamma.Parsing.Javascript.Syntax;
using NUnit.Framework;

namespace Gamma.Parsing.Javascript.Tests;

[TestFixture]
public class ParserTests
{
    #region Assignments
    [Test]
    public void Parse_VariableAssignment_ReturnsBinaryExpressionNode()
    {
        var ast = RunTest<BinaryExpressionNode>("let x = 42;", "BinaryExpressionNode");
        Assert.AreEqual("var_declaration_let", ast.Type);
        Assert.AreEqual("identifier", ast.Left.Type);
        Assert.AreEqual("number", ast.Right.Type);

        Assert.AreEqual("x", ast.Left.As<IdentifierNode>().Name);
        Assert.AreEqual(42, ast.Right.As<LiteralNode>().Value);
    }

    [Test]
    public void Parse_ExpressionWithVariable_ReturnsBinaryExpressionNode()
    {
        var ast = RunTest<BinaryExpressionNode>("let y = x + 10;", "BinaryExpressionNode");
        Assert.AreEqual("var_declaration_let", ast.Type);
        Assert.AreEqual("identifier", ast.Left.Type);
        Assert.AreEqual("binary", ast.Right.Type);

        Assert.AreEqual("y", ast.Left.As<IdentifierNode>().Name);
        var right = ast.Right.As<BinaryExpressionNode>();
        Assert.AreEqual("identifier", right.Left.Type);
        Assert.AreEqual("+", right.Operator);
        Assert.AreEqual("number", right.Right.Type);
        var variable = right.Left.As<IdentifierNode>();
        Assert.AreEqual("x", variable.Name);
        var literal = right.Right.As<LiteralNode>();
        Assert.AreEqual(10, literal.Value);
    }

    [Test]
    public void Parse_ExpressionWithParentheses_ReturnsBinaryExpressionNode()
    {
        RunTest<BinaryExpressionNode>("let z = (y * 2) + x;", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_SimpleVariableAssignment_ReturnsBinaryExpressionNode()
    {
        RunTest("let a = 5;", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ComplexExpression_ReturnsBinaryExpressionNode()
    {
        RunTest("let b = (a * 3) + 7;", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ExpressionWithDivision_ReturnsBinaryExpressionNode()
    {
        RunTest("let c = b / (a + 2);", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ExpressionWithSubtraction_ReturnsBinaryExpressionNode()
    {
        RunTest("let d = 2 * (c - b);", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ExpressionWithMultiplication_ReturnsBinaryExpressionNode()
    {
        RunTest("let e = d / (c + 1) * 4;", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ExpressionWithMultipleOperators_ReturnsBinaryExpressionNode()
    {
        RunTest("let f = (e + 3) / 2;", "BinaryExpressionNode");
    }

    [Test]
    public void Parse_ExpressionWithComparisonOperator_ReturnsBinaryExpressionNode()
    {
        RunTest("let result = f >= 10;", "BinaryExpressionNode");
    }

    #endregion

    #region Increments
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
    #endregion

    #region IfElseStatements
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
    #endregion

    #region ForLoops
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
    #endregion

    #region FunctionDeclarations
    [Test]
    public void TestSimpleFunctionDeclaration()
    {
        var code = "function simpleFunction() { var x; }";
        var ast = RunTest(code, "FunctionDeclarationNode");

        // Additional assertions based on your AST structure
        Assert.AreEqual("simpleFunction", ((FunctionDeclarationNode)ast).Identifier.Name);
        Assert.AreEqual(0, ((FunctionDeclarationNode)ast).Parameters.Count);
        Assert.AreEqual("identifier", ast.As<FunctionDeclarationNode>().Body.Type);
    }

    [Test]
    public void TestFunctionDeclarationWithParameters()
    {
        var code = "function functionWithParams(param1, param2) { var x; }";
        var ast = RunTest(code, "FunctionDeclarationNode");

        // Additional assertions based on your AST structure
        Assert.AreEqual("functionWithParams", ((FunctionDeclarationNode)ast).Identifier.Name);
        Assert.AreEqual(2, ((FunctionDeclarationNode)ast).Parameters.Count);
        Assert.AreEqual("param1", ((IdentifierNode)((FunctionDeclarationNode)ast).Parameters[0]).Name);
        Assert.AreEqual("param2", ((IdentifierNode)((FunctionDeclarationNode)ast).Parameters[1]).Name);
        Assert.AreEqual("identifier", ast.As<FunctionDeclarationNode>().Body.Type);
    }
    #endregion

    private static AstNode RunTest(string code, string expectedNodeType)
    {
        var parser = new Parser();
        var ast = parser.Parse(code);
        Assert.AreEqual(expectedNodeType, ast.GetType().Name);
        return ast;
    }

    private static T RunTest<T>(string code, string expectedNodeType) where T : AstNode
    {
        var ast = RunTest(code, expectedNodeType);
        return (T)ast;
    }
}