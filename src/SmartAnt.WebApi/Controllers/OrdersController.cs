using Clone.Models;
using Clone.Utility;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SmartAnt.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private const string X_API_KEY = "3416117792"; // Replace with your

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { DateTime.UtcNow.ToDateHHmm(), Guid.NewGuid().ToString() };
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WhatsAppOrder order, [FromHeader(Name = "x-api-key")] string verify_token)
        {
            if (verify_token != X_API_KEY)
            {
                return Unauthorized("Invalid verify token.");
            }

            Console.WriteLine($"Received message: {order}");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            {
                await channel.QueueDeclareAsync(
                    queue: "fila",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //publicando a mensagem

                var message = JsonSerializer.Serialize(order);

                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "fila",
                    mandatory: false, // Nuevo parámetro recomendado en la versión asíncrona
                    basicProperties: new RabbitMQ.Client.BasicProperties(), // Propiedades predeterminadas vacías
                    body: body);

            }

            return Ok(order);
        }
    }
}
