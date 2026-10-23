using Clone.Worker;
using Clone.Worker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using MongoDB.Driver;

Console.WriteLine("Start Worker");
var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection") ?? 
    throw new InvalidOperationException("Connection MongoDbConnection string not found.");

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(connectionString));

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("WhatsAppWebhookDb");
}); 

builder.Services.AddSingleton<IEventHandler, OrderCreatedConsumer>();
builder.Services.AddSingleton<IEventHandler, OrderUpdatedConsumer>();

builder.Services.AddSingleton<RabbitDispatcher>();

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = "host.docker.internal"
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});


builder.Services.AddHostedService<RabbitConsumerService>();

var host = builder.Build();

host.Run();