using Gatherly.Application.Abstractions;
using Gatherly.Domain.Enums;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IInvitationRepository invitationRepository;
    private readonly IMemberRepository memberRepository;
    private readonly IGatheringRepository gatheringRepository;
    private readonly IAttendeeRepository attendeeRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEmailService emailService;

    public AcceptInvitationCommandHandler(IInvitationRepository invitationRepository,
        IMemberRepository memberRepository,
        IGatheringRepository gatheringRepository,
        IAttendeeRepository attendeeRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        this.invitationRepository = invitationRepository;
        this.memberRepository = memberRepository;
        this.gatheringRepository = gatheringRepository;
        this.attendeeRepository = attendeeRepository;
        this.unitOfWork = unitOfWork;
        this.emailService = emailService;
    }
    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await invitationRepository.GetByIdAsync(request.invitationId, cancellationToken);

        if (invitation is null || invitation.Status != InvitationStatus.Pending)
            return Unit.Value;

        var member = await memberRepository.GetByIdAsync(invitation.MemberId, cancellationToken);
        var gathering = await gatheringRepository.GetByIdWithCreatorAsync(invitation.GatheringId, cancellationToken);

        if (member is null || gathering is null)
            return Unit.Value;

        var attendee = gathering.AcceptInvitation(invitation);

        if (attendee is not null)
        {
            attendeeRepository.Add(attendee);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (invitation.Status == InvitationStatus.Accepted)
        {
            await emailService.SendInvitationAcceptedEmailAsync(gathering, cancellationToken);
        }

        return Unit.Value;
    }
}
