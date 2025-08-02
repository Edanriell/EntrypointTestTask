using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("refunds");

        builder.HasKey(refund => refund.Id);

        builder.Property(refund => refund.PaymentId)
            .HasColumnName("payment_id")
            .IsRequired();

        builder.OwnsOne(refund => refund.Amount, moneyBuilder =>
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

        builder.Property(refund => refund.Reason)
            .HasConversion(
                reason => reason.Value,
                value => RefundReason.Create(value).Value)
            .HasColumnName("refund_reason")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(refund => refund.Status)
            .HasConversion<string>()
            .HasColumnName("refund_status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(refund => refund.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(refund => refund.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired(false);

        builder.Property(refund => refund.RefundReference)
            .HasColumnName("refund_reference")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(refund => refund.RefundFailureReason)
            .HasColumnName("refund_failure_reason")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasIndex(refund => refund.PaymentId)
            .HasDatabaseName("ix_refunds_payment_id");
        builder.HasIndex(refund => refund.Status)
            .HasDatabaseName("ix_refunds_status");
        builder.HasIndex(refund => refund.CreatedAt)
            .HasDatabaseName("ix_refunds_created_at");
    }
}
 
