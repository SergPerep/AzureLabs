using Azure.Storage.Blobs;
using Azure.Storage.Blobs.ChangeFeed;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var connectionString = config["StorageAccountConnectionString"];

var blobServiceClient = new BlobServiceClient(connectionString);

var changeFeedClient = blobServiceClient.GetChangeFeedClient();

await foreach (var change in changeFeedClient.GetChangesAsync())
{
    Console.WriteLine($"Subject: {change.Subject}");
    Console.WriteLine($"Event type: {change.EventType}");
    Console.WriteLine($"Operation name: {change.EventData.BlobOperationName}");
    Console.WriteLine(new string('-', 50));
}