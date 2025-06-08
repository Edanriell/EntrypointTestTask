using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.ToTable("Orders");
		builder.HasKey(o => o.Id);

		builder.Property(o => o.Id).HasColumnName(nameof(Order.Id)).IsRequired();
		builder.Property(o => o.CreatedAt).HasColumnName(nameof(Order.CreatedAt)).IsRequired();
		builder.Property(o => o.UpdatedAt).HasColumnName(nameof(Order.UpdatedAt)).IsRequired();
		builder.Property(o => o.Status).HasColumnName(nameof(Order.Status)).HasDefaultValue(OrderStatus.Created)
		   .IsRequired();
		builder.Property(o => o.ShippingName).HasColumnName(nameof(Order.ShippingName)).HasMaxLength(100).IsRequired();
		builder.Property(o => o.AdditionalInformation).HasColumnName(nameof(Order.AdditionalInformation))
		   .HasMaxLength(400);
		builder.Property(o => o.UserId).HasColumnName(nameof(Order.UserId)).IsRequired();


		builder.OwnsOne(o => o.ShippingAddress, sa =>
		{
			sa.Property(s => s.Street).HasColumnName(nameof(Address.Street)).HasMaxLength(50).IsRequired();
			sa.Property(s => s.City).HasColumnName(nameof(Address.City)).HasMaxLength(50).IsRequired();
			sa.Property(s => s.Region).HasColumnName(nameof(Address.Region)).HasMaxLength(50);
			sa.Property(s => s.PostalCode).HasColumnName(nameof(Address.PostalCode)).HasMaxLength(18);
			sa.Property(s => s.Country).HasColumnName(nameof(Address.Country)).HasMaxLength(50).IsRequired();
		});

		builder.OwnsOne(o => o.BillingAddress, ba =>
		{
			ba.Property(b => b.Street).HasColumnName(nameof(Address.Street)).HasMaxLength(50).IsRequired();
			ba.Property(b => b.City).HasColumnName(nameof(Address.City)).HasMaxLength(50).IsRequired();
			ba.Property(b => b.Region).HasColumnName(nameof(Address.Region)).HasMaxLength(50);
			ba.Property(b => b.PostalCode).HasColumnName(nameof(Address.PostalCode)).HasMaxLength(18);
			ba.Property(b => b.Country).HasColumnName(nameof(Address.Country)).HasMaxLength(50).IsRequired();
		});

		builder
		   .HasOne(o => o.User)
		   .WithMany(u => u.Orders)
		   .HasForeignKey(o => o.UserId)
		   .IsRequired()
		   .OnDelete(DeleteBehavior.Cascade);

		builder
		   .HasMany(o => o.Payments)
		   .WithOne(p => p.Order)
		   .HasForeignKey(p => p.OrderId)
		   .OnDelete(DeleteBehavior.Cascade);

		// New way for creating many-to-many relationship
		builder.HasMany(o => o.Products)
		   .WithMany(p => p.Orders)
		   .UsingEntity<ProductOrderLink>(
					j => j
					   .HasOne(pol => pol.Product)
					   .WithMany(p => p.ProductOrders)
					   .HasForeignKey(pol => pol.ProductId),
					k => k
					   .HasOne(pol => pol.Order)
					   .WithMany(o => o.OrderProducts)
					   .HasForeignKey(pol => pol.OrderId),
					l =>
					{
						// We can add more additional configuration here
						l.Property(po => po.Quantity).HasColumnName("Quantity");
						l.HasKey(po => new { po.ProductId, po.OrderId });
					}
				);
	}
}