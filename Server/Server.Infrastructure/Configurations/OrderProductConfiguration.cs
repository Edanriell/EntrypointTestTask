using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.OrderProducts;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> builder)
    {
        builder.ToTable("order_products");

        builder.HasKey(op => op.Id);

        builder.Property(op => op.OrderId)
            .IsRequired();

        builder.Property(op => op.ProductId)
            .IsRequired();

        builder.Property(op => op.ProductName)
            .HasMaxLength(300)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                value => ProductName.Create(value).Value
            );

        builder.OwnsOne(op => op.UnitPrice, priceBuilder =>
        {
            priceBuilder.Property(money => money.Amount)
                .HasColumnName("unit_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            priceBuilder.Property(money => money.Currency)
                .HasColumnName("unit_price_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.OwnsOne(op => op.UnitPrice,
            priceBuilder =>
            {
                priceBuilder.Property(money => money.Currency)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

        builder.OwnsOne(op => op.TotalPrice, priceBuilder =>
        {
            priceBuilder.Property(money => money.Amount)
                .HasColumnName("total_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            priceBuilder.Property(money => money.Currency)
                .HasColumnName("total_price_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.Property(op => op.Quantity)
            .IsRequired()
            .HasConversion(
                quantity => quantity.Value,
                value => Quantity.CreateQuantity(value).Value
            );

        // Relationships
        builder.HasOne(op => op.Order)
            .WithMany(order => order.OrderProducts)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(op => op.Product)
            .WithMany()
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(op => op.OrderId);
        builder.HasIndex(op => op.ProductId);
        builder.HasIndex(op => new { op.OrderId, op.ProductId }).IsUnique();
        builder.HasIndex(op => op.OrderId)
            .HasDatabaseName("ix_order_products_order_id");
        builder.HasIndex(op => op.ProductId)
            .HasDatabaseName("ix_order_products_product_id");
        builder.HasIndex(op => op.OrderId)
            .HasDatabaseName("ix_order_products_order_id_for_totals");
    }
}
