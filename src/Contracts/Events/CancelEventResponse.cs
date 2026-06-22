namespace EventHub.Contracts.Events;

public sealed record CancelEventResponse(string Status, DateTimeOffset CancelledAt, DateTimeOffset UpdatedAt);
