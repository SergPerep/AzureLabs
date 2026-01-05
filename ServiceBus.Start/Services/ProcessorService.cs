using System.Timers;
using Azure.Messaging.ServiceBus;

namespace ServiceBus.Start.Services;

public class ProcessorService : IAsyncDisposable
{
    private ServiceBusProcessor _processor;
    private const int idleTimeoutMs = 3000;
    private System.Timers.Timer _idleTimer;
    public ProcessorService(ServiceBusClient sbClient, string queueName)
    {
        _processor = sbClient.CreateProcessor(queueName);
        _idleTimer = new(idleTimeoutMs);

        // Add TimerElapse
        _idleTimer.Elapsed += TimerElapsedAsync;

        // Add MessageHandler
        _processor.ProcessMessageAsync += MessageHandlerAsync;

        // Add ErrorHandler
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    public async Task ProcessAsync()
    {
        await _processor.StartProcessingAsync();
        Console.WriteLine("Processor started");

        while (_processor.IsProcessing)
        {
            await Task.Delay(500);
        }

        _idleTimer.Stop();
        Console.WriteLine("Stopped receiving messages");
    }

    public ValueTask DisposeAsync()
    {
        return _processor.DisposeAsync();
    }

    private async void TimerElapsedAsync(object? s, ElapsedEventArgs e)
    {
        Console.WriteLine($"No messages received for {idleTimeoutMs / 1000} seconds. Stopping processor...");
        await _processor.StopProcessingAsync();
    }

    private async Task MessageHandlerAsync(ProcessMessageEventArgs args)
    {
        var messageBody = args.Message.Body.ToString();
        Console.WriteLine($"Received: \"{messageBody}\"");

        _idleTimer.Stop();
        _idleTimer.Start();

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}