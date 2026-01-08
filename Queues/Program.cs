using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Shared.Models;
using Queueus.Services;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var storageConnectionString = config["AzureStorageAccount"];

var queueClient = new QueueClient(storageConnectionString, "orders");
queueClient.CreateIfNotExists();

// Publish
// var publisher = new Publisher(queueClient);
// var books = new List<Book>
// {
//     new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
//     new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
//     new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
//     new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", WordCount = 100000 }
// };

// await publisher.PublishAsync(books.Select(b => JsonSerializer.Serialize(b)));

// Consume
var consumer = new Consumer(queueClient);
await consumer.ConsumeAsync();
