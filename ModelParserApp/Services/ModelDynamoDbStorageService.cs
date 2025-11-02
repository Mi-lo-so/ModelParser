using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using ModelParserApp.Models;

namespace ModelParserApp.Services;

/// <summary>
///     Configuration class to be used when injecting service.
///     Defines the name of the table, and is used for fetching the correct service based on table name.
/// </summary>
public record ModelDynamoDbStorageConfiguration
{
    public const string SectionName = "ModelDynamoDbStorage"; // required for IOptions when injecting service in web api
    public required string TableName { get; init; }
}

/// <summary>
///     Interface, so the storage service can be reused for e.g. S3.
///     Makes testing easier, and injecting different services with the same functionality.
/// </summary>
public interface IModelStorageService
{
    Task CreateOrReplaceModel(ModelInfo data, bool overwrite = true);
    Task<ModelInfo> Get(string modelId);
    Task<List<ModelInfo>> GetAll();
}

public class ModelDynamoDbStorageService(ModelDynamoDbStorageConfiguration configuration, IAmazonDynamoDB client)
    : IModelStorageService
{
    private readonly DynamoDBContext _context = new(client);

    public async Task CreateOrReplaceModel(ModelInfo data, bool overwrite = true)
    {
        var fetch = await Get(data.ModelId);
        if (fetch == null && !overwrite)
            throw new ArgumentException($"Model {data.ModelId} already exists, and overwrite is set to false.");

        await _context.SaveAsync(data);
    }

    public async Task<ModelInfo> Get(string modelId)
    {
        return await _context.LoadAsync<ModelInfo>(modelId); // TODO verify result in dynamoDB
    }

    public async Task<List<ModelInfo>> GetAll()
    {
        // TODO
        throw new NotImplementedException();
        //return await _context.LoadAsync<List<ModelInfo>>();
    }

    public async Task EnsureTableExists()
    {
        var tableName = configuration.TableName;

        var tables = await client.ListTablesAsync();
        if (!tables.TableNames.Contains(tableName))
        {
            var request = new CreateTableRequest
            {
                TableName = tableName,
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

            await WaitForTableToExist(tableName);
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