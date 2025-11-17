using System.Runtime.CompilerServices;
using SplitPay.Domain.ValueObjects;

namespace SplitPay.Domain.Entities;

public enum SplitMethod { Equal, Exact, Percent }

public class SplitPart
{
    public Guid MemberId { get; private set; }
    public Money Share { get; private set; }
    public decimal? Percent { get; private set; }

    private SplitPart() { }
    private SplitPart(Guid memberId, Money share, decimal? percent)
    {
        MemberId = memberId;
        Share = share;
        Percent = percent;
    }
    // ПОНЯТЬ ЧЕ ТУТ ПРОИСХОИДТ)))ФЫ0В9ФЫ0В8)))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
    public static SplitPart Exact(Guid memberId, Money share) => new(memberId, share, null);
    public static SplitPart PercentOf(Guid memberId, decimal percent, string currency)
    {
        if (percent is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(percent));
        return new SplitPart(memberId, Money.Zero(currency), percent);
    }
    internal void SetShare(Money share) => Share = share;
}
public class Expense
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid GroupId { get; private set; }
    public Guid PayerId { get; private set; }
    public Money Amount { get; private set; }
    public string Description { get; private set; } = "";
    public DateTime OccurredAtUtc { get; private set; } = DateTime.UtcNow;
    public SplitMethod Method { get; private set; }

    private readonly List<SplitPart> _parts = new();
    public IReadOnlyCollection<SplitPart> Parts => _parts.AsReadOnly();

    private Expense() { }

    public Expense(Guid groupId, Guid payerId, Money amount, string description, SplitMethod method)
    {
        if (groupId == Guid.Empty) throw new ArgumentException(nameof(groupId));
        if (payerId == Guid.Empty) throw new ArgumentException(nameof(payerId));
        if (amount.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        GroupId = groupId; PayerId = payerId; Amount = amount;
        Description = description?.Trim() ?? ""; Method = method;
    }

    public void SetParts(IEnumerable<SplitPart> parts, int participantsCountForEqual = 0)
    {
        _parts.Clear(); _parts.AddRange(parts);
        RecalculateShares(participantsCountForEqual);
    }

    private void RecalculateShares(int participantsCountForEqual)
    {
        switch (Method)
        {
            case SplitMethod.Equal:
                if (participantsCountForEqual <= 0) participantsCountForEqual = _parts.Count;
                var each = new Money(Math.Round(Amount.Amount / Math.Max(1, participantsCountForEqual), 2), Amount.Currency);
                foreach (var p in _parts) p.SetShare(each);
                break;

            case SplitMethod.Exact:
                var sum = _parts.Sum(p => p.Share.Amount);
                if (Math.Round(sum, 2) != Math.Round(Amount.Amount, 2))
                    throw new InvalidOperationException("Sum of exact shares must equal total amount");
                break;

            case SplitMethod.Percent:
                var pct = _parts.Sum(p => p.Percent ?? 0m);
                if (Math.Round(pct, 2) != 100m) throw new InvalidOperationException("Percent shares must sum to 100%");
                foreach (var p in _parts)
                {
                    var part = Math.Round(Amount.Amount * ((p.Percent ?? 0m) / 100m), 2);
                    p.SetShare(new Money(part, Amount.Currency));
                }
                break;
        }
    }
}
