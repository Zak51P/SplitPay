using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Application.Abstractions;
using SplitPay.Application.Dtos;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Groups;

public sealed record GetGroupDetailsQuery(Guid GroupId);

public sealed class GetGroupDetailsQueryHandler
{
    private readonly IGroupsRepository _groups;
    private readonly IExpensesRepository _expenses;

    public GetGroupDetailsQueryHandler(IGroupsRepository groups, IExpensesRepository expenses)
    {
        _groups = groups;
        _expenses = expenses;
    }

    public async Task<GroupDto?> Handle(GetGroupDetailsQuery query, CancellationToken cancellationToken = default)
    {
        if (query.GroupId == Guid.Empty) throw new ArgumentException("GroupId is required", nameof(query));

        var group = await _groups.GetByIdWithMembersAsync(query.GroupId, cancellationToken);
        if (group == null) return null;

        var expenses = await _expenses.GetByGroupIdAsync(query.GroupId, cancellationToken);

        return Map(group, expenses);
    }

    private static GroupDto Map(Group group, IReadOnlyCollection<Expense> expenses)
    {
        var memberDtos = group.Members.Select(m => new MemberDto(m.Id, m.DisplayName, m.JoinedAtUtc)).ToList();

        var expenseDtos = expenses.Select(e =>
        {
            var moneyDto = new MoneyDto(e.Amount.Amount, e.Amount.Currency);
            var parts = e.Parts.Select(p => new SplitPartDto(p.MemberId, p.Share.Amount, p.Percent)).ToList();
            return new ExpenseDto(e.Id, e.GroupId, e.PayerId, moneyDto, e.Description, e.OccurredAtUtc, e.Method, parts);
        }).ToList();

        return new GroupDto(group.Id, group.Name, group.CreatedAtUtc, memberDtos, expenseDtos);
    }
}
