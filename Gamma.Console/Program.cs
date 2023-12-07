
using System.Text.Json;
using Gamma.Parsing.Javascript;

var code = "if (x > 10) { y = 5; } else if (x < 0) { y = -5; } else { y = 0; }";

var parser = new Parser();
var ast = parser.Parse(code);


var json = JsonSerializer.Serialize(ast, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
