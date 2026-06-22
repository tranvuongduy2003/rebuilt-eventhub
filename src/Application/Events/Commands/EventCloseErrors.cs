using EventHub.Application.Common;

namespace EventHub.Application.Events.Commands;

public static class EventCloseErrors
{
    public static readonly Error EventNotFound = Error.NotFound(
        "EVENT_NOT_FOUND",
        "The event was not found.");
}
