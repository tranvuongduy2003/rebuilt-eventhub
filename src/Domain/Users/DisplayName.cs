using Solution.Domain.Abstractions;
using Solution.Domain.Exceptions;

namespace Solution.Domain.Users;

public sealed class DisplayName : ValueObject
{
    private const int MinimumLength = 1;
    private const int MaximumLength = 64;

    private DisplayName(string value) => Value = value;

    public string Value { get; }

    public static DisplayName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException(
                "DISPLAY_NAME_REQUIRED",
                "Display name is required.");
        }

        var trimmed = value.Trim();

        if (trimmed.Length is < MinimumLength or > MaximumLength)
        {
            throw new BusinessRuleValidationException(
                "DISPLAY_NAME_LENGTH",
                $"Display name must be between {MinimumLength} and {MaximumLength} characters.");
        }

        return new DisplayName(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
