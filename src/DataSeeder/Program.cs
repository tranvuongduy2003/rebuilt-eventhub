using System.Text.Json;
using EventHub.DataSeeder.Models;
using EventHub.Domain.Users;
using EventHub.Infrastructure.Persistence;
using EventHub.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

const string defaultPassword = "DevPass123!";

var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

// Priority: env var (Aspire injects ConnectionStrings__app) → appsettings.json
var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__app")
    ?? ReadConnectionStringFromAppSettings()
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__App");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("Connection string 'App' not found.");
    return 1;
}

var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
if (!Directory.Exists(dataDirectory))
{
    Console.Error.WriteLine($"Data directory not found: {dataDirectory}");
    return 1;
}

var options = new DbContextOptionsBuilder<ApplicationDatabaseContext>()
    .UseNpgsql(connectionString, npgsql =>
        npgsql.MigrationsHistoryTable("__ef_migrations_history", ApplicationDatabaseContext.SchemaName))
    .Options;

await using var dbContext = new ApplicationDatabaseContext(options);

Console.WriteLine("Applying migrations...");
await dbContext.Database.MigrateAsync();
Console.WriteLine("Migrations applied.");

var identityHasher = new PasswordHasher<object>();
var password = Password.Create(defaultPassword);
var hash = identityHasher.HashPassword(null!, password.Value);
var passwordHash = PasswordHash.Create(hash);
var now = DateTimeOffset.UtcNow;

await SeedUsersAsync(dbContext, passwordHash, now, dataDirectory, jsonOptions);

Console.WriteLine("Seeding complete.");
return 0;

// ── Helpers ─────────────────────────────────────────────────────

static string? ReadConnectionStringFromAppSettings()
{
    var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    if (!File.Exists(path))
    {
        return null;
    }

    using var doc = JsonDocument.Parse(File.ReadAllText(path));
    return doc.RootElement
        .GetProperty("ConnectionStrings")
        .GetProperty("App")
        .GetString();
}

// ── Seed logic ──────────────────────────────────────────────────

static async Task SeedUsersAsync(
    ApplicationDatabaseContext dbContext,
    PasswordHash passwordHash,
    DateTimeOffset now,
    string dataDirectory,
    JsonSerializerOptions jsonOptions)
{
    if (await dbContext.Users.AnyAsync())
    {
        Console.WriteLine("Users already exist — skipping.");
        return;
    }

    var filePath = Path.Combine(dataDirectory, "Users.json");
    if (!File.Exists(filePath))
    {
        Console.Error.WriteLine($"Users.json not found: {filePath}");
        return;
    }

    var json = await File.ReadAllTextAsync(filePath);
    var seeds = JsonSerializer.Deserialize<List<UserSeed>>(json, jsonOptions);

    if (seeds is null or { Count: 0 })
    {
        Console.Error.WriteLine("Users.json is empty or invalid.");
        return;
    }

    var records = seeds.Select(seed => new UserRecord
    {
        Id = seed.Id,
        DisplayName = seed.DisplayName,
        Email = seed.Email.ToLowerInvariant(),
        PasswordHash = passwordHash.Value,
        Role = Enum.Parse<UserRole>(seed.Role),
        CreatedAt = now,
        UpdatedAt = now,
        RowVersion = 1,
    }).ToList();

    dbContext.Users.AddRange(records);
    await dbContext.SaveChangesAsync();

    Console.WriteLine($"Seeded {records.Count} users.");
}
