using Solution.Application.Abstractions.Messaging;

namespace Solution.Infrastructure.Messaging;

public sealed class NoOpIntegrationEventPublisher : IIntegrationEventPublisher
{
    public Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
        where TIntegrationEvent : class =>
        Task.CompletedTask;
}
