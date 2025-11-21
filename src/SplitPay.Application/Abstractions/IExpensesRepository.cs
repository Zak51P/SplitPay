using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Abstractions;

public interface IExpensesRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}
