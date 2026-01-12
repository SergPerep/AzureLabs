using System;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared.Models;

namespace AzFunction.QueueTrigger.Function;

public class QueueTrigger
{
    private readonly ILogger<QueueTrigger> _logger;

    public QueueTrigger(ILogger<QueueTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(QueueTrigger))]
    [TableOutput("books", Connection = "StorageConnectionString")]
    public ITableEntity Run([QueueTrigger("orders", Connection = "StorageConnectionString")] QueueMessage message)
    {
        var book = JsonSerializer.Deserialize<Book>(message.MessageText);
        if (book == null || string.IsNullOrWhiteSpace(book.Author))
        {
            _logger.LogError("Invalid book data: Author is null or empty. Message: {Message}", message.MessageText);
            throw new ArgumentException("Book Author cannot be null or empty.");
        }
        
        _logger.LogInformation($"Received a book: \n   -> Title: {book.Name}, Author: {book.Author}, Genre: {book.Genre}, WordCount: {book.WordCount}");

        string partitionKey = book.Author.Replace(" ", "");
        if (string.IsNullOrWhiteSpace(partitionKey))
        {
            partitionKey = "Unknown";
        }
        
        _logger.LogInformation($"Setting PartitionKey: '{partitionKey}'");

        var bookEntity = new BookEntity()
        {
            PartitionKey = partitionKey,
            RowKey = Guid.NewGuid().ToString(),
            Name = book.Name,
            Author = book.Author,
            Genre = book.Genre,
            WordCount = book.WordCount
        };
        return bookEntity;
    }
}