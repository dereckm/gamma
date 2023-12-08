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
        Assert.That("let", Is.EqualTo(t1.Value));
        Assert.That(TokenType.Keyword, Is.EqualTo(t1.Type));

        var t2 = stream.Next();
        Assert.That("x", Is.EqualTo(t2.Value));
        Assert.That(TokenType.Identifier, Is.EqualTo(t2.Type));

        var t3 = stream.Next();
        Assert.That( "=", Is.EqualTo(t3.Value));
        Assert.That(TokenType.Operator, Is.EqualTo(t3.Type));

        var t4 = stream.Next();
        Assert.That("25", Is.EqualTo(t4.Value));
        Assert.That(TokenType.Number, Is.EqualTo(t4.Type));
    }
}