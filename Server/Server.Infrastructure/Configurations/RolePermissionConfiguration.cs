using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Domain.Users;

namespace Server.Infrastructure.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(
            "role_permissions"
        );

        builder.HasKey(rolePermission => new
            {
                rolePermission.RoleId,
                rolePermission.PermissionId
            }
        );

        builder.HasData(
            // Admin - Full access to everything
            new RolePermission { RoleId = Role.Admin.Id, PermissionId = Permission.UsersManage.Id },
            new RolePermission { RoleId = Role.Admin.Id, PermissionId = Permission.ClientsManage.Id },
            new RolePermission { RoleId = Role.Admin.Id, PermissionId = Permission.ProductsManage.Id },
            new RolePermission { RoleId = Role.Admin.Id, PermissionId = Permission.OrdersManage.Id },
            new RolePermission { RoleId = Role.Admin.Id, PermissionId = Permission.PaymentsManage.Id },

            // Manager - Read/Write access to most areas
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.UsersRead.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.UsersWrite.Id }, // ✅ Fixed
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.ClientsRead.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.ClientsWrite.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.ProductsRead.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.ProductsWrite.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.OrdersRead.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.OrdersWrite.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.OrdersProcess.Id },
            new RolePermission { RoleId = Role.Manager.Id, PermissionId = Permission.PaymentsRead.Id },

            // User Manager - User and Client management
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.UsersRead.Id },
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.UsersWrite.Id },
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.UsersDelete.Id },
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.ClientsRead.Id },
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.ClientsWrite.Id },
            new RolePermission { RoleId = Role.UserManager.Id, PermissionId = Permission.ClientsDelete.Id },

            // Product Manager - Product management
            new RolePermission { RoleId = Role.ProductManager.Id, PermissionId = Permission.ProductsRead.Id },
            new RolePermission { RoleId = Role.ProductManager.Id, PermissionId = Permission.ProductsWrite.Id },
            new RolePermission { RoleId = Role.ProductManager.Id, PermissionId = Permission.ProductsDelete.Id },

            // Order Manager - Order management + Client read access
            new RolePermission { RoleId = Role.OrderManager.Id, PermissionId = Permission.OrdersRead.Id },
            new RolePermission { RoleId = Role.OrderManager.Id, PermissionId = Permission.OrdersWrite.Id },
            new RolePermission { RoleId = Role.OrderManager.Id, PermissionId = Permission.OrdersDelete.Id },
            new RolePermission { RoleId = Role.OrderManager.Id, PermissionId = Permission.OrdersProcess.Id },
            new RolePermission { RoleId = Role.OrderManager.Id, PermissionId = Permission.ClientsRead.Id },

            // Payment Manager - Payment management + Order read access
            new RolePermission { RoleId = Role.PaymentManager.Id, PermissionId = Permission.PaymentsRead.Id },
            new RolePermission { RoleId = Role.PaymentManager.Id, PermissionId = Permission.PaymentsWrite.Id },
            new RolePermission { RoleId = Role.PaymentManager.Id, PermissionId = Permission.PaymentsRefund.Id },
            new RolePermission { RoleId = Role.PaymentManager.Id, PermissionId = Permission.OrdersRead.Id },

            // Customer - Limited access for customers
            new RolePermission { RoleId = Role.Customer.Id, PermissionId = Permission.ProductsRead.Id },
            new RolePermission { RoleId = Role.Customer.Id, PermissionId = Permission.OrdersRead.Id },
            new RolePermission { RoleId = Role.Customer.Id, PermissionId = Permission.OrdersWrite.Id },
            new RolePermission { RoleId = Role.Customer.Id, PermissionId = Permission.PaymentsRead.Id },
 
            // Guest - Minimal access
            new RolePermission { RoleId = Role.Guest.Id, PermissionId = Permission.ProductsRead.Id }
        );
    }
}
