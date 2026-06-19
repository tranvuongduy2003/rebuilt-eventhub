namespace EventHub.DataSeeder.Models;

public sealed record UserSeed(Guid Id, string DisplayName, string Email, string Role);
