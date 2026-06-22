using EventHub.Application.Abstractions.Auth;
using EventHub.Application.Abstractions.Messaging;
using EventHub.Domain.Events;

namespace EventHub.Application.Events.Commands;

public sealed record CancelEventCommand(int EventId)
    : ICommand<CancelEventResult>, IAuthorizeEventOperation
{
    EventId IAuthorizeEventOperation.EventId => Domain.Events.EventId.From(EventId);

    Permission IAuthorizeEventOperation.RequiredPermission => Permission.EventManagement;
}

public sealed record CancelEventResult(string Status, DateTimeOffset CancelledAt, DateTimeOffset UpdatedAt);
