
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
var romanceSubscriptionName = config["RomanceSubscriptionName"];
var dystopianSubscriptionName = config["DystopianSubscriptionName"];
var credentials = new DefaultAzureCredential();

await using var sbClient = new ServiceBusClient(serviceBusHostName, credentials);

//await BuildAndSendMessagesAsync();
await ReceiveRomanceMessagesAsync();

async Task ReceiveRomanceMessagesAsync()
{
	// Receive messages from romance subscription
	var receiverService = new ReceiverService(sbClient, topicName, romanceSubscriptionName);
	await receiverService.ReceiveMessagesAsync();
}

async Task ReceiveDystopianMessagesAsync()
{
	// Receive messages from dystopian subscription
	var receiverService = new ReceiverService(sbClient, topicName, dystopianSubscriptionName);
	await receiverService.ReceiveMessagesAsync();
}

async Task BuildAndSendMessagesAsync()
{
	// Send messages of different types

	var senderService = new SenderService(sbClient, topicName);
	var books = new List<Book>
	{
		// Dystopian
		new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
		new Book { Name = "Brave New World", Author = "Aldous Huxley", Genre = "Dystopian", WordCount = 64000 },
		new Book { Name = "Fahrenheit 451", Author = "Ray Bradbury", Genre = "Dystopian", WordCount = 46000 },
		new Book { Name = "The Handmaid's Tale", Author = "Margaret Atwood", Genre = "Dystopian", WordCount = 90000 },

		// Romance
		new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
		new Book { Name = "Jane Eyre", Author = "Charlotte Brontë", Genre = "Romance", WordCount = 183000 },
		new Book { Name = "Wuthering Heights", Author = "Emily Brontë", Genre = "Romance", WordCount = 107000 },
		new Book { Name = "Sense and Sensibility", Author = "Jane Austen", Genre = "Romance", WordCount = 119000 },

		// Fantasy
		new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
		new Book { Name = "The Fellowship of the Ring", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 187000 },
		new Book { Name = "A Game of Thrones", Author = "George R.R. Martin", Genre = "Fantasy", WordCount = 298000 },
		new Book { Name = "The Name of the Wind", Author = "Patrick Rothfuss", Genre = "Fantasy", WordCount = 250000 }
	};

	await senderService.SendMessagesAsync(books);
}