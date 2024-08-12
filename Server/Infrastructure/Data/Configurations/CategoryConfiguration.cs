using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
	public void Configure(EntityTypeBuilder<Category> builder)
	{
		builder.ToTable("Categories");
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id).HasColumnName(nameof(Category.Id)).IsRequired();
		builder.Property(c => c.Name).HasColumnName(nameof(Category.Name)).HasMaxLength(72).IsRequired();
		builder.Property(c => c.Description).HasColumnName(nameof(Category.Description)).HasMaxLength(500).IsRequired();
	}
}