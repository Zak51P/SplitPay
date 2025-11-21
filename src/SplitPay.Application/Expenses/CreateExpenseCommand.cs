using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;
using SplitPay.Domain.ValueObjects;

namespace SplitPay.Application.Expenses;

public sealed record CreateExpensePart(Guid MemberId, decimal? Share, decimal? Percent);

public sealed record CreateExpenseCommand(Guid GroupId, Guid PayerId, decimal Amount, string Currency, string Description, SplitMethod Method, IReadOnlyCollection<CreateExpensePart> Parts, int? ParticipantsCountForEqual = null);

public sealed class CreateExpenseCommandHandler
{
    private readonly IMembersRepository _members;
    private readonly IExpensesRepository _expenses;
    private readonly IUnitOfWork _uow;

    public CreateExpenseCommandHandler(IMembersRepository members, IExpensesRepository expenses, IUnitOfWork uow)
    {
        _members = members;
        _expenses = expenses;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateExpenseCommand command, CancellationToken cancellationToken = default)
    {
        if (command.GroupId == Guid.Empty) throw new ArgumentException("GroupId is required", nameof(command));
        if (command.PayerId == Guid.Empty) throw new ArgumentException("PayerId is required", nameof(command));
        if (command.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(command.Amount));
        if (command.Parts == null || command.Parts.Count == 0) throw new ArgumentException("Parts are required", nameof(command));

        await EnsureMembersBelongToGroup(command.GroupId, new[] { command.PayerId }.Concat(command.Parts.Select(p => p.MemberId)), cancellationToken);

        var amount = new Money(command.Amount, command.Currency);
        var expense = new Expense(command.GroupId, command.PayerId, amount, command.Description, command.Method);

        var parts = BuildParts(command, amount.Currency);
        expense.SetParts(parts, command.ParticipantsCountForEqual ?? 0);

        await _expenses.AddAsync(expense, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }

    private async Task EnsureMembersBelongToGroup(Guid groupId, IEnumerable<Guid> memberIds, CancellationToken cancellationToken)
    {
        foreach (var memberId in memberIds)
        {
            var member = await _members.GetByIdAsync(memberId, cancellationToken);
            if (member == null || member.GroupId != groupId) throw new InvalidOperationException("Member does not belong to the group");
        }
    }

    private static IReadOnlyCollection<SplitPart> BuildParts(CreateExpenseCommand command, string currency)
    {
        return command.Method switch
        {
            SplitMethod.Equal => command.Parts
                .Select(p => SplitPart.Exact(p.MemberId, Money.Zero(currency)))
                .ToList(),
            SplitMethod.Exact => command.Parts
                .Select(p =>
                {
                    if (p.Share is null) throw new ArgumentException("Share is required for Exact");
                    return SplitPart.Exact(p.MemberId, new Money(p.Share.Value, currency));
                })
                .ToList(),
            SplitMethod.Percent => command.Parts
                .Select(p =>
                {
                    if (p.Percent is null) throw new ArgumentException("Percent is required for Percent method");
                    return SplitPart.PercentOf(p.MemberId, p.Percent.Value, currency);
                })
                .ToList(),
            _ => throw new InvalidOperationException("Unsupported split method")
        };
    }
}
