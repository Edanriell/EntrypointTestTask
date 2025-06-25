using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.OrderId)
            .IsRequired();

        builder.OwnsOne(payment => payment.TotalAmount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(money => money.Currency)
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code))
                .IsRequired();
        });

        // builder.OwnsOne(payment => payment.TotalAmount,
        //     priceBuilder =>
        //     {
        //         priceBuilder.Property(money => money.Currency)
        //             .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        //     });

        builder.OwnsOne(payment => payment.PaidAmount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasColumnName("paid_amount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(money => money.Currency)
                .HasColumnName("paid_currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code))
                .IsRequired();
        });

        // builder.OwnsOne(payment => payment.PaidAmount,
        //     priceBuilder =>
        //     {
        //         priceBuilder.Property(money => money.Currency)
        //             .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        //     });

        builder.OwnsOne(payment => payment.OutstandingAmount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasColumnName("outstanding_amount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(money => money.Currency)
                .HasColumnName("outstanding_currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code))
                .IsRequired();
        });

        // builder.OwnsOne(payment => payment.OutstandingAmount,
        //     priceBuilder =>
        //     {
        //         priceBuilder.Property(money => money.Currency)
        //             .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        //     });

        builder.Property(payment => payment.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(payment => payment.PaymentCompletedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(payment => payment.Order)
            .WithOne(order => order.Payment)
            .HasForeignKey<Payment>(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(payment => payment.OrderId)
            .IsUnique();

        builder.HasIndex(payment => payment.PaymentStatus);
        builder.HasIndex(payment => payment.PaymentCompletedAt);
    }
}
