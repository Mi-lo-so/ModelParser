using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using ModelParserApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Set which AWS profile to use
builder.Services.AddDefaultAWSOptions(new AWSOptions
{
    Profile = "personal", // This is the name I've given to my personal account AWS configuration. Feel free to change.
    Region = RegionEndpoint.EUWest1
});


// Add services
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<DynamoService>();
builder.Services.AddHostedService<DynamoDbInitializer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.Run();