using AzuerTables.Start.Models;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Shared.Models;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false) // Reads json
    .Build();

var tableServiceUri = new Uri(config["TableServiceUri"]);
var credential = new DefaultAzureCredential();
var tableService = new TableServiceClient(tableServiceUri, credential);

var tableClient = tableService.GetTableClient(config["TableName"]);

// Write

var books = new List<Book>
{
    new Book { Name = "1984", Author = "George Orwell", Genre = "Dystopian", WordCount = 88000 },
    new Book { Name = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", WordCount = 122000 },
    new Book { Name = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", WordCount = 95000 },
    new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", WordCount = 100000 }
};

var tableEntities = books.Select(book =>
{
    var genre = string.IsNullOrWhiteSpace(book.Genre) ? "Unknown" : book.Genre;
    return new BookEntity
    {
        PartitionKey = genre,
        RowKey = new Guid().ToString(),
        Name = book.Name,
        Author = book.Author,
        Genre = book.Genre,
        WordCount = book.WordCount
    };
});

foreach (var tableEntity in tableEntities)
{
    tableClient.AddEntity(tableEntity);
}

// Read

// var entities = tableClient.Query<BookEntity>();

// foreach (var entity in entities)
// {
//     Console.WriteLine($"Name: {entity.Name}, Author: {entity.Author}, Genre: {entity.Genre}, WordCount: {entity.WordCount}");
// }

