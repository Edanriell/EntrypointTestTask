using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Orders;
using Server.Domain.Refunds;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.ClientId)
            .IsRequired();

        builder.Property(order => order.OrderNumber)
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                orderNumber => orderNumber.Value,
                value => OrderNumber.FromValue(value)
            );

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired()
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            );

        builder.OwnsOne(order => order.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("total_amount_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.OwnsOne(order => order.PaidAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("paid_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("paid_amount_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.OwnsOne(order => order.TotalRefundedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_refunded_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("total_refunded_amount_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.Property(order => order.Courier)
            .HasConversion<string>()
            .HasColumnName("courier")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(order => order.EstimatedDeliveryDate)
            .HasColumnName("estimated_delivery_date")
            .IsRequired(false);

        builder.Property<List<Guid>>("_paymentIds")
            .HasField("_paymentIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("payment_ids")
            .HasConversion(
                v => v != null && v.Any() ? string.Join(',', v) : string.Empty,
                v => string.IsNullOrEmpty(v)
                    ? new List<Guid>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Guid.Parse)
                        .ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.Property(order => order.HasPendingPayments)
            .HasDefaultValue(false);

        builder.Property(order => order.HasActivePayments)
            .HasDefaultValue(false);

        builder.Property(order => order.HasFailedPayments)
            .HasDefaultValue(false);

        builder.Property(order => order.HasDisputedPayments)
            .HasDefaultValue(false);

        builder.OwnsOne(order => order.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("shipping_address_street")
                .HasMaxLength(200)
                .IsRequired();

            addressBuilder.Property(a => a.City)
                .HasColumnName("shipping_address_city")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.Country)
                .HasColumnName("shipping_address_country")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.ZipCode)
                .HasColumnName("shipping_address_zipcode")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.ConfirmedAt)
            .IsRequired(false);

        builder.Property(order => order.ShippedAt)
            .IsRequired(false);

        builder.Property(order => order.DeliveredAt)
            .IsRequired(false);

        builder.Property(order => order.CancelledAt)
            .IsRequired(false);

        builder.Property(order => order.CancellationReason)
            .HasMaxLength(500)
            .IsRequired(false)
            .HasConversion(
                reason => reason != null ? reason.Value : null,
                value => value != null ? CancellationReason.Create(value).Value : null
            );

        builder.Property(order => order.ReturnReason)
            .HasMaxLength(500)
            .IsRequired(false)
            .HasConversion(
                reason => reason != null ? reason.Value : null,
                value => value != null ? ReturnReason.Create(value) : null
            );

        builder.Property(order => order.RefundReason)
            .HasMaxLength(500)
            .IsRequired(false)
            .HasConversion(
                reason => reason != null ? reason.Value : null,
                value => value != null ? RefundReason.Create(value).Value : null
            );

        builder.Property(order => order.Info)
            .HasMaxLength(500)
            .IsRequired(false)
            .HasConversion(
                info => info != null ? info.Value : null,
                value => value != null ? OrderInfo.Create(value) : null);

        builder.Property(order => order.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired(false)
            .HasConversion(
                trackingNumber => trackingNumber != null ? trackingNumber.Value : null,
                value => value != null ? TrackingNumber.Create(value).Value : null
            );

        builder.HasMany(order => order.OrderProducts)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(order => order.ClientId);
        builder.HasIndex(order => order.Status);
        builder.HasIndex(order => order.CreatedAt);
        builder.HasIndex(order => order.OrderNumber).IsUnique();
        builder.HasIndex(order => order.TrackingNumber).IsUnique();
    }
}
