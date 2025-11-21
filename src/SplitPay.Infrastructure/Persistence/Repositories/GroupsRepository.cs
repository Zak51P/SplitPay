using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Repositories;

public sealed class GroupsRepository : IGroupsRepository
{
    private readonly AppDbContext _context;

    public GroupsRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public Task<Group?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }
}
