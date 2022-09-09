using Gatherly.Application.Abstractions;
using Gatherly.Domain.DomainEvents;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Invitations.Events;

internal sealed class InvitationAcceptedDomainEventHandler
    : INotificationHandler<InvitationAcceptedDomainEvent>
{
    private readonly IEmailService emailService;
    private readonly IGatheringRepository gatheringRepository;

    public InvitationAcceptedDomainEventHandler(IEmailService emailService,
        IGatheringRepository gatheringRepository)
    {
        this.emailService = emailService;
        this.gatheringRepository = gatheringRepository;
    }

    public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var gathering = await gatheringRepository
            .GetByIdWithCreatorAsync(notification.GatheringId, cancellationToken);
        
        if (gathering is null) return;

        await emailService.SendInvitationAcceptedEmailAsync(gathering, cancellationToken);
    }
}
