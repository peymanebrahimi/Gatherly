using Gatherly.Domain.Entities;

namespace Gatherly.Domain.Repositories;

public interface IMemberRepository
{
    Task<Member> GetByIdAsync(Guid memberId, CancellationToken cancellationToken);
}
