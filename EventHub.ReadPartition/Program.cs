using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Shared.Models;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false) // Reads json
    .Build();


// await BuildAndSendEvents();
await ReadFromPartition();

async Task BuildAndSendEvents()
{
var senderPolicyString = config["SenderPloictyString"];
    var producer = new EventHubProducerClient(senderPolicyString);

    var books = new List<Book>
    {
		// Dystopian
		new() { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
        new() { Name = "Brave New World", Author = "Aldous Huxley", Genre = "Dystopian", WordCount = 64000 },
        new() { Name = "Fahrenheit 451", Author = "Ray Bradbury", Genre = "Dystopian", WordCount = 46000 },
        new() { Name = "The Handmaid's Tale", Author = "Margaret Atwood", Genre = "Dystopian", WordCount = 90000 },

		// Romance
		new() { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
        new() { Name = "Jane Eyre", Author = "Charlotte Brontë", Genre = "Romance", WordCount = 183000 },
        new() { Name = "Wuthering Heights", Author = "Emily Brontë", Genre = "Romance", WordCount = 107000 },
        new() { Name = "Sense and Sensibility", Author = "Jane Austen", Genre = "Romance", WordCount = 119000 },

		// Fantasy
		new() { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
        new() { Name = "The Fellowship of the Ring", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 187000 },
        new() { Name = "A Game of Thrones", Author = "George R.R. Martin", Genre = "Fantasy", WordCount = 298000 },
        new() { Name = "The Name of the Wind", Author = "Patrick Rothfuss", Genre = "Fantasy", WordCount = 250000 }
    };

    foreach (var book in books)
    {
        var jsonString = JsonSerializer.Serialize(book);
        var eventData = new EventData(jsonString);
        await producer.SendAsync(new[] { eventData }, new SendEventOptions { PartitionKey = book.Genre });
        Console.WriteLine($"Sent: {book.Name}");
    }
}

async Task ReadFromPartition()
{
    var readerPolicyString = config["ReaderPolicyString"];
    var consumer = new EventHubConsumerClient("$Default", readerPolicyString);
    var partitionId = 3; // Appears to be for Fantasy partition

    var cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));

    await foreach (var partitionEvent in consumer.ReadEventsFromPartitionAsync(partitionId.ToString(), EventPosition.Earliest, cancellationToken: cancellationSource.Token))
    {
        var jsonString = partitionEvent.Data.EventBody.ToString();
        var book = JsonSerializer.Deserialize<Book>(jsonString);
        Console.WriteLine($"Received from Partition {partitionId}: {book.Name}, Genre: {book.Genre}");
    }
}