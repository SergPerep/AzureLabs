using Azure.Storage.Queues;

namespace Queueus.Services;

public class Consumer(QueueClient queueClient)
{
    public async Task ConsumeAsync()
    {
        while (queueClient.GetProperties().Value.ApproximateMessagesCountLong > 0)
        {
            var messages = await queueClient.ReceiveMessagesAsync();
            foreach (var message in messages.Value)
            {
                Console.WriteLine($"Consume message {message.MessageId}: {message.MessageText}");
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }
    }
}