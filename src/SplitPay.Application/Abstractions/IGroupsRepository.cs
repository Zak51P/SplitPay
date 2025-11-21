using System;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Abstractions;

public interface IGroupsRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Group?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
}
