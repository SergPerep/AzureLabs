using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var queueName = config["QueueName"];
var serviceBusHostName = config["ServiceBusHostName"];

var sbClient = new ServiceBusClient(serviceBusHostName, new DefaultAzureCredential());

// await SendMessagesAsync();
await ReceiveMessagesAsync();

async Task ReceiveMessagesAsync()
{
    await using var receiver = sbClient.CreateReceiver(queueName);
    var messages = await receiver.ReceiveMessagesAsync(maxMessages: 5);
    foreach (var message in messages)
    {
        // Receive but do not complete
        Console.WriteLine($"Received: {message.Body}");
    }
}

async Task SendMessagesAsync()
{
    await using var sender = sbClient.CreateSender(queueName);
    for (var i = 0; i < 5; i++)
    {
        var message = new ServiceBusMessage($"Message {i + 1}");
        await sender.SendMessageAsync(message);
        Console.WriteLine($"Sent: {message.Body}");
    }
}