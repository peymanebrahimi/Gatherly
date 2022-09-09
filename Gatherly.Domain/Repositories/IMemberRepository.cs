using Gatherly.Domain.Entities;

namespace Gatherly.Domain.Repositories;

public interface IMemberRepository
{
    void Add(Member member);
    Task<Member> GetByIdAsync(Guid memberId, CancellationToken cancellationToken);
}
