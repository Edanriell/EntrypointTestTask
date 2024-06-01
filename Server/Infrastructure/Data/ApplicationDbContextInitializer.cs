using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var superAdministratorRole = new IdentityRole(Roles.SuperAdministrator);

        if (roleManager.Roles.All(r => r.Name != superAdministratorRole.Name))
            await roleManager.CreateAsync(superAdministratorRole);

        // Default users
        var superAdministrator = new User
        {
            Name = "SuperAdmin",
            Surname = "SuperAdmin",
            Email = "super-administrator@localhost",
            Password = "SuperAdministrator96!$",
            UserName = "SuperAdministrator",
            Address = "SuperAdminAddress",
            BirthDate = new DateTime(),
            Gender = Gender.Male
        };

        if (userManager.Users.All(u => u.UserName != superAdministrator.UserName))
        {
            await userManager.CreateAsync(superAdministrator, "SuperAdministrator96!$");
            if (!string.IsNullOrWhiteSpace(superAdministratorRole.Name))
                await userManager.AddToRolesAsync(superAdministrator, new[] { superAdministratorRole.Name });
        }

        // Default data
        // Seed, if necessary
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new()
                {
                    Code = "P001",
                    ProductName = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with customizable buttons",
                    UnitPrice = 25.99m,
                    UnitsInStock = 150,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P002",
                    ProductName = "Mechanical Keyboard",
                    Description = "RGB backlit mechanical keyboard with brown switches",
                    UnitPrice = 75.50m,
                    UnitsInStock = 100,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P003",
                    ProductName = "Gaming Headset",
                    Description = "Surround sound gaming headset with noise-cancelling mic",
                    UnitPrice = 59.99m,
                    UnitsInStock = 200,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P004",
                    ProductName = "4K Monitor",
                    Description = "27-inch 4K UHD monitor with HDR support",
                    UnitPrice = 299.99m,
                    UnitsInStock = 50,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P005",
                    ProductName = "External SSD",
                    Description = "1TB portable external SSD with USB-C connectivity",
                    UnitPrice = 129.99m,
                    UnitsInStock = 75,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P006",
                    ProductName = "Smartphone",
                    Description = "Latest model smartphone with 128GB storage and 8GB RAM",
                    UnitPrice = 699.99m,
                    UnitsInStock = 300,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P007",
                    ProductName = "Smartwatch",
                    Description = "Fitness tracking smartwatch with heart rate monitor",
                    UnitPrice = 199.99m,
                    UnitsInStock = 180,
                    UnitsOnOrder = 0
                },
                new()
                {
                    Code = "P008",
                    ProductName = "Bluetooth Speaker",
                    Description = "Portable Bluetooth speaker with deep bass and long battery life",
                    UnitPrice = 49.99m,
                    UnitsInStock = 250,
                    UnitsOnOrder = 0
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        if (!context.Orders.Any())
        {
            var orders = new List<Order>
            {
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-8),
                    Status = OrderStatus.Created,
                    ShipAddress = "123 Main Street",
                    OrderInformation = "Deliver during the day.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P001").Id, Quantity = 2 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P002").Id, Quantity = 1 }
                    ]
                },
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7),
                    Status = OrderStatus.PendingForPayment,
                    ShipAddress = "456 Elm Street",
                    OrderInformation = "Leave at the front door.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P003").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P004").Id, Quantity = 2 }
                    ]
                },
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-6),
                    Status = OrderStatus.Paid,
                    ShipAddress = "789 Oak Street",
                    OrderInformation = "Ring the bell.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P005").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P006").Id, Quantity = 1 }
                    ]
                },
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Status = OrderStatus.Delivered,
                    ShipAddress = "321 Pine Street",
                    OrderInformation = "Call upon arrival.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P007").Id, Quantity = 2 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P008").Id, Quantity = 1 }
                    ]
                },
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4),
                    Status = OrderStatus.Cancelled,
                    ShipAddress = "654 Maple Street",
                    OrderInformation = "No special instructions.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P001").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P003").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P005").Id, Quantity = 1 }
                    ]
                },
                new()
                {
                    UserId = superAdministrator.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    Status = OrderStatus.InTransit,
                    ShipAddress = "987 Cedar Street",
                    OrderInformation = "Leave with the neighbor.",
                    OrderProducts =
                    [
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P002").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P004").Id, Quantity = 1 },
                        new ProductOrderLink
                            { ProductId = context.Products.First(p => p.Code == "P006").Id, Quantity = 1 }
                    ]
                }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }
}

// Password field must be removed !
// Just for development reasons it is here