using Gatherly.Application.Abstractions;
using Gatherly.Domain.Enums;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IGatheringRepository gatheringRepository;
    private readonly IAttendeeRepository attendeeRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEmailService emailService;

    public AcceptInvitationCommandHandler(IGatheringRepository gatheringRepository,
        IAttendeeRepository attendeeRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        this.gatheringRepository = gatheringRepository;
        this.attendeeRepository = attendeeRepository;
        this.unitOfWork = unitOfWork;
        this.emailService = emailService;
    }
    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var gathering = await gatheringRepository.GetByIdWithCreatorAsync(request.gatheringId, cancellationToken);

        if (gathering is null)
        {
            return Unit.Value;
        }

        var invitation = gathering.Invitations.FirstOrDefault(i => i.Id == request.invitationId);

        if (invitation is null || invitation.Status != InvitationStatus.Pending)
            return Unit.Value;

        var attendeeResult = gathering.AcceptInvitation(invitation);

        if (attendeeResult.IsSuccess)
        {
            attendeeRepository.Add(attendeeResult.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        //if (invitation.Status == InvitationStatus.Accepted)
        //{
        //    await emailService.SendInvitationAcceptedEmailAsync(gathering, cancellationToken);
        //}

        return Unit.Value;
    }
}
