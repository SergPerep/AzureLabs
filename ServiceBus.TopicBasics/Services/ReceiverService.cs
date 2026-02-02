
using Azure.Messaging.ServiceBus;

namespace ServiceBus.TopicBasics.Services;



public class ReceiverService : IAsyncDisposable
{
    private readonly ServiceBusReceiver _receiver;
    public ReceiverService(ServiceBusClient sbClient, string topicName, string subscriptionName)
    {
        _receiver = sbClient.CreateReceiver(topicName, subscriptionName);
    }

    public async Task ReceiveMessagesAsync()
    {
        var receivedMessages = await _receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));
        foreach (var message in receivedMessages)
        {
            Console.WriteLine($"Received message: {message.Body}");
            await _receiver.CompleteMessageAsync(message);
        }
    }
    public ValueTask DisposeAsync()
    {
        return _receiver.DisposeAsync();
    }
}