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
        Assert.AreEqual("assignment_let", ast.Type);
        Assert.AreEqual("variable_declaration", ast.Left.Type);
        Assert.AreEqual("number", ast.Right.Type);

        Assert.AreEqual("x", ast.Left.As<IdentifierNode>().Name);
        Assert.AreEqual(42, ast.Right.As<LiteralNode>().Value);
    }

    [Test]
    public void Parse_ExpressionWithVariable_ReturnsBinaryExpressionNode()
    {
        var ast = RunTest<BinaryExpressionNode>("let y = x + 10;", "BinaryExpressionNode");
        Assert.AreEqual("assignment_let", ast.Type);
        Assert.AreEqual("variable_declaration", ast.Left.Type);
        Assert.AreEqual("binary", ast.Right.Type);

        Assert.AreEqual("y", ast.Left.As<IdentifierNode>().Name);
        var right = ast.Right.As<BinaryExpressionNode>();
        Assert.AreEqual(right.Left.Type, "variable");
        Assert.AreEqual(right.Operator, "+");
        Assert.AreEqual(right.Right.Type, "number");
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