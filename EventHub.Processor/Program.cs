using EventHub.Processor.Services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var blobContainerEndpoint = config["BlobContainerEndpoint"];
var consumerGroup = config["ConsumerGroup"];
var readerConnectionString = config["ReaderConnectionString"];
var eventHubName = config["EventHubName"];

await using var processorService = new ProcessorService(
    blobContainerEndpoint: blobContainerEndpoint,
    consumerGroup: consumerGroup,
    readerConnectionString: readerConnectionString,
    eventHubName: eventHubName,
    stopAfterSeconds: 7);

await processorService.RunAsync();