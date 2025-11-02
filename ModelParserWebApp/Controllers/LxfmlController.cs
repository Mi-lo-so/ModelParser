using Microsoft.AspNetCore.Mvc;
using ModelParserApp.Models;
using ModelParserApp.Services;

namespace ModelParserWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LxfmlController : ControllerBase
{
    [HttpGet("{modelId}")]
    public async Task<IActionResult> GetModel(string modelId, [FromServices] IModelStorageService storageService)
    {
        var result = await storageService.Get(modelId);

        if (result == null)
            return NotFound($"Model with ID {modelId} not found.");

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetModels([FromServices] IModelStorageService storageService)
    {
        var result = await storageService.GetAll();

        if (result == null)
            return NotFound("Could not fetch all models.");

        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file,
        [FromServices] IModelStorageService storageService)
    {
        Console.WriteLine("Called upload api");
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var fileName = file.FileName;

        List<BrickInfo> bricks;
        using (var stream = file.OpenReadStream())
        {
            bricks = LxfmlParser.Parse(stream);
        }


        var modelData = ModelInfo.From(bricks, fileName, "");
        await storageService.CreateOrReplaceModel(modelData);

        return Ok(modelData);
    }
}