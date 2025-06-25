using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Infrastructure.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(300)
            .IsRequired()
            .HasConversion(
                name => name.Value,
                value => ProductName.Create(value).Value
            );

        builder.Property(product => product.Description)
            .HasMaxLength(2000)
            .IsRequired()
            .HasConversion(
                description => description.Value,
                value => ProductDescription.Create(value).Value
            );

        // TODO 
        // Needs testing
        // builder.OwnsOne(product => product.Price,
        //     priceBuilder =>
        //     {
        //         priceBuilder.Property(money => money.Currency)
        //             .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        //     });

        builder.OwnsOne(product => product.Price, priceBuilder =>
        {
            priceBuilder.Property(money => money.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            priceBuilder.Property(money => money.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code)
                );
        });

        builder.Property(product => product.Reserved)
            .IsRequired()
            .HasConversion(
                reserved => reserved.Value,
                value => Quantity.CreateQuantity(value).Value
            );

        builder.Property(product => product.Stock)
            .IsRequired()
            .HasConversion(
                stock => stock.Value,
                value => Quantity.CreateQuantity(value).Value
            );

        builder.Property(product => product.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.LastUpdatedAt)
            .IsRequired();

        builder.Property(product => product.LastRestockedAt)
            .IsRequired(false);

        // Indexes for better query performance
        builder.HasIndex(product => product.Name).IsUnique();
        builder.HasIndex(product => product.Status);
        builder.HasIndex(product => product.CreatedAt);
        builder.HasIndex(product => product.LastUpdatedAt);

        // Composite index for commonly queried combinations
        builder.HasIndex(product => new { product.Status, product.Stock });
    }
}
