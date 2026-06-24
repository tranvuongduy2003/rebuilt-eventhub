using EventHub.Domain.Abstractions;

namespace EventHub.Domain.Events;

public sealed record TicketTypeAddedEvent(
    EventId EventId,
    TicketTypeId TicketTypeId) : DomainEvent;
