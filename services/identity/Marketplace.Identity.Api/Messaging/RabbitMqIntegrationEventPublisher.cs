using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Marketplace.Identity.Api.Messaging;

public sealed class RabbitMqIntegrationEventPublisher(
    IConfiguration configuration,
    ILogger<RabbitMqIntegrationEventPublisher> logger) : IIntegrationEventPublisher, IDisposable
{
    private const string ExchangeName = "marketplace.events";
    private readonly object _sync = new();
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishUserRegisteredAsync(
        UserRegisteredIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        await EnsureTopologyAsync(cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(integrationEvent));
        var properties = new BasicProperties { ContentType = "application/json", DeliveryMode = DeliveryModes.Persistent };

        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: "identity.user.registered",
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Published UserRegistered for {UserId} to {Exchange}",
            integrationEvent.UserId,
            ExchangeName);
    }

    private async Task EnsureTopologyAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            return;
        }

        lock (_sync)
        {
            if (_channel is not null)
            {
                return;
            }

            var section = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = section["Host"] ?? "localhost",
                Port = section.GetValue("Port", 5672),
                UserName = section["UserName"] ?? "marketplace",
                Password = section["Password"] ?? "change_me_local_only"
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken).GetAwaiter().GetResult();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
