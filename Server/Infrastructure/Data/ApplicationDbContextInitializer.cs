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
	ApplicationDbContext                     context,
	UserManager<User>                        userManager,
	RoleManager<IdentityRole>                roleManager)
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
									 Name      = "SuperAdmin",
									 Surname   = "SuperAdmin",
									 Email     = "super-administrator@localhost",
									 Password  = "SuperAdministrator96!$",
									 UserName  = "SuperAdministrator",
									 Address   = "SuperAdminAddress",
									 BirthDate = new DateTime(),
									 Gender    = Gender.Male
								 };

		if (userManager.Users.All(u => u.UserName != superAdministrator.UserName))
		{
			await userManager.CreateAsync(superAdministrator, "SuperAdministrator96!$");
			if (!string.IsNullOrWhiteSpace(superAdministratorRole.Name))
				await userManager.AddToRolesAsync(superAdministrator, new[] { superAdministratorRole.Name });
		}

		// Default data
		// Seed, if necessary
		if (!context.Categories.Any())
		{
			var categories = new List<Category>
							 {
								 new()
								 {
									 Name        = "Electronics",
									 Description = "Devices and gadgets for daily use"
								 },
								 new()
								 {
									 Name        = "Home Appliances",
									 Description = "Essential appliances for home use"
								 },
								 new()
								 {
									 Name        = "Books",
									 Description = "Wide range of books and literature"
								 },
								 new()
								 {
									 Name        = "Clothing",
									 Description = "Apparel and fashion items for all ages"
								 },
								 new()
								 {
									 Name        = "Sports & Outdoors",
									 Description = "Gear and equipment for outdoor activities and sports"
								 },
								 new()
								 {
									 Name        = "Health & Beauty",
									 Description = "Products for personal care, wellness, and beauty"
								 },
								 new()
								 {
									 Name        = "Toys & Games",
									 Description = "Fun and entertainment for children and adults"
								 },
								 new()
								 {
									 Name        = "Automotive",
									 Description = "Car accessories and automotive products"
								 },
								 new()
								 {
									 Name        = "Furniture",
									 Description = "Furniture and home decor items"
								 },
								 new()
								 {
									 Name        = "Grocery & Gourmet Food",
									 Description = "Food, beverages, and gourmet items"
								 },
								 new()
								 {
									 Name        = "Jewelry",
									 Description = "Jewelry and accessories for all occasions"
								 },
								 new()
								 {
									 Name        = "Pet Supplies",
									 Description = "Products for pets and pet care"
								 },
								 new()
								 {
									 Name        = "Garden & Outdoor",
									 Description = "Tools, plants, and accessories for gardening and outdoor spaces"
								 },
								 new()
								 {
									 Name        = "Office Supplies",
									 Description = "Office equipment and supplies"
								 },
								 new()
								 {
									 Name        = "Baby Products",
									 Description = "Products and accessories for babies and toddlers"
								 },
								 new()
								 {
									 Name        = "Musical Instruments",
									 Description = "Instruments and accessories for music lovers"
								 },
								 new()
								 {
									 Name        = "Tools & Home Improvement",
									 Description = "Tools, hardware, and home improvement products"
								 },
								 new()
								 {
									 Name        = "Video Games",
									 Description = "Games and gaming accessories for consoles and PCs"
								 },
								 new()
								 {
									 Name        = "Movies & TV",
									 Description = "DVDs, Blu-rays, and streaming options for movies and TV shows"
								 },
								 new()
								 {
									 Name        = "Arts, Crafts & Sewing",
									 Description = "Supplies for crafting, sewing, and other creative projects"
								 },
								 new()
								 {
									 Name        = "Industrial & Scientific",
									 Description = "Equipment and supplies for industrial and scientific purposes"
								 },
								 new()
								 {
									 Name        = "Luggage & Travel Gear",
									 Description = "Travel accessories and luggage"
								 },
								 new()
								 {
									 Name        = "Collectibles & Fine Art",
									 Description = "Art pieces and collectibles"
								 }
							 };

			context.Categories.AddRange(categories);
			await context.SaveChangesAsync();
		}

		if (!context.Products.Any())
		{
			var products = new List<Product>
						   {
							   new()
							   {
								   Code = "EL001",
								   Name = "Smartphone X12",
								   Description =
									   "Latest generation smartphone with high-resolution display and advanced camera features.",
								   UnitPrice    = 799.99M,
								   UnitsInStock = 150,
								   UnitsOnOrder = 50,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Electronics").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "EL002",
								   Name = "Noise Cancelling Headphones",
								   Description =
									   "Premium headphones with active noise cancellation and superior sound quality.",
								   UnitPrice    = 199.99M,
								   UnitsInStock = 200,
								   UnitsOnOrder = 100,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Electronics").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "HA001",
								   Name = "Air Conditioner",
								   Description =
									   "Energy-efficient air conditioner with fast cooling and remote control.",
								   UnitPrice    = 499.99M,
								   UnitsInStock = 80,
								   UnitsOnOrder = 20,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Home Appliances").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "BK001",
								   Name         = "The Art of Coding",
								   Description  = "Comprehensive guide to coding best practices and design patterns.",
								   UnitPrice    = 39.99M,
								   UnitsInStock = 300,
								   UnitsOnOrder = 0,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Books").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "CL001",
								   Name         = "Men's Leather Jacket",
								   Description  = "Stylish and durable leather jacket for all seasons.",
								   UnitPrice    = 149.99M,
								   UnitsInStock = 50,
								   UnitsOnOrder = 10,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Clothing").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "SP001",
								   Name         = "Mountain Bike",
								   Description  = "High-performance mountain bike suitable for all terrains.",
								   UnitPrice    = 699.99M,
								   UnitsInStock = 30,
								   UnitsOnOrder = 10,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Sports & Outdoors").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "HB001",
								   Name         = "Herbal Shampoo",
								   Description  = "Natural herbal shampoo for healthy and shiny hair.",
								   UnitPrice    = 14.99M,
								   UnitsInStock = 200,
								   UnitsOnOrder = 50,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Health & Beauty").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "TG001",
								   Name = "Board Game - Adventure Quest",
								   Description =
									   "Exciting board game for families and friends, with multiple levels of challenges.",
								   UnitPrice    = 49.99M,
								   UnitsInStock = 120,
								   UnitsOnOrder = 30,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Toys & Games").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "AU001",
								   Name         = "Car GPS Navigator",
								   Description  = "Advanced GPS navigation system with real-time traffic updates.",
								   UnitPrice    = 129.99M,
								   UnitsInStock = 70,
								   UnitsOnOrder = 20,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Automotive").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "FR001",
								   Name         = "Modern Sofa",
								   Description  = "Comfortable and stylish sofa with premium fabric and cushions.",
								   UnitPrice    = 899.99M,
								   UnitsInStock = 20,
								   UnitsOnOrder = 5,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Furniture").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "GF001",
								   Name         = "Gourmet Chocolate Box",
								   Description  = "Assorted gourmet chocolates made from high-quality ingredients.",
								   UnitPrice    = 29.99M,
								   UnitsInStock = 250,
								   UnitsOnOrder = 100,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.First(c =>
																   c.Name == "Grocery & Gourmet Food").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "JW001",
								   Name         = "Gold Necklace",
								   Description  = "Elegant gold necklace with a minimalist design.",
								   UnitPrice    = 299.99M,
								   UnitsInStock = 40,
								   UnitsOnOrder = 10,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Jewelry").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "PS001",
								   Name         = "Pet Toy - Chew Bone",
								   Description  = "Durable chew toy for dogs, made from safe and non-toxic materials.",
								   UnitPrice    = 9.99M,
								   UnitsInStock = 500,
								   UnitsOnOrder = 150,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Pet Supplies").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "GO001",
								   Name = "Garden Hose",
								   Description =
									   "Flexible and durable garden hose, suitable for all weather conditions.",
								   UnitPrice    = 24.99M,
								   UnitsInStock = 300,
								   UnitsOnOrder = 50,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Garden & Outdoor").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "OS001",
								   Name         = "Office Chair",
								   Description  = "Ergonomic office chair with adjustable height and lumbar support.",
								   UnitPrice    = 199.99M,
								   UnitsInStock = 100,
								   UnitsOnOrder = 20,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Office Supplies").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "BP001",
								   Name         = "Baby Stroller",
								   Description  = "Lightweight and compact stroller with multiple recline positions.",
								   UnitPrice    = 249.99M,
								   UnitsInStock = 60,
								   UnitsOnOrder = 15,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Baby Products").Id
														   }
													   }
							   },
							   new()
							   {
								   Code         = "MI001",
								   Name         = "Acoustic Guitar",
								   Description  = "High-quality acoustic guitar with a rich and full sound.",
								   UnitPrice    = 349.99M,
								   UnitsInStock = 80,
								   UnitsOnOrder = 25,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.First(c =>
																   c.Name == "Musical Instruments").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "TI001",
								   Name = "Cordless Drill",
								   Description =
									   "Powerful cordless drill with multiple speed settings and long-lasting battery.",
								   UnitPrice    = 129.99M,
								   UnitsInStock = 150,
								   UnitsOnOrder = 40,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.First(c =>
																   c.Name == "Tools & Home Improvement").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "VG001",
								   Name = "Action RPG Game",
								   Description =
									   "Immersive action role-playing game with stunning graphics and a gripping storyline.",
								   UnitPrice    = 59.99M,
								   UnitsInStock = 500,
								   UnitsOnOrder = 100,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .First(c => c.Name == "Video Games").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "MT001",
								   Name = "Blu-ray Movie Collection",
								   Description =
									   "Collection of critically acclaimed movies in high-definition Blu-ray format.",
								   UnitPrice    = 79.99M,
								   UnitsInStock = 100,
								   UnitsOnOrder = 30,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories
																  .Single(c => c.Name == "Movies & TV").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "AC001",
								   Name = "Watercolor Paint Set",
								   Description =
									   "Professional-grade watercolor paints with a wide range of vibrant colors.",
								   UnitPrice    = 39.99M,
								   UnitsInStock = 120,
								   UnitsOnOrder = 25,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.Single(c =>
																   c.Name == "Arts, Crafts & Sewing").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "IS001",
								   Name = "Digital Microscope",
								   Description =
									   "High-precision digital microscope for scientific research and educational use.",
								   UnitPrice    = 499.99M,
								   UnitsInStock = 30,
								   UnitsOnOrder = 10,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.Single(c =>
																   c.Name == "Industrial & Scientific").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "LT001",
								   Name = "Travel Backpack",
								   Description =
									   "Durable and spacious backpack, perfect for travel and outdoor adventures.",
								   UnitPrice    = 89.99M,
								   UnitsInStock = 180,
								   UnitsOnOrder = 50,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.Single(c =>
																   c.Name == "Luggage & Travel Gear").Id
														   }
													   }
							   },
							   new()
							   {
								   Code = "CF001",
								   Name = "Limited Edition Collectible Figure",
								   Description =
									   "Highly detailed collectible figure, limited to a small production run.",
								   UnitPrice    = 129.99M,
								   UnitsInStock = 20,
								   UnitsOnOrder = 5,
								   ProductCategories = new List<ProductCategoryLink>
													   {
														   new()
														   {
															   CategoryId = context.Categories.Single(c =>
																   c.Name == "Collectibles & Fine Art").Id
														   }
													   }
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
								 CreatedAt    = DateTime.UtcNow.AddDays(-10),
								 UpdatedAt    = DateTime.UtcNow.AddDays(-9),
								 Status       = OrderStatus.Created,
								 ShippingName = "John Doe",
								 ShippingAddress = new Address
												   {
													   Street     = "123 Elm Street",
													   City       = "Metropolis",
													   Region     = "Central",
													   PostalCode = "12345",
													   Country    = "USA"
												   },
								 BillingAddress = new Address
												  {
													  Street     = "123 Elm Street",
													  City       = "Metropolis",
													  Region     = "Central",
													  PostalCode = "12345",
													  Country    = "USA"
												  },
								 AdditionalInformation = "Leave at the front door if not at home.",
								 UserId                = superAdministrator.Id,
								 OrderProducts = new List<ProductOrderLink>
												 {
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "EL001").Id,
														 Quantity = 2
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "EL002").Id,
														 Quantity = 1
													 }
												 },
								 Payments = new List<Payment>
											{
												new()
												{
													Amount = 1799.97M, // EL001 * 2 + EL002 * 1
													PaymentDate = DateTime.UtcNow.AddDays(-9),
													PaymentMethod =
														PaymentMethod.CreditCard, // Ensure PaymentMethod is an enum
													TransactionId = "TX1234567890", // Example transaction ID
													OrderId       = 1 // Optional, or you can set this later
												}
											}
							 },
							 new()
							 {
								 CreatedAt    = DateTime.UtcNow.AddDays(-8),
								 UpdatedAt    = DateTime.UtcNow.AddDays(-7),
								 Status       = OrderStatus.Paid,
								 ShippingName = "Jane Smith",
								 ShippingAddress = new Address
												   {
													   Street     = "456 Oak Avenue",
													   City       = "Gotham",
													   Region     = "Northern",
													   PostalCode = "54321",
													   Country    = "USA"
												   },
								 BillingAddress = new Address
												  {
													  Street     = "456 Oak Avenue",
													  City       = "Gotham",
													  Region     = "Northern",
													  PostalCode = "54321",
													  Country    = "USA"
												  },
								 AdditionalInformation = "Deliver after 5 PM.",
								 UserId                = superAdministrator.Id,
								 OrderProducts = new List<ProductOrderLink>
												 {
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "BK001").Id,
														 Quantity = 1
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "CL001").Id,
														 Quantity = 2
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "SP001").Id,
														 Quantity = 1
													 }
												 },
								 Payments = new List<Payment>
											{
												new()
												{
													Amount        = 1089.96M, // BK001 * 1 + CL001 * 2 + SP001 * 1
													PaymentDate   = DateTime.UtcNow.AddDays(-7),
													PaymentMethod = PaymentMethod.BankTransfer,
													TransactionId = "TX0987654321", // Example transaction ID
													OrderId       = 2 // Optional, or you can set this later
												}
											}
							 },
							 new()
							 {
								 CreatedAt    = DateTime.UtcNow.AddDays(-5),
								 UpdatedAt    = DateTime.UtcNow.AddDays(-4),
								 Status       = OrderStatus.InTransit,
								 ShippingName = "Alice Johnson",
								 ShippingAddress = new Address
												   {
													   Street     = "789 Pine Road",
													   City       = "Star City",
													   Region     = "Western",
													   PostalCode = "67890",
													   Country    = "USA"
												   },
								 BillingAddress = new Address
												  {
													  Street     = "789 Pine Road",
													  City       = "Star City",
													  Region     = "Western",
													  PostalCode = "67890",
													  Country    = "USA"
												  },
								 AdditionalInformation = "Contact via phone upon arrival.",
								 UserId                = superAdministrator.Id,
								 OrderProducts = new List<ProductOrderLink>
												 {
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "HB001").Id,
														 Quantity = 3
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "TG001").Id,
														 Quantity = 1
													 }
												 },
								 Payments = new List<Payment>
											{
												new()
												{
													Amount        = 94.96M, // HB001 * 3 + TG001 * 1
													PaymentDate   = DateTime.UtcNow.AddDays(-4),
													PaymentMethod = PaymentMethod.CreditCard,
													TransactionId = "TX1122334455", // Example transaction ID
													OrderId       = 3 // Optional, or you can set this later
												}
											}
							 },
							 new()
							 {
								 CreatedAt    = DateTime.UtcNow.AddDays(-3),
								 UpdatedAt    = DateTime.UtcNow.AddDays(-2),
								 Status       = OrderStatus.Delivered,
								 ShippingName = "Robert Brown",
								 ShippingAddress = new Address
												   {
													   Street     = "101 Maple Lane",
													   City       = "Central City",
													   Region     = "Eastern",
													   PostalCode = "45678",
													   Country    = "USA"
												   },
								 BillingAddress = new Address
												  {
													  Street     = "101 Maple Lane",
													  City       = "Central City",
													  Region     = "Eastern",
													  PostalCode = "45678",
													  Country    = "USA"
												  },
								 AdditionalInformation = "Ring the doorbell twice.",
								 UserId                = superAdministrator.Id,
								 OrderProducts = new List<ProductOrderLink>
												 {
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "AU001").Id,
														 Quantity = 1
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "FR001").Id,
														 Quantity = 2
													 }
												 },
								 Payments = new List<Payment>
											{
												new()
												{
													Amount        = 1829.97M, // AU001 * 1 + FR001 * 2
													PaymentDate   = DateTime.UtcNow.AddDays(-2),
													PaymentMethod = PaymentMethod.BankTransfer,
													TransactionId = "TX5566778899", // Example transaction ID
													OrderId       = 4 // Optional, or you can set this later
												}
											}
							 },
							 new()
							 {
								 CreatedAt    = DateTime.UtcNow.AddDays(-1),
								 UpdatedAt    = DateTime.UtcNow,
								 Status       = OrderStatus.PendingForPayment,
								 ShippingName = "Emily White",
								 ShippingAddress = new Address
												   {
													   Street     = "202 Birch Street",
													   City       = "Coast City",
													   Region     = "Southern",
													   PostalCode = "34567",
													   Country    = "USA"
												   },
								 BillingAddress = new Address
												  {
													  Street     = "202 Birch Street",
													  City       = "Coast City",
													  Region     = "Southern",
													  PostalCode = "34567",
													  Country    = "USA"
												  },
								 AdditionalInformation = "Ensure package is secure.",
								 UserId                = superAdministrator.Id,
								 OrderProducts = new List<ProductOrderLink>
												 {
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "JW001").Id,
														 Quantity = 1
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "PS001").Id,
														 Quantity = 5
													 },
													 new()
													 {
														 ProductId = context.Products
															.First(p => p.Code == "GO001").Id,
														 Quantity = 2
													 }
												 },
								 Payments = new List<Payment>
											{
												new()
												{
													Amount        = 424.93M, // JW001 * 1 + PS001 * 5 + GO001 * 2
													PaymentDate   = DateTime.UtcNow,
													PaymentMethod = PaymentMethod.CreditCard,
													TransactionId = "TX9988776655", // Example transaction ID
													OrderId       = 5 // Optional, or you can set this later
												}
											}
							 }
						 };

			context.Orders.AddRange(orders);
			await context.SaveChangesAsync();
		}
	}
}

// TODO
// Password field must be removed !
// Just for development reasons it is here
// TODO