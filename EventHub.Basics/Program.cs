using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Shared.Models;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false) // Reads json
    .Build();

await SendEventsAsync();

async Task SendEventsAsync()
{
    var eventHubProducerClient = new EventHubProducerClient(config["eventHubInstanceConnStringSenderPolicy"]);

    var books = new List<Book>
{
    new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
    new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
    new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
    new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", WordCount = 100000 }
};

    using var eventBatch = await eventHubProducerClient.CreateBatchAsync();
    foreach (var book in books)
    {
        var jsonString = JsonSerializer.Serialize(book);
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        eventBatch.TryAdd(new EventData(bytes));
    }
    await eventHubProducerClient.SendAsync(eventBatch);
    Console.WriteLine("Data sent");
}