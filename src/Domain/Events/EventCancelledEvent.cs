using EventHub.Domain.Abstractions;

namespace EventHub.Domain.Events;

public sealed record EventCancelledEvent(EventId EventId, DateTimeOffset CancelledAt) : DomainEvent;
