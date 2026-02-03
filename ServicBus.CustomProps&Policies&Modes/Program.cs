using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ServicBus.CustomProps_Policies_Modes.Services;
using Shared.Models;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

string senderConnectionString = config["SenderConnectionString"];
string listenerConnectionString = config["ListenerConnectionString"];
string queueName = config["QueueName"];

// var listenerClient = new Azure.Messaging.ServiceBus.ServiceBusClient(listenerConnectionString);

// var books = new List<Book>
// {
//     new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
//     new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
//     new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
//     new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", WordCount = 100000 }
// };

// await using var senderService = new SenderService(senderConnectionString, queueName);
// await senderService.SendBooks(books);

await using var receiverService = new RecieverService(listenerConnectionString, queueName);
var receivedBooks = await receiverService.RecieveAsync();
foreach (var book in receivedBooks)
{
    Console.WriteLine($"Received book: {book.Name}, Genre: {book.Genre}, WordCount: {book.WordCount}");
}