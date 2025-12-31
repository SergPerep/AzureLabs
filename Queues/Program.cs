using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Queueus.Services;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var storageConnectionString = config["AzureStorageAccount"];

var queueClient = new QueueClient(storageConnectionString, "orders");
queueClient.CreateIfNotExists();

// Publish
// var publisher = new Publisher(queueClient);
// var messages = Publisher.GenerateMessages(10);
// await publisher.PublishAsync(messages);

// Consume
var consumer = new Consumer(queueClient);
await consumer.ConsumeAsync();
