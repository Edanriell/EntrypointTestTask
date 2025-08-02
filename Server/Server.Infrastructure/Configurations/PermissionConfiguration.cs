using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Users;

namespace Server.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
 
        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Id)
            .ValueGeneratedNever();

        builder.Property(permission => permission.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(permission => permission.Name)
            .IsUnique();

        builder.HasData(
            // User Management
            Permission.UsersRead,
            Permission.UsersWrite,
            Permission.UsersDelete,
            Permission.UsersManage,

            // Client Management
            Permission.ClientsRead,
            Permission.ClientsWrite,
            Permission.ClientsDelete,
            Permission.ClientsManage,

            // Product Management
            Permission.ProductsRead,
            Permission.ProductsWrite,
            Permission.ProductsDelete,
            Permission.ProductsManage,

            // Order Management
            Permission.OrdersRead,
            Permission.OrdersWrite,
            Permission.OrdersDelete,
            Permission.OrdersProcess,
            Permission.OrdersManage,

            // Payment Management
            Permission.PaymentsRead,
            Permission.PaymentsWrite,
            Permission.PaymentsRefund,
            Permission.PaymentsManage
        );
    }
}
 
