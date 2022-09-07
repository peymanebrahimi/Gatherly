using Gatherly.Application.Abstractions;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gatherly.Application.Invitations.Commands.SendInvitation;

public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand>
{
    private readonly IMemberRepository memberRepository;
    private readonly IGatheringRepository gatheringRepository;
    private readonly IInvitationRepository invitationRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEmailService emailService;

    public SendInvitationCommandHandler(IMemberRepository memberRepository,
        IGatheringRepository gatheringRepository,
        IInvitationRepository invitationRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        this.memberRepository = memberRepository;
        this.gatheringRepository = gatheringRepository;
        this.invitationRepository = invitationRepository;
        this.unitOfWork = unitOfWork;
        this.emailService = emailService;
    }

    public async Task<Unit> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.MemberId, cancellationToken);

        var gathering = await gatheringRepository.GetByIdWithCreatorAsync(request.GatheringId, cancellationToken);

        if (member is null || gathering is null)
            return Unit.Value;

        Result<Invitation> result = gathering.SendInvitation(member);

        if (result.IsFailure)
        {
            //log error
            return Unit.Value;
        }
        invitationRepository.Add(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailService.SendInvitationSentEmailAsync(member, gathering, cancellationToken);

        return Unit.Value;
    }
}
