using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ModelParserApp.Models;

namespace ModelParserApp.Services;

public class DynamoService(IAmazonDynamoDB client)
{
    private const string TableName = "LxfmlModels";
    private readonly DynamoDBContext _context = new(client);

    public async Task CreateInDynamoDbAsync(string tableName, string modelId, ModelInfo data)
    {
        await _context.SaveAsync(data);
    }

    public async Task<ModelInfo> ReadFromDynamoDbAsync(string modelId)
    {
        return await _context.LoadAsync<ModelInfo>(modelId);
    }

    public async Task EnsureTableExists()
    {
        var tables = await client.ListTablesAsync();
        if (!tables.TableNames.Contains(TableName))
        {
            var request = new CreateTableRequest
            {
                TableName = TableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new()
                    {
                        AttributeName = "ModelId",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new()
                    {
                        AttributeName = "ModelId",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await client.CreateTableAsync(request);

            await WaitForTableToExist(TableName);
        }
    }

    private async Task WaitForTableToExist(string tableName)
    {
        string status;
        do
        {
            await Task.Delay(2000);
            var response = await client.DescribeTableAsync(tableName);
            status = response.Table.TableStatus;
        } while (status != TableStatus.ACTIVE);
    }
}