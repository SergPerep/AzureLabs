using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Shared.Models;

namespace EventHub.Processor.Services;

public class ProcessorService : IAsyncDisposable
{
    CancellationTokenSource cancellationSource;
    EventProcessorClient processor;

    public ProcessorService(string blobContainerEndpoint, string consumerGroup, string readerConnectionString, string eventHubName, int stopAfterSeconds = 5)
    {
        cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(stopAfterSeconds));
        var credential = new DefaultAzureCredential();

        var blobContainerClient = new BlobContainerClient(
            new Uri(blobContainerEndpoint),
            credential);

        processor = new EventProcessorClient(
            checkpointStore: blobContainerClient,
            consumerGroup: consumerGroup,
            connectionString: readerConnectionString,
            eventHubName: eventHubName);

        processor.ProcessEventAsync += processEventHandler;
        processor.ProcessErrorAsync += processErrorHandler;
    }

    public async Task RunAsync()
    {

        await processor.StartProcessingAsync();

        // Wait for the processor to finish processing (which will happen when the cancellation token is triggered)
        try
        {
            // The processor performs its work in the background; block until cancellation
            // to allow processing to take place.

            await Task.Delay(Timeout.Infinite, cancellationSource.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Processing stopped.");
        }
    }

    private async Task processEventHandler(ProcessEventArgs eventArgs)
    {
        try
        {
            var book = eventArgs.Data.EventBody.ToObjectFromJson<Book>();
            Console.WriteLine($"Received: {book.Name}, Genre: {book.Genre} from partition: {eventArgs.Partition.PartitionId}");
            await eventArgs.UpdateCheckpointAsync(); // On Blob storage
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing event: {ex.Message}");
        }
    }

    private async Task processErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        try
        {
            Console.WriteLine($"Error in partition: {eventArgs.PartitionId}, Operation: {eventArgs.Operation}, Exception: {eventArgs.Exception.Message}");
        }
        catch (Exception ex)
        {
            // Handle the exception from handler code
            Console.WriteLine($"Error processing error event: {ex.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (processor != null)
            {
                await processor.StopProcessingAsync();
                processor.ProcessEventAsync -= processEventHandler;
                processor.ProcessErrorAsync -= processErrorHandler;
            }
        }
        finally
        {
            cancellationSource?.Dispose();
        }
    }
}