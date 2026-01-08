using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Queueus.Models;

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
                var bytes = Convert.FromBase64String(message.MessageText);
                var decodedMessage = Encoding.UTF8.GetString(bytes);
                var book = JsonSerializer.Deserialize<Book>(decodedMessage);
                Console.WriteLine($"Consume message {message.MessageId}");
                Console.WriteLine($"  -> Book title: {book.Name}, Author: {book.Author}, Genre: {book.Genre}, WordCount: {book.WordCount}");
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }
    }
}