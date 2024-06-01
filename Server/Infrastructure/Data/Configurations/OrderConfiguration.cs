using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // New way for creating many-to-many relationship
        builder.HasMany(o => o.Products)
            .WithMany(p => p.Orders)
            .UsingEntity<ProductOrderLink>(
                j => j
                    .HasOne(po => po.Product)
                    .WithMany(o => o.ProductOrders)
                    .HasForeignKey(ma => ma.ProductId),
                o => o
                    .HasOne(pa => pa.Order)
                    .WithMany(o => o.OrderProducts)
                    .HasForeignKey(ma => ma.OrderId),
                k =>
                {
                    // We can add more additional configuration here
                    k.Property(po => po.Quantity).HasColumnName("Quantity");
                    k.HasKey(po => new { po.ProductId, po.OrderId });
                }
            );
    }
}