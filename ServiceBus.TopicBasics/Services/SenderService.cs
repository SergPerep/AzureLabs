using System.Text.Json;
using Azure.Messaging.ServiceBus;

public class SenderService : IAsyncDisposable
{
    private readonly ServiceBusSender _sender;
    public SenderService(ServiceBusClient sbClient, string topicName)
    {
        _sender = sbClient.CreateSender(topicName);
    }

    public async Task SendMessagesAsync<T>(IEnumerable<T> messages)
    {
        foreach (var textMessage in messages)
        {
            var msg = JsonSerializer.Serialize(textMessage);
            var serviceBusMessage = new ServiceBusMessage(msg);
            await _sender.SendMessageAsync(serviceBusMessage);
            Console.WriteLine($"Sent message: {msg}");
        }
    }

    public ValueTask DisposeAsync()
    {
        return _sender.DisposeAsync();
    }
}