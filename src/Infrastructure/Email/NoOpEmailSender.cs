using Solution.Application.Abstractions.Email;

namespace Solution.Infrastructure.Email;

public sealed class NoOpEmailSender : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
