using System;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Abstractions;

public interface IMembersRepository
{
    Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
}
