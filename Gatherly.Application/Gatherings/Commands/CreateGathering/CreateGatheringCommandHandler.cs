using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Gatherings.Commands.CreateGathering;

public class CreateGatheringCommandHandler : IRequestHandler<CreateGatheringCommand>
{
    private readonly IMemberRepository memberRepository;
    private readonly IGatheringRepository gatheringRepository;
    private readonly IUnitOfWork unitOfWork;

    public CreateGatheringCommandHandler(IMemberRepository memberRepository,
        IGatheringRepository gatheringRepository, IUnitOfWork unitOfWork)
    {
        this.memberRepository = memberRepository;
        this.gatheringRepository = gatheringRepository;
        this.unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(CreateGatheringCommand request, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null)
            return Unit.Value;

        var gathering = Gathering.Create(Guid.NewGuid(), member,
            request.Type, request.ScheduledAtUtc, request.Name, request.Location,
            request.MaximumNumberOfAttendees, request.InvitationsValidBeforeInHours);

        gatheringRepository.Add(gathering);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
