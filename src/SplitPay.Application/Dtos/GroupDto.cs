using System;
using System.Collections.Generic;

namespace SplitPay.Application.Dtos;

public record GroupDto(Guid Id, string Name, DateTime CreatedAtUtc, IReadOnlyCollection<MemberDto> Members, IReadOnlyCollection<ExpenseDto> Expenses);
