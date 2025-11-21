using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitPay.Domain.Entities;

namespace SplitPay.Infrastructure.Persistence.Configurations;

internal static class SplitPartConfiguration
{
    public static void Configure(OwnedNavigationBuilder<Expense, SplitPart> builder)
    {
        builder.ToTable("split_parts");

        builder.WithOwner().HasForeignKey("ExpenseId");

        builder.Property<Guid>("Id");
        builder.HasKey("Id");

        builder.Property(p => p.MemberId)
            .IsRequired();

        builder.Property(p => p.Percent)
            .HasPrecision(5, 2);

        builder.OwnsOne(p => p.Share, MoneyConfiguration.ConfigureOwned);

        builder.HasIndex("ExpenseId", nameof(SplitPart.MemberId));
    }
}
