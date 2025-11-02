using Microsoft.AspNetCore.Mvc;
using ModelParserApp.Models;
using ModelParserApp.Services;

namespace ModelParserWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LxfmlController([FromServices] DynamoService dynamoService) : ControllerBase
{
    [HttpGet("{modelId}")]
    public async Task<IActionResult> GetModel(string modelId)
    {
        var result = await dynamoService.ReadFromDynamoDbAsync(modelId);

        if (result == null)
            return NotFound($"Model with ID {modelId} not found.");

        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        Console.WriteLine("Called upload api");
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        List<BrickInfo> bricks;
        using (var stream = file.OpenReadStream())
        {
            bricks = LxfmlParser.Parse(stream);
        }

        var totalBricks = bricks.Count;
        var totalParts = bricks.Sum(b => b.Parts.Count);
        var allMaterials = bricks
            .SelectMany(b => b.Parts)
            .SelectMany(p => p.Materials)
            .Distinct()
            .ToList();
        var dynamoData = new ModelInfo();
        dynamoData.TotalBricks = totalBricks;
        dynamoData.TotalParts = totalParts;
        dynamoData.Materials = allMaterials;
        dynamoData.Bricks = bricks;


        var modelId = Guid.NewGuid().ToString();
        await dynamoService.CreateInDynamoDbAsync("LxfmlModels", modelId, dynamoData);

        return Ok(dynamoData);
    }
}