using Amazon.DynamoDBv2;
using Microsoft.Extensions.Hosting;

namespace ModelParserApp.Services;

public class DynamoDbInitializer(DynamoService dynamoService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dynamoService.EnsureTableExists();
        }
        catch (AmazonDynamoDBException ex) when (ex.Message.Contains("security token"))
        {
            Console.WriteLine("AWS credentials are expired or invalid.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}