using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("AspNetUsers");

		builder.Property(u => u.Name).HasColumnName(nameof(User.Name)).HasMaxLength(50).IsRequired();
		builder.Property(u => u.Surname).HasColumnName(nameof(User.Surname)).HasMaxLength(50).IsRequired();
		builder.Property(u => u.Password).HasColumnName(nameof(User.Password)).HasMaxLength(100).IsRequired();
		builder.Property(u => u.Address).HasColumnName(nameof(User.Address)).HasMaxLength(140).IsRequired();
		builder.Property(u => u.BirthDate).HasColumnName(nameof(User.BirthDate)).IsRequired();
		builder.Property(u => u.Gender).HasColumnName(nameof(User.Gender)).IsRequired();
		builder.Property(u => u.CreatedAt).HasColumnName(nameof(User.CreatedAt)).IsRequired();
		builder.Property(u => u.LastLogin).HasColumnName(nameof(User.LastLogin)).IsRequired();
		builder.Property(u => u.Photo).HasColumnName(nameof(User.Photo)).HasColumnType("image");
	}
}