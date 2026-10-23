
namespace Clone.Worker.Services;

public interface IEventHandler
{
    string QueueName { get; }
    string RoutingKey { get; }
    string Exchange { get; }

    Task HandleAsync(string message);
}
