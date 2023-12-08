
using Gamma.Interpreting.Javascript;
using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;

var code = "let x = 11; let y = 0; if (x > 10) { y = 5; } else if (x < 0) { y = 5; } else { y = 0; } y;";

var forLoop = "let y = 0; for(let i = 0; i < 10; i++) { y += 2 } y;";

var parser = new Parser();
var ast = parser.Parse(code);

// var printer = new AstPrinter();
// printer.Print(ast);

var interpreter = new JavascriptInterpreter();
var result = interpreter.Evaluate(ast);
Console.WriteLine(result);


