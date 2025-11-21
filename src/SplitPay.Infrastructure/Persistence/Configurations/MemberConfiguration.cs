using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Configurations;

internal sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.JoinedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(m => m.GroupId)
            .IsRequired();

        builder.HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
