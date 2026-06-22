namespace EventHub.Contracts.Events;

public sealed record CloseEventResponse(string Status, DateTimeOffset UpdatedAt);
