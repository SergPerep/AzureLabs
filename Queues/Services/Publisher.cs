using System.Text;
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
    public async Task PublishAsync(IEnumerable<string> messages)
    {
        // Send messages to queue
        if (queueClient.Exists())
        {
            foreach (var message in messages)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                var base64Message = Convert.ToBase64String(bytes);
                await queueClient.SendMessageAsync(base64Message);
                Console.WriteLine($"Sent: {message}");
            }
        }
    }
}