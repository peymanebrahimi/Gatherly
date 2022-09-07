using Gatherly.Domain.Entities;

namespace Gatherly.Application.Abstractions;

public interface IEmailService
{
    Task SendInvitationSentEmailAsync(Member member, Gathering gathering, CancellationToken cancellationToken);
    Task SendInvitationAcceptedEmailAsync(Gathering gathering, CancellationToken cancellationToken);
}
