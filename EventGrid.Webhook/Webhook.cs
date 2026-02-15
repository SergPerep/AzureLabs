using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EventHub.Webhook;

public class Webhook
{
    private readonly ILogger<Webhook> _logger;

    public Webhook(ILogger<Webhook> logger)
    {
        _logger = logger;
    }

    [Function("Webhook")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var events = await BinaryData.FromStreamAsync(req.Body);

        // EventGrid sends events in batches
        EventGridEvent[] eventGridEvents = EventGridEvent.ParseMany(events);

        foreach (EventGridEvent eventGridEvent in eventGridEvents)
        {
            if (eventGridEvent.TryGetSystemEventData(out object eventData))
            {
                if (eventData is SubscriptionValidationEventData subscriptionValidationEventData)
                {
                    _logger.LogInformation("Got SubscriptionValidation event data, validation code: {ValidationCode}", subscriptionValidationEventData.ValidationCode);
                    var responseData = new
                    {
                        ValidationResponse = subscriptionValidationEventData.ValidationCode
                    };
                    return new OkObjectResult(responseData);
                }
            }
        }
        return new OkObjectResult("Reveived event successfully");
    }
}