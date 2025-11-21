using System.ComponentModel.DataAnnotations;
using SplitPay.Application.Expenses;

namespace SplitPay.Api.Requests;

public sealed record CreateGroupRequest([Required] string Name);

public sealed record AddMemberRequest([Required] string DisplayName);

public sealed record CreateExpensePartRequest([Required] Guid MemberId, decimal? Share, decimal? Percent);

public sealed record CreateExpenseRequest(
    [Required] Guid PayerId,
    [Required] decimal Amount,
    [Required] string Currency,
    string Description,
    [Required] string Method,
    [Required] List<CreateExpensePartRequest> Parts,
    int? ParticipantsCountForEqual)
{
    public CreateExpenseCommand ToCommand(Guid groupId)
    {
        if (!Enum.TryParse<SplitMethod>(Method, true, out var splitMethod))
            throw new ValidationException("Unsupported split method");

        var parts = Parts?.Select(p => new CreateExpensePart(p.MemberId, p.Share, p.Percent)).ToList()
                    ?? throw new ValidationException("Parts are required");

        return new CreateExpenseCommand(groupId, PayerId, Amount, Currency, Description ?? string.Empty, splitMethod, parts, ParticipantsCountForEqual);
    }
}
