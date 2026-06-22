using FluentValidation;

namespace EventHub.Application.Events.Commands;

public sealed class CloseEventCommandValidator : AbstractValidator<CloseEventCommand>
{
    public CloseEventCommandValidator()
    {
        RuleFor(c => c.EventId)
            .GreaterThan(0)
            .WithMessage("Event ID must be a positive integer.");
    }
}
