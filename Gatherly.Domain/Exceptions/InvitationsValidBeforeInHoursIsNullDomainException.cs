namespace Gatherly.Domain.Exceptions;

public sealed class InvitationsValidBeforeInHoursIsNullDomainException : DomainException
{
    public InvitationsValidBeforeInHoursIsNullDomainException(string message) : base(message)
    {
    }
}
