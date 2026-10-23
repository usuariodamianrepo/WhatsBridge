using Clone.Models.Dtos;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace Clone.Worker.Services;

public class OrderUpdatedConsumer : IEventHandler
{
    private readonly ILogger<OrderUpdatedConsumer> _logger;

    private readonly IMongoClient _mongoClient;

    public string QueueName => "order.updated.queue";
    public string RoutingKey => "order.updated";
    public string Exchange => "orders";

    public OrderUpdatedConsumer(ILogger<OrderUpdatedConsumer> logger, IMongoClient mongoClient)
    {
        _logger = logger;
        _mongoClient = mongoClient;
    }

    public async Task HandleAsync(string message)
    {
        var updateOrder = JsonSerializer.Deserialize<WhatsAppWebhookDto>(message);

        if (updateOrder is null)
        {
            _logger.LogError("Failed to deserialize OrderUpdated message: {Message}", message);
            return;
        }

        //TODO: Implement the logic to update the order in the database using the _mongoClient.

        Console.WriteLine($"Order updated: {message}");
    }
}
