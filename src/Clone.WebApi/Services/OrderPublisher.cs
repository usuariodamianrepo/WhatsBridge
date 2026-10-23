using Clone.Models;
using Clone.Models.Dtos;

namespace Clone.WebApi.Services
{
    public class OrderPublisher
    {
        private const string EXCHANGE = "orders";
        private readonly IEventPublisher _publisher;

        public OrderPublisher(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task OrderCreated(WhatsAppWebhookDto e)
            => _publisher.PublishAsync(EXCHANGE, "order.created", e);

        public Task OrderUpdated(WhatsAppWebhookDto e)
            => _publisher.PublishAsync(EXCHANGE, "order.updated", e);
    }
}
