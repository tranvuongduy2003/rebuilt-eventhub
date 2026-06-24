using EventHub.Domain.Abstractions;
using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Events;

public sealed class Capacity : ValueObject
{
    private Capacity()
    {
    }

    public int Value { get; private set; }

    public static Capacity Create(int value)
    {
        if (value <= 0)
        {
            throw new BusinessRuleValidationException(
                "INVALID_TICKET_TYPE_CAPACITY",
                "Capacity must be a positive integer.");
        }

        return new Capacity { Value = value };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
