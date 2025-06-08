using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("Products");
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id).HasColumnName(nameof(Product.Id)).IsRequired();
		builder.Property(p => p.Code).HasColumnName(nameof(Product.Code)).HasMaxLength(50).IsRequired();
		builder.Property(p => p.Name).HasColumnName(nameof(Product.Name)).HasMaxLength(100).IsRequired();
		builder.Property(p => p.Description).HasColumnName(nameof(Product.Description)).HasMaxLength(324);
		builder.Property(p => p.UnitPrice).HasColumnName(nameof(Product.UnitPrice)).HasColumnType("decimal(18, 2)")
		   .IsRequired();
		builder.Property(p => p.UnitsInStock).HasColumnName(nameof(Product.UnitsInStock)).IsRequired();
		builder.Property(p => p.UnitsOnOrder).HasColumnName(nameof(Product.UnitsOnOrder)).IsRequired();
		builder.Property(p => p.Brand).HasColumnName(nameof(Product.Brand)).HasMaxLength(80).IsRequired();
		builder.Property(p => p.Rating).HasColumnName(nameof(Product.Rating)).IsRequired();
		builder.Property(p => p.Photo).HasColumnName(nameof(Product.Photo)).HasColumnType("image");

		builder.HasMany(p => p.Categories)
		   .WithMany(c => c.Products)
		   .UsingEntity<ProductCategoryLink>(
					j => j
					   .HasOne(pcl => pcl.Category)
					   .WithMany(c => c.CategoryProducts)
					   .HasForeignKey(pcl => pcl.CategoryId),
					k => k
					   .HasOne(pcl => pcl.Product)
					   .WithMany(p => p.ProductCategories)
					   .HasForeignKey(pcl => pcl.ProductId),
					pcl => { pcl.HasKey(l => new { l.ProductId, l.CategoryId }); }
				);
	}
}