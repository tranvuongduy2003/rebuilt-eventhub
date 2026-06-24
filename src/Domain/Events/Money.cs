using EventHub.Domain.Abstractions;
using EventHub.Domain.Exceptions;

namespace EventHub.Domain.Events;

public sealed class Money : ValueObject
{
    private Money()
    {
    }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = null!;

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new BusinessRuleValidationException(
                "INVALID_TICKET_TYPE_PRICE",
                "Price amount must be non-negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new BusinessRuleValidationException(
                "INVALID_TICKET_TYPE_PRICE",
                "Currency is required.");
        }

        return new Money
        {
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
