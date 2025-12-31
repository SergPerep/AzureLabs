using Azure.Storage.Queues;

namespace Queueus.Services;

public class Publisher(QueueClient queueClient)
{
    static public List<string> GenerateMessages(int count)
    {
        var messages = new List<string>();
        for (int i = 1; i <= count; i++)
        {
            messages.Add($"Order {i}");
        }
        return messages;
    }
    public async Task PublishAsync(List<string> messages)
    {
        // Send messages to queue
        if (queueClient.Exists())
        {
            foreach (var message in messages)
            {
                await queueClient.SendMessageAsync(message);
                Console.WriteLine($"Sent: {message}");
            }
        }
    }
}