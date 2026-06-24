using EventHub.Domain.Abstractions;
using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Events;

public sealed class TicketName : ValueObject
{
    private TicketName()
    {
    }

    public string Value { get; private set; } = null!;

    public static TicketName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException(
                "INVALID_TICKET_TYPE_NAME",
                "Ticket type name is required.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > 200)
        {
            throw new BusinessRuleValidationException(
                "INVALID_TICKET_TYPE_NAME",
                "Ticket type name must not exceed 200 characters.");
        }

        return new TicketName { Value = trimmed };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
