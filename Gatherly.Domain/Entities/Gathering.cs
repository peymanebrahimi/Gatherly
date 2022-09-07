using Gatherly.Domain.Enums;
using Gatherly.Domain.Errors;
using Gatherly.Domain.Exceptions;
using Gatherly.Domain.Primitives;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using System.Threading;

namespace Gatherly.Domain.Entities;

public sealed class Gathering : Entity
{
    private readonly List<Invitation> _invitations = new();
    private readonly List<Attendee> _attendees = new();
    private Gathering(Guid id, Member creator, GatheringType type, DateTime scheduledAt, string name, string? location)
        : base(id)
    {
        Creator = creator;
        Type = type;
        ScheduledAt = scheduledAt;
        Name = name;
        Location = location;
    }

    public Member Creator { get; private set; }
    public GatheringType Type { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string Name { get; private set; }
    public string? Location { get; private set; }
    public int? MaximumNumberOfAttendees { get; private set; }
    public DateTime? InvitationsExpireAtUtc { get; private set; }
    public int NumberOfAttendees { get; private set; }
    public IReadOnlyCollection<Attendee> Attendees => _attendees;
    public IReadOnlyCollection<Invitation> Invitations => _invitations;

    public static Gathering Create(Guid id, Member creator, GatheringType type,
        DateTime scheduledAt, string name, string? location, int? maximumNumberOfAttendees,
    int? invitationsValidBeforeInHours)
    {
        var gathering = new Gathering(id, creator, type, scheduledAt, name, location);

        switch (gathering.Type)
        {
            case GatheringType.WithFixedNumberOfAttendees:
                //ArgumentNullException.ThrowIfNull(maximumNumberOfAttendees, nameof(maximumNumberOfAttendees));
                if (maximumNumberOfAttendees is null)
                {
                    throw new GatheringMaximumNumberOfAttendeesIsNullDomainException(
                        $"{nameof(maximumNumberOfAttendees)} can't be null.");
                }

                gathering.MaximumNumberOfAttendees = maximumNumberOfAttendees;
                break;
            case GatheringType.WithExpirationForInvitations:
                //ArgumentNullException.ThrowIfNull(invitationsValidBeforeInHours, nameof(invitationsValidBeforeInHours));
                if (invitationsValidBeforeInHours is null)
                {
                    throw new InvitationsValidBeforeInHoursIsNullDomainException(
                        $"{nameof(invitationsValidBeforeInHours)} can't be null.");
                }

                gathering.InvitationsExpireAtUtc = gathering.ScheduledAt.AddHours(-invitationsValidBeforeInHours.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gathering.Type));
        }

        return gathering;
    }


    public Result<Invitation> SendInvitation(Member member)
    {
        if (Creator.Id == member.Id)
            return Result.Failure<Invitation>(DomainErrors.Gathering.InvitingCreator);

        if (ScheduledAt < DateTime.UtcNow)
            return Result.Failure<Invitation>(DomainErrors.Gathering.AlreadyPassed);

        var invitation = new Invitation(Guid.NewGuid(), member, this);

        _invitations.Add(invitation);

        return invitation;
    }

    public Attendee? AcceptInvitation(Invitation invitation)
    {
        var expired = (Type == GatheringType.WithFixedNumberOfAttendees &&
            NumberOfAttendees == MaximumNumberOfAttendees) ||
            (Type == GatheringType.WithExpirationForInvitations &&
            InvitationsExpireAtUtc < DateTime.UtcNow);
        if (expired)
        {
            invitation.Expire();
            //await unitOfWork.SaveChangesAsync(cancellationToken);

            return null;
        }

        var attendee = invitation.Accept();

        _attendees.Add(attendee);

        NumberOfAttendees++;

        return attendee;
    }
}
