
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Shared.Models;

namespace ServicBus.CustomProps_Policies_Modes.Services;

public class RecieverService : IAsyncDisposable
{
    private readonly ServiceBusClient _sbClient;
    private readonly ServiceBusReceiver _listener;

    public RecieverService(string listenerConnectionString, string queueName)
    {
        _sbClient = new ServiceBusClient(listenerConnectionString);
        _listener = _sbClient.CreateReceiver(queueName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        });
    }

    public async Task<Book[]> RecieveAsync()
    {
        if(!(await _listener.PeekMessagesAsync(10)).Any()) return [];

        var messages = await _listener.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));
        return messages.Select(m => JsonSerializer.Deserialize<Book>(m.Body.ToString())).OfType<Book>().ToArray();
    }
    public async ValueTask DisposeAsync()
    {
        await _sbClient.DisposeAsync();
        await _listener.DisposeAsync();
    }
}