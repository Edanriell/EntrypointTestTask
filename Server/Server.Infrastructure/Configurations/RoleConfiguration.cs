using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Users;

namespace Server.Infrastructure.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.HasMany(role => role.Permissions).WithMany().UsingEntity<RolePermission>();

        // Seed roles
        builder.HasData(
            Role.Admin,
            Role.Manager,
            Role.OrderManager,
            Role.ProductManager,
            Role.UserManager,
            Role.PaymentManager,
            Role.Customer,
            Role.Guest
        );

        builder.HasIndex(role => role.Name)
            .HasDatabaseName("ix_roles_name");
    }
}
