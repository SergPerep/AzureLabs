
using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using ServiceBus.TopicBasics.Services;
using Shared.Models;

var config = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false) // Reads json
	.Build();

var serviceBusHostName = config["ServiceBusHostName"];
var topicName = config["TopicName"];
var booksSubscriptionName = config["BooksSubscriptionName"];
var genericSubscriptionName = config["GenericSubscriptionName"];
var credentials = new DefaultAzureCredential();

await using var sbClient = new ServiceBusClient(serviceBusHostName, credentials);

//await BuildAndSendMessagesAsync();
await ReceiveMessagesAsync();

async Task ReceiveMessagesAsync()
{
	// Receive messages from generic subscription
	var receiverService = new ReceiverService(sbClient, topicName, booksSubscriptionName);
	await receiverService.ReceiveMessagesAsync();
}

async Task BuildAndSendMessagesAsync()
{
	// Send messages of different types

	var senderService = new SenderService(sbClient, topicName);

	var genericMessages = new string[5].Select((_, i) => $"Generic topic Message {i + 1}");

	await senderService.SendMessagesAsync(genericMessages);

	var books = new List<Book>
	{
		new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
		new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
		new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
		new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", WordCount = 100000 }
	};

	await senderService.SendMessagesAsync(books);
}