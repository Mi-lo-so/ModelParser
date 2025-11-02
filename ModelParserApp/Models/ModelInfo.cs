using Amazon.DynamoDBv2.DataModel;

namespace ModelParserApp.Models;

[DynamoDBTable("LxfmlModels")]
public class ModelInfo
{
    [DynamoDBHashKey] // Partition key for the database (main id)
    public string ModelId { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty; // empty for now
    public int TotalBricks { get; set; }
    public int TotalParts { get; set; }

    [DynamoDBProperty] public List<MaterialInfo> Materials { get; set; } = new();

    [DynamoDBProperty] public List<BrickInfo> Bricks { get; set; } = new();

    public static ModelInfo From(List<BrickInfo> bricks, string name, string? description)
    {
        return new ModelInfo
        {
            Name = name,
            Description = description,
            TotalBricks = bricks.Count,
            TotalParts = bricks.Sum(b => b.Parts.Count),
            Materials = bricks
                .SelectMany(b => b.Parts)
                .SelectMany(p => p.Materials)
                .Distinct()
                .ToList()
        };
    }
}