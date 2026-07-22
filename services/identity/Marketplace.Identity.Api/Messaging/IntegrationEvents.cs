namespace Marketplace.Identity.Api.Messaging;

public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Email,
    string Role,
    string PreferredCulture,
    DateTime OccurredAtUtc);

public interface IIntegrationEventPublisher
{
    Task PublishUserRegisteredAsync(UserRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
