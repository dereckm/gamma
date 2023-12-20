namespace Gamma.Interpreting.Javascript.Tests;

[TestFixture]
public class InterpreterEnvironmentTests
{
    [Test]
    public void ShouldDefineVariableCorrectly()
    {
        var env = new InterpreterEnvironment();
        env.Def("x", 1, "const");

        var value = env.Get("x");
        Assert.That(value, Is.EqualTo(1));
    }

    [Test]
    public void ShouldPreventConstReassignment()
    {
        var env = new InterpreterEnvironment();
        env.Def("x", 1, "const");

        var thrown = Assert.Throws<Exception>(() => env.Set("x", 3));
        Assert.That(thrown.Message, Is.EqualTo("Illegal assignment on const variable: \"x\""));
    }

    [Test]
    public void ShouldReassignLetCorrectly()
    {
        var env = new InterpreterEnvironment();
        env.Def("x", 1, "let");
        env.Set("x", 3);

        var value = env.Get("x");
        Assert.That(value, Is.EqualTo(3));
    }

    [Test]
    public void ShouldReadFromParentScope()
    {
        var env = new InterpreterEnvironment();
        env.Def("x", 1, "let");
        env = env.Extend();

        var value = env.Get("x");
        Assert.That(value, Is.EqualTo(1));
    }

    [Test]
    public void ShouldAllowRedefinitionInChildScope()
    {
        var env = new InterpreterEnvironment();
        env.Def("x", 1, "let");
        env = env.Extend();

        env.Def("x", 5, "const");

        var value = env.Get("x");
        Assert.That(value, Is.EqualTo(5));
    }
}