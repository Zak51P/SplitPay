using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitPay.Domain.ValueObjects;

namespace SplitPay.Infrastructure.Persistence.Configurations;

internal static class MoneyConfiguration
{
    public static void ConfigureOwned<TOwner>(OwnedNavigationBuilder<TOwner, Money> builder)
        where TOwner : class
    {
        builder.Property(m => m.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.Currency)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsRequired();
    }
}
