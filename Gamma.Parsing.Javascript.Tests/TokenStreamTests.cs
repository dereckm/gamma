using NUnit.Framework;
using Gamma.Parsing.Javascript;

namespace Gamma.Parsing.Javascript.Tests;

public class TokenStreamTests
{

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void ShouldTokenizeSimpleAssignmentExpression()
    {
        var stream = new TokenStream(new CharacterStream("let x = 25;"));
        var t1 = stream.Next();
        Assert.AreEqual( "let", t1.Value);
        Assert.AreEqual(TokenType.Keyword, t1.Type);

        var t2 = stream.Next();
        Assert.AreEqual("x", t2.Value);
        Assert.AreEqual(TokenType.Identifier, t2.Type);

        var t3 = stream.Next();
        Assert.AreEqual( "=", t3.Value);
        Assert.AreEqual(TokenType.Operator, t3.Type);

        var t4 = stream.Next();
        Assert.AreEqual("25", t4.Value);
        Assert.AreEqual(TokenType.Number, t4.Type);
    }
}