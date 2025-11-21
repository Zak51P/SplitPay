using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Configurations;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasMany(g => g.Members)
            .WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
