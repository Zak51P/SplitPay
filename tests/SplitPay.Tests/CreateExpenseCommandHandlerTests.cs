using Microsoft.EntityFrameworkCore;
using SplitPay.Application.Abstractions;
using SplitPay.Application.Expenses;
using SplitPay.Application.Members;
using SplitPay.Domain.Entities;
using SplitPay.Domain.ValueObjects;
using SplitPay.Infrastructure.Persistence;
using SplitPay.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SplitPay.Tests;

public class CreateExpenseCommandHandlerTests
{
    private readonly AppDbContext _context;
    private readonly IMembersRepository _membersRepository;
    private readonly IExpensesRepository _expensesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExpenseCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _membersRepository = new MembersRepository(_context);
        _expensesRepository = new ExpensesRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public async Task CreatesExpense_WithEqualSplit()
    {
        var group = new Group { Name = "Trip" };
        _context.Groups.Add(group);

        var members = new[]
        {
            new Member { Group = group, GroupId = group.Id, DisplayName = "A" },
            new Member { Group = group, GroupId = group.Id, DisplayName = "B" }
        };
        _context.Members.AddRange(members);
        await _context.SaveChangesAsync();

        var handler = new CreateExpenseCommandHandler(_membersRepository, _expensesRepository, _unitOfWork);

        var parts = members.Select(m => new CreateExpensePart(m.Id, null, null)).ToList();
        var command = new CreateExpenseCommand(group.Id, members.First().Id, 100m, "USD", "Test", SplitMethod.Equal, parts, null);

        var expenseId = await handler.Handle(command);

        var tracked = _context.ChangeTracker.Entries<Expense>().Single().Entity;
        Assert.Equal(expenseId, tracked.Id);
        Assert.Equal(members.Length, tracked.Parts.Count);
    }
}
