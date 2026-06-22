using EventHub.Domain.Abstractions;

namespace EventHub.Domain.Events;

public sealed record EventClosedEvent(EventId EventId, DateTimeOffset ClosedAt) : DomainEvent;
