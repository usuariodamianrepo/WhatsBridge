using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SmartAnt.Worker
{
    public class Consumir : BackgroundService
    {
        private readonly ILogger<Consumir> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly IUnitOfWork _unitOfWork;

        public Consumir(ILogger<Consumir> logger, IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //configuracion RabbitQM

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                };

                using var connection = await factory.CreateConnectionAsync();
                using (var channel = await connection.CreateChannelAsync())
                {
                    await channel.QueueDeclareAsync(
                    queue: "fila",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                    //consumidor de eventos asincrono

                    var consumer = new AsyncEventingBasicConsumer(channel);

                    consumer.ReceivedAsync += async (sender, eventsargs) =>
                    {
                        var body = eventsargs.Body.ToArray();
                        var mensagem = Encoding.UTF8.GetString(body);
                        var whatsAppOrder = JsonSerializer.Deserialize<WhatsAppOrder>(mensagem);

                        var order = new Order
                        {
                            UserName = whatsAppOrder?.User.Name ?? string.Empty,
                            UserEmail = whatsAppOrder?.User.Email ?? string.Empty,
                            Quantity = whatsAppOrder?.Quantity ?? 0
                        };

                        using var scope = _serviceProvider.CreateScope();

                        if (order != null)
                        {
                            // Adiciona ou atualiza no banco de dados.
                            await _unitOfWork.Repository<Order>().AddAsync(order);
                            await _unitOfWork.SaveAsync();
                        }

                        Console.WriteLine(order?.ToString());
                    };

                    //despachando a mensagem
                    await channel.BasicConsumeAsync(
                        queue: "fila",
                        autoAck: true,
                        consumer: consumer);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

