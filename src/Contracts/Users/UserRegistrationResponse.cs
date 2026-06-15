namespace Solution.Contracts.Users;

public sealed record UserRegistrationResponse(
    Guid UserId,
    string DisplayName,
    string Email,
    DateTimeOffset CreatedAt);
