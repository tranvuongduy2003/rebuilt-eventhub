namespace Solution.Contracts.Users;

public sealed record LoginUserResponse(Guid UserId, string DisplayName, string Email);
