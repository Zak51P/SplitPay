namespace SplitPay.Domain.Entities;

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = default!;
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;
}
