using System;
using System.Threading;
using System.Threading.Tasks;
using SplitPay.Application.Abstractions;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Members;

public sealed record AddMemberCommand(Guid GroupId, string DisplayName);

public sealed class AddMemberCommandHandler
{
    private readonly IGroupsRepository _groups;
    private readonly IMembersRepository _members;
    private readonly IUnitOfWork _uow;

    public AddMemberCommandHandler(IGroupsRepository groups, IMembersRepository members, IUnitOfWork uow)
    {
        _groups = groups;
        _members = members;
        _uow = uow;
    }

    public async Task<Guid> Handle(AddMemberCommand command, CancellationToken cancellationToken = default)
    {
        if (command.GroupId == Guid.Empty) throw new ArgumentException("GroupId is required", nameof(command));
        if (string.IsNullOrWhiteSpace(command.DisplayName)) throw new ArgumentException("DisplayName is required", nameof(command));

        var group = await _groups.GetByIdAsync(command.GroupId, cancellationToken) ?? throw new InvalidOperationException("Group not found");

        var member = new Member
        {
            GroupId = group.Id,
            Group = group,
            DisplayName = command.DisplayName.Trim(),
            JoinedAtUtc = DateTime.UtcNow
        };

        await _members.AddAsync(member, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return member.Id;
    }
}
