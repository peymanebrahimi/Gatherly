using Gatherly.Domain.Shared;

namespace Gatherly.Domain.Errors;

public static class DomainErrors
{
	public static class Gathering
	{
		public static readonly Error InvitingCreator = new Error("Gathering.InvitingCreator", "na na khodeti");
		public static readonly Error AlreadyPassed = new Error("Gathering.AlreadyPassed", "Already late");
		public static readonly Error Expired = new Error(
				"Invitation.Expired",
				"Invitation Expired");

    }
}
