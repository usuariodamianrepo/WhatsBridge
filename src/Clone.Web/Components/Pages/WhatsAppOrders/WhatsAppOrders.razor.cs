using Clone.Models.Dtos;
using MongoDB.Driver;

namespace Clone.Web.Components.Pages.WhatsAppOrders
{
    public partial class WhatsAppOrders
    {
        private readonly IMongoClient _mongoClient;
        private IQueryable<WhatsAppWebhookDto>? orders = default;

        public WhatsAppOrders(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        protected override async Task OnInitializedAsync()
        {
            var database = _mongoClient.GetDatabase("WhatsAppWebhookDb");
            var collection = database.GetCollection<WhatsAppWebhookDto>("Orders");

            var documents = await collection
                .Find(Builders<WhatsAppWebhookDto>.Filter.Empty)
                .SortByDescending(o => o.InsertDate)
                .ToListAsync();

            orders = documents.AsQueryable();
        }

        private static WhatsAppMessageDto? GetFirstMessage(WhatsAppWebhookDto order)
            => order.Entry.FirstOrDefault()?.Changes.FirstOrDefault()?.Value.Messages.FirstOrDefault();

        private static WhatsAppContactDto? GetFirstContact(WhatsAppWebhookDto order)
            => order.Entry.FirstOrDefault()?.Changes.FirstOrDefault()?.Value.Contacts.FirstOrDefault();

        private static string GetOrderText(WhatsAppWebhookDto order)
            => GetFirstMessage(order)?.Order?.Text ?? string.Empty;

        private static int GetItemCount(WhatsAppWebhookDto order)
            => GetFirstMessage(order)?.Order?.ProductItems.Count ?? 0;

        private static decimal GetOrderTotal(WhatsAppWebhookDto order)
            => GetFirstMessage(order)?.Order?.ProductItems.Sum(p => p.ItemPrice * p.Quantity) ?? 0;
    }
}
