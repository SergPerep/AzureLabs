using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ServiceBus.Start.Services;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var sbHostName = config["ServiceBusHostName"];
var queueName = config["QueueName"];

await using var sbClient = new ServiceBusClient(sbHostName, new DefaultAzureCredential());


// // SENDER
// await using var senderService = new SenderService(sbClient, queueName);

// var messages = new List<string>();

// for (int i = 1; i <= 3; i++)
// {
// 	messages.Add($"Message {i}");
// }

// await senderService.SendMessagesInBatch(messages);

// PROCESSOR
await using var processor = new ProcessorService(sbClient, queueName);
await processor.ProcessAsync();



