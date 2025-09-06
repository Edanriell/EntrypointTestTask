using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Payments;
using Server.Domain.Refunds;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.OwnsOne(payment => payment.Amount, moneyBuilder =>
        {
            moneyBuilder.Property(money => money.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            moneyBuilder.Property(money => money.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code))
                .IsRequired();
        });

        builder.Property(payment => payment.PaymentStatus)
            .HasConversion<string>()
            .HasColumnName("payment_status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(payment => payment.PaymentMethod)
            .HasConversion<string>()
            .HasColumnName("payment_method")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(payment => payment.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(payment => payment.PaymentCompletedAt)
            .HasColumnName("payment_completed_at")
            .IsRequired(false);

        builder.Property(payment => payment.PaymentFailedAt)
            .HasColumnName("payment_failed_at")
            .IsRequired(false);

        builder.Property(payment => payment.PaymentExpiredAt)
            .HasColumnName("payment_expired_at")
            .IsRequired(false);

        builder.Property(payment => payment.PaymentFailureReason)
            .HasConversion(
                reason => reason != null ? reason.Value : null,
                value => value != null ? PaymentFailureReason.Create(value) : null)
            .HasColumnName("payment_failure_reason")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasOne(payment => payment.Refund)
            .WithOne(refund => refund.Payment)
            .HasForeignKey<Refund>(refund => refund.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(payment => payment.OrderId)
            .HasDatabaseName("ix_payments_order_id");
        builder.HasIndex(payment => payment.PaymentStatus)
            .HasDatabaseName("ix_payments_payment_status");
        builder.HasIndex(payment => payment.CreatedAt)
            .HasDatabaseName("ix_payments_created_at");
    }
}
