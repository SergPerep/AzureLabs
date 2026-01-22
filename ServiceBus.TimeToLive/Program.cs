using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false) // Reads json
    .Build();


var serviceBusHostName = config["ServiceBusHostName"];

var credentials = new DefaultAzureCredential();
var sbClient = new ServiceBusClient(serviceBusHostName, credentials);


// await SendMessagesAsync(5);
await ReadDeadLetterQueueAsync();

async Task ReadDeadLetterQueueAsync()
{
    var queueName = config["DLQName"];
    await using var receiver = sbClient.CreateReceiver(queueName);
    var deadLetterMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(10));
    foreach (var message in deadLetterMessages)
    {
        Console.WriteLine($"Received from DLQ: {message.Body.ToString()}");
        await receiver.CompleteMessageAsync(message);
    }
}


async Task SendMessagesAsync(int timeToLiveInSeconds)
{
    var queueName = config["QueueName"];
    await using var sender = sbClient.CreateSender(queueName);

    for (int i = 1; i <= 10; i++)
    {
        var message = new ServiceBusMessage($"Message {i}")
        {
            TimeToLive = TimeSpan.FromSeconds(timeToLiveInSeconds)
        };

        await sender.SendMessageAsync(message);
        Console.WriteLine($"Sent: Message {i} with TTL of {timeToLiveInSeconds} seconds");
    }

}