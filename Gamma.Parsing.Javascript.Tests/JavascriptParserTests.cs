using NUnit.Framework;

namespace Gamma.Parsing.Javascript.Tests;

public class JavascriptParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ShouldParseFunctionCorrectly()
    {
        var parser = new JavascriptParser(new TokenStream(new CharacterStream("logConcat(a, b) { console.log(a + b) }")));
        var function = parser.ParseFunction();
        Assert.AreEqual("logConcat", function.Identifier);
        Assert.AreEqual(2, function.Parameters.Length);
        Assert.AreEqual("a", function.Parameters[0].Identifier);
        Assert.AreEqual("b", function.Parameters[1].Identifier);
    }
}