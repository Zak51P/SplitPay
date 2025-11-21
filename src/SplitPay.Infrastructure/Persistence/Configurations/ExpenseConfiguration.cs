using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Configurations;

internal sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.OccurredAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(e => e.Method)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(e => e.GroupId).IsRequired();
        builder.Property(e => e.PayerId).IsRequired();

        builder.HasIndex(e => e.GroupId);
        builder.HasIndex(e => e.PayerId);

        builder.OwnsOne(e => e.Amount, MoneyConfiguration.ConfigureOwned);

        builder.OwnsMany(e => e.Parts, SplitPartConfiguration.Configure);

        builder.Navigation(e => e.Parts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
