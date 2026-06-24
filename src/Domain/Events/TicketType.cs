using EventHub.Domain.Abstractions;

namespace EventHub.Domain.Events;

public sealed class TicketType : Entity<TicketTypeId>
{
    private TicketType()
    {
    }

    public TicketName Name { get; private set; } = null!;

    public Money Price { get; private set; } = null!;

    public Capacity Capacity { get; private set; } = null!;

    public int Sold { get; private set; }

    public int Reserved { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static TicketType Create(
        TicketName name,
        Money price,
        Capacity capacity,
        DateTimeOffset createdAt) =>
        new()
        {
            Name = name,
            Price = price,
            Capacity = capacity,
            Sold = 0,
            Reserved = 0,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };

    public static TicketType FromPersistence(
        TicketTypeId id,
        TicketName name,
        Money price,
        Capacity capacity,
        int sold,
        int reserved,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt) =>
        new()
        {
            Id = id,
            Name = name,
            Price = price,
            Capacity = capacity,
            Sold = sold,
            Reserved = reserved,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };
}
