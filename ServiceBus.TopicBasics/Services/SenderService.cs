using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Shared.Models;

public class SenderService : IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    public SenderService(ServiceBusClient sbClient, string topicName)
    {
        _sender = sbClient.CreateSender(topicName);
    }

    public async Task SendMessagesAsync(IEnumerable<Book> books)
    {
        foreach (var book in books)
        {
            var msg = JsonSerializer.Serialize(book);
            var serviceBusMessage = new ServiceBusMessage(msg);
            serviceBusMessage.ApplicationProperties["Genre"] = book.Genre;
            await _sender.SendMessageAsync(serviceBusMessage);
            Console.WriteLine($"Sent message: {msg}");
        }
    }

    public ValueTask DisposeAsync()
    {
        return _sender.DisposeAsync();
    }
}