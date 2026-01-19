using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Shared.Models;

namespace ServicBus.CustomProps_Policies_Modes.Services;

public class SenderService : IAsyncDisposable
{
    private readonly ServiceBusClient _sbClient;
    private readonly ServiceBusSender _sender;
    public SenderService(string senderConnectionString, string queueName)
    {
        _sbClient = new ServiceBusClient(senderConnectionString);
        _sender = _sbClient.CreateSender(queueName);
    }

    public async Task SendBooks(IEnumerable<Book> books)
    {
        foreach (var book in books)
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(book));

            // Custom Properties
            message.ApplicationProperties.Add("Genre", book.Genre);
            message.ApplicationProperties.Add("WordCount", book.WordCount);
            await _sender.SendMessageAsync(message);
            Console.WriteLine($"Sent book: {book.Name}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _sbClient.DisposeAsync();
    }
}