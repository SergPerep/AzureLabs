using System;
using System.Text.Json;
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
    public void Run([QueueTrigger("orders", Connection = "StorageConnectionString")] QueueMessage message)
    {
        var book = JsonSerializer.Deserialize<Book>(message.MessageText);
        _logger.LogInformation($"Received a book: \n   -> Title: {book.Name}, Author: {book.Author}, Genre: {book.Genre}, WordCount: {book.WordCount}");
    }
}