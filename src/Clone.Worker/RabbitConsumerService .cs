using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
namespace Clone.Worker;
public class RabbitConsumerService : BackgroundService
{
    private readonly RabbitDispatcher _dispatcher;
    private readonly IConnection _connection;

    public RabbitConsumerService(
        RabbitDispatcher dispatcher,
        IConnection connection)
    {
        _dispatcher = dispatcher;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _dispatcher.Start(_connection);

        Console.WriteLine("Consumer started...");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
