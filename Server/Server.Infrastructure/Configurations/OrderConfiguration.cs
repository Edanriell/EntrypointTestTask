using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Orders;

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

        builder.Property(order => order.TrackingNumber)
            .HasMaxLength(100)
            .IsRequired(false)
            .HasConversion(
                trackingNumber => trackingNumber != null ? trackingNumber.Value : null,
                value => value != null ? TrackingNumber.Create(value).Value : null
            );

        // Relationships
        builder.HasOne(order => order.Client)
            .WithMany(client => client.Orders)
            .HasForeignKey(order => order.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure one-to-many relationship with Payments
        builder.HasMany(order => order.Payments)
            .WithOne(payment => payment.Order)
            .HasForeignKey(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationship with Products through OrderProducts
        builder.HasMany(order => order.OrderProducts)
            .WithOne(op => op.Order)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(order => order.ClientId);
        builder.HasIndex(order => order.Status);
        builder.HasIndex(order => order.CreatedAt);
        builder.HasIndex(order => order.OrderNumber).IsUnique();
        builder.HasIndex(order => order.TrackingNumber).IsUnique();
        builder.HasIndex(order => order.ClientId)
            .HasDatabaseName("ix_orders_client_id");
        builder.HasIndex(order => order.Status)
            .HasDatabaseName("ix_orders_status");
        builder.HasIndex(order => order.CreatedAt)
            .HasDatabaseName("ix_orders_created_at");
        builder.HasIndex(order => new { order.ClientId, order.CreatedAt })
            .HasDatabaseName("ix_orders_client_id_created_at");
        builder.HasIndex(order => new { order.ClientId, order.Status })
            .HasDatabaseName("ix_orders_client_id_status");
        builder.HasIndex(order => new { order.CreatedAt, order.Status })
            .HasDatabaseName("ix_orders_created_at_status");
        builder.HasIndex(order => new { order.ClientId, order.Status, order.CreatedAt })
            .HasDatabaseName("ix_orders_client_id_status_created_at");
    }
}
