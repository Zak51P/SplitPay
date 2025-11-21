using System;

namespace SplitPay.Application.Dtos;

public record SplitPartDto(Guid MemberId, decimal Share, decimal? Percent);
