using Clone.Models.Dtos;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace Clone.Worker.Services;

public class OrderCreatedConsumer : IEventHandler
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private readonly IMongoClient _mongoClient;

    public string QueueName => "order.created.queue";
    public string RoutingKey => "order.created";
    public string Exchange => "orders";

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IMongoClient mongoClient)
    {
        _logger = logger;
        _mongoClient = mongoClient;
    }

    public async Task HandleAsync(string message)
    {
        var createOrder = JsonSerializer.Deserialize<WhatsAppWebhookDto>(message);

        if (createOrder is null)
        {
            _logger.LogError("Failed to deserialize OrderCreated message: {Message}", message);
            return;
        }

        var database = _mongoClient.GetDatabase("WhatsAppWebhookDb");
        var orders = database.GetCollection<WhatsAppWebhookDto>("Orders");
        await orders.InsertOneAsync(createOrder);

        Console.WriteLine($"Order created: {message}");
    }
}
