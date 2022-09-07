namespace Gatherly.Domain.Entities;

public class Attendee
{
    internal Attendee(Invitation invitation)
    {
        MemberId = invitation.MemberId;
        GatheringId = invitation.GatheringId;
        CreatedOnUtc = DateTime.UtcNow;
    }
    public Guid MemberId { get; private set; }
    public Guid GatheringId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

}
