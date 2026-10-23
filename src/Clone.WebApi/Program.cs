using Clone.WebApi.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory
    {
        HostName = "host.docker.internal"
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<OrderPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
