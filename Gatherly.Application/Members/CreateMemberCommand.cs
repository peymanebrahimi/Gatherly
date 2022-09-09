using MediatR;

namespace Gatherly.Application.Members;

public record CreateMemberCommand(string Email, string FirstName, string LastName) : IRequest;
