using System;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Groups;

public sealed record CreateGroupCommand(string Name);

public sealed class CreateGroupCommandHandler
{
    private readonly IGroupsRepository _groups;
    private readonly IUnitOfWork _uow;

    public CreateGroupCommandHandler(IGroupsRepository groups, IUnitOfWork uow)
    {
        _groups = groups;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateGroupCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) throw new ArgumentException("Name is required", nameof(command));

        var group = new Group
        {
            Name = command.Name.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _groups.AddAsync(group, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
