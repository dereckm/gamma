using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Gamma.Interpreting.Javascript;
using Gamma.Parsing.Javascript;
using Gamma.Parsing.Javascript.Syntax;
using Microsoft.AspNetCore.Mvc;

namespace Gamma.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class JavascriptController : ControllerBase
{
    private readonly ILogger<JavascriptController> _logger;

    public JavascriptController(ILogger<JavascriptController> logger)
    {
        _logger = logger;
    }

    [HttpPost("parse")]
    public IActionResult Parse([FromBody] string code)
    {
        try 
        {
            var sw = Stopwatch.StartNew();
            var parser = new Parser();
            var ast = parser.Parse(Regex.Unescape(code));
            var printer = new AstPrinter();
            var result = printer.Print(ast);
            sw.Stop();
            return Ok(new ParserResponse {
                Result = result,
                ExecutionTimeMs = sw.ElapsedMilliseconds
            });
        } 
        catch (Exception ex)
        {
            _logger.LogError("Cannot parse code: {ex}", ex);
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpPost("interpret")]
    public IActionResult Interpret([FromBody] string code)
    {
        try 
        {
            var sw = Stopwatch.StartNew();
            var parser = new Parser();
            var ast = parser.Parse(Regex.Unescape(code));
            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);
            sw.Stop();
            return Ok(new ParserResponse {
                Result = result.ToString() ?? "###ERROR###",
                ExecutionTimeMs = sw.ElapsedMilliseconds
            });
        } 
        catch (Exception ex)
        {
            _logger.LogError("Cannot parse code: {ex}", ex);
             return UnprocessableEntity(ex.Message);
        }
    }

    public class ParserResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; set; } = "###ERROR###";
        [JsonPropertyName("executionTimeMs")]
        public long ExecutionTimeMs { get; set; }
    }

}
