using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Repositories;

public sealed class ExpensesRepository : IExpensesRepository
{
    private readonly AppDbContext _context;

    public ExpensesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _context.Expenses.AddAsync(expense, cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Expenses
            .Include(e => e.Parts)
            .ThenInclude(p => p.Share)
            .Where(e => e.GroupId == groupId)
            .OrderBy(e => e.OccurredAtUtc)
            .ToListAsync(cancellationToken);

        return list;
    }
}
