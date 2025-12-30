using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var storageConnectionString = config["AzureStorageAccount"];

var queueClient = new QueueClient(storageConnectionString, "orders");

// queueClient.CreateIfNotExists();

// Send messages to queue
if(queueClient.Exists())
{
    for (int i = 1; i <= 10; i++)
    {
        queueClient.SendMessage($"Order {i}");
        Console.WriteLine($"Sent: Order {i}");
    }
}
