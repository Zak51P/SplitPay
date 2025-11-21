using System;
using System.Collections.Generic;
using SplitPay.Domain.Entities;

namespace SplitPay.Application.Dtos;

public record ExpenseDto(Guid Id, Guid GroupId, Guid PayerId, MoneyDto Amount, string Description, DateTime OccurredAtUtc, SplitMethod Method, IReadOnlyCollection<SplitPartDto> Parts);
