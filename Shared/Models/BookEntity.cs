namespace Shared.Models;

using Azure;
using Azure.Data.Tables;

public class BookEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Author { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public int WordCount { get; set; }
    public DateTimeOffset? Timestamp { get; set;}
    public ETag ETag { get; set; }
}