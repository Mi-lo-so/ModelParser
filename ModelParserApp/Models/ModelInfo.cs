using Amazon.DynamoDBv2.DataModel;

namespace ModelParserApp.Models;

[DynamoDBTable("LxfmlModels")]
public class ModelInfo
{
    [DynamoDBHashKey] // Partition key for the database (main id)
    public string ModelId { get; set; } = Guid.NewGuid().ToString();

    public int TotalBricks { get; set; }
    public int TotalParts { get; set; }

    [DynamoDBProperty] public List<MaterialInfo> Materials { get; set; } = new();

    [DynamoDBProperty] public List<BrickInfo> Bricks { get; set; } = new();
}