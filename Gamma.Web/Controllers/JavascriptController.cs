using System.Net;
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
    public IActionResult Parse([FromBody] ParserPayload payload)
    {
        try 
        {
            var parser = new Parser();
            var ast = parser.Parse(payload.Code);
            var printer = new AstPrinter();
            var result = printer.Print(ast);
            return Ok(result);
        } 
        catch (Exception ex)
        {
            _logger.LogError("Cannot parse code: {ex}", ex);
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpPost("interpret")]
    public IActionResult Interpret([FromBody] ParserPayload payload)
    {
        try 
        {
            var parser = new Parser();
            var ast = parser.Parse(payload.Code);
            var interpreter = new JavascriptInterpreter();
            var result = interpreter.Evaluate(ast);
            return Ok(result);
        } 
        catch (Exception ex)
        {
            _logger.LogError("Cannot parse code: {ex}", ex);
             return UnprocessableEntity(ex.Message);
        }
    }

    public class ParserPayload
    {
        public string Code { get; set;} = "";
    }
}
