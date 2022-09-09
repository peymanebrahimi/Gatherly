using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gatherly.Application.Members;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand>
{
	private readonly IMemberRepository memberRepository;
	private readonly IUnitOfWork unitOfWork;

	public CreateMemberCommandHandler(IMemberRepository memberRepository,
		IUnitOfWork unitOfWork)
	{
		this.memberRepository = memberRepository;
		this.unitOfWork = unitOfWork;
	}

	public async Task<Unit> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
	{
		var firstNameResult = FirstName.Create(request.FirstName);

		if (firstNameResult.IsFailure)
		{
			// log error
			return Unit.Value;
		}

		var member = new Member(
			Guid.NewGuid(),
			firstNameResult.Value,
			request.LastName,
			request.Email);

		memberRepository.Add(member);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}
