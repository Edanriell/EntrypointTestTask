// Old way for creating many-to-many relationship
//
//using Domain.Entities;
// 
//namespace Infrastructure.Data.Configurations;
//
//public class ProductOrderLinkConfiguration : IEntityTypeConfiguration<ProductOrderLink>
//{
//	public void Configure(EntityTypeBuilder<ProductOrderLink> builder)
//	{
//		builder.HasKey(p => new { p.OrderId, p.ProductId });
//
//		builder
//		   .HasOne(p => p.Order)
//		   .WithMany(o => o.OrderProducts)
//		   .HasForeignKey(p => p.OrderId)
//		   .IsRequired()
//		   .OnDelete(DeleteBehavior.Cascade);
//
//		builder
//		   .HasOne(p => p.Product)
//		   .WithMany(p => p.ProductOrders)
//		   .HasForeignKey(p => p.ProductId)
//		   .IsRequired()
//		   .OnDelete(DeleteBehavior.Cascade);
//	}
//}

