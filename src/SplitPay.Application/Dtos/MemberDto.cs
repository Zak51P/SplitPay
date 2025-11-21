using System;

namespace SplitPay.Application.Dtos;

public record MemberDto(Guid Id, string DisplayName, DateTime JoinedAtUtc);
