using Clone.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Clone.Web.Extensions
{
    public static class OrdersApiEndpoints
    {
        const string API_KEY = "API-KEY-4564636";

        public static IEndpointRouteBuilder MapOrdersApiEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/orders-minimal");

            group.MapGet("/{id}", async (
                string id,
                [FromHeader(Name = "x-api-key")] string ? apiKey,
                IMongoDatabase database
                ) =>
            {
                if (apiKey != API_KEY)
                {
                    return Results.Unauthorized();
                }

                if (!ObjectId.TryParse(id, out var objectId))
                {
                    return Results.BadRequest(new { message = "Id not valid." });
                }

                var order = await database
                    .GetCollection<WhatsAppWebhookDto>("Orders")
                    .Find(o => o.Id == objectId)
                    .FirstOrDefaultAsync();

                if (order is null)
                {
                    return Results.NotFound();
                }

                var orderDto = new OrderDto
                {
                    Id = order.Id.ToString(),
                    InsertDate = order.InsertDate
                };

                return Results.Ok(orderDto);
            });

            group.MapGet("/", async(
                [FromQuery] DateTime ? dateFrom,
                [FromQuery] DateTime ? dateTo,
                [FromHeader(Name = "x-api-key")] string? apiKey,
                IMongoDatabase database
                ) =>
            {
                if (apiKey != API_KEY)
                {
                    return Results.Unauthorized();
                }

                if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
                {
                    return Results.BadRequest(new { message = "DateFrom cannot be greater than DateTo." });
                }

                var filter = Builders<WhatsAppWebhookDto>.Filter.Empty;

                if (dateFrom.HasValue)
                {
                    filter &= Builders<WhatsAppWebhookDto>.Filter.Gte(o => o.InsertDate, dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    filter &= Builders<WhatsAppWebhookDto>.Filter.Lte(o => o.InsertDate, dateTo.Value);
                }

                var orders = await database
                    .GetCollection<WhatsAppWebhookDto>("Orders")
                    .Find(filter)
                    .SortByDescending(o => o.InsertDate)
                    .ToListAsync();

                var ordersDto = orders.Select(o => new OrderDto
                {
                    Id = o.Id.ToString(),
                    InsertDate = o.InsertDate
                });

                return Results.Ok(ordersDto);
            });

            return endpoints;
        }

    }
}
