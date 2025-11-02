using ModelParserApp.Services;

namespace ModelParserApp.Modules;

/// <summary>
///     Any checks that needs to be run when starting the web app.
/// </summary>
public static class DynamoDbWebAppExtensions
{
    // interfaces - fordi de er nemmere at teste, og selvfolgelig nemmere at instantiere paa
    // using the singleton dependency injection for the dynamotable using the table name ensures we could have multiple tables.
    // SectionName er required by IOptions. Indbygget i C#. laver til model
    // TODO check what happens if unique id is already in DB
    // reconsider modelInfo.cs dynamo specification - more generic?
    // Guard framework??
    // TODO more expertise on DynamoDB
    // TODO after removing thr DynamoDbInitializer, I can remove the dependency on dot net dependency in ModelParseApp
    public static void InitializeDynamoDb(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dynamoServices = scope.ServiceProvider.GetRequiredService<ModelDynamoDbStorageService>();
            dynamoServices.EnsureTableExists().Wait();
        }
    }
}