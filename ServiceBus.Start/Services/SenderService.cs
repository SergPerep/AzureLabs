using Azure.Messaging.ServiceBus;

namespace ServiceBus.Start.Services;

public class SenderService : IAsyncDisposable
{
    ServiceBusSender _sender;
    public SenderService(ServiceBusClient sbClient, string queueName)
    {
        _sender = sbClient.CreateSender(queueName);

    }
    
    public async Task SendMessagesInBatch(IEnumerable<string> messages)
    {
        using var messageBatch = await _sender.CreateMessageBatchAsync();

        foreach (var message in messages)
        {
            // Try to fit message into the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
            {
                throw new Exception($"The message \"{message}\" is too large to fit in the batch.");
            }
        }

        // Send batch
        await _sender.SendMessagesAsync(messageBatch);
        Console.WriteLine($"A Batch of {messages.Count()} has been published");
    }

    public ValueTask DisposeAsync()
    {
        return _sender.DisposeAsync();
    }
}