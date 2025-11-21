using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Repositories;

public sealed class MembersRepository : IMembersRepository
{
    private readonly AppDbContext _context;

    public MembersRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Members.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _context.Members.AddAsync(member, cancellationToken);
    }
}
