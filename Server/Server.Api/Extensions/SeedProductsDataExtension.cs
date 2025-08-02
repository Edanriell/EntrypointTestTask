using System.Data;
using Bogus;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Domain.Abstractions;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Api.Extensions;

internal static class SeedProductsDataExtension
{
    public static async Task SeedProductsDataAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        IProductRepository productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        using IDbConnection connection = sqlConnectionFactory.CreateConnection();

        // Check if seed_history table exists, if not create it
        const string checkTableExistsSql = """
                                           SELECT EXISTS (
                                               SELECT FROM information_schema.tables 
                                               WHERE table_schema = 'public' 
                                               AND table_name = 'seed_history'
                                           )
                                           """;

        bool tableExists = connection.QuerySingle<bool>(checkTableExistsSql);

        if (!tableExists)
        {
            const string createTableSql = """
                                          CREATE TABLE seed_history (
                                              id SERIAL PRIMARY KEY,
                                              seed_name VARCHAR(255) NOT NULL UNIQUE,
                                              executed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                              version VARCHAR(50) NOT NULL DEFAULT '1.0'
                                          )
                                          """;

            connection.Execute(createTableSql);
            Console.WriteLine("Created seed_history table.");
        }

        // Check if this specific seed has been run
        const string checkSeedSql = "SELECT COUNT(*) FROM seed_history WHERE seed_name = @SeedName";
        int seedCount = connection.QuerySingle<int>(checkSeedSql, new { SeedName = "ProductsData" });

        if (seedCount > 0)
        {
            Console.WriteLine("ProductsData seed has already been executed. Skipping.");
            return;
        }

        // Double-check by counting existing products
        const string checkProductsSql = "SELECT COUNT(*) FROM products";
        int existingProducts = connection.QuerySingle<int>(checkProductsSql);

        if (existingProducts > 0)
        {
            Console.WriteLine(
                $"Products already exist ({existingProducts}). Marking seed as completed and skipping.");

            // Mark as completed since products exist
            const string markCompletedSql = """
                                            INSERT INTO seed_history (seed_name, version) 
                                            VALUES (@SeedName, @Version)
                                            """;

            connection.Execute(markCompletedSql, new
            {
                SeedName = "ProductsData",
                Version = "1.0"
            });

            return;
        }

        var faker = new Faker();

        // Define arrays for realistic product categories and names
        string[] electronics = new[]
        {
            "Smartphone", "Laptop", "Tablet", "Desktop Computer", "Monitor", "Keyboard", "Mouse", "Headphones",
            "Speakers", "Webcam", "Microphone", "Router", "Switch", "Cable", "Charger", "Power Bank",
            "Smart Watch", "Fitness Tracker", "VR Headset", "Gaming Console", "Controller", "Hard Drive",
            "SSD", "RAM Memory", "Graphics Card", "Processor", "Motherboard", "Power Supply", "Case",
            "Cooling Fan", "Thermal Paste", "USB Hub", "Docking Station", "Printer", "Scanner",
            "Network Card", "Sound Card", "Bluetooth Adapter", "WiFi Adapter", "External Battery"
        };

        string[] homeGarden = new[]
        {
            "Garden Hose", "Lawn Mower", "Trimmer", "Rake", "Shovel", "Pruning Shears", "Watering Can",
            "Plant Pot", "Fertilizer", "Seeds", "Soil", "Mulch", "Garden Gloves", "Wheelbarrow",
            "Compost Bin", "Garden Sprinkler", "Hedge Trimmer", "Leaf Blower", "Garden Tool Set",
            "Outdoor Furniture", "Patio Umbrella", "BBQ Grill", "Fire Pit", "Garden Lights",
            "Bird Feeder", "Garden Ornament", "Greenhouse", "Trellis", "Garden Arch", "Planter Box"
        };

        string[] sportsOutdoors = new[]
        {
            "Running Shoes", "Hiking Boots", "Backpack", "Tent", "Sleeping Bag", "Camping Chair",
            "Flashlight", "Compass", "Water Bottle", "Thermos", "Cooler", "Fishing Rod", "Tackle Box",
            "Bicycle", "Helmet", "Knee Pads", "Basketball", "Football", "Soccer Ball", "Tennis Racket",
            "Golf Clubs", "Yoga Mat", "Dumbbells", "Resistance Bands", "Jump Rope", "Swimming Goggles",
            "Surfboard", "Skateboard", "Roller Skates", "Climbing Gear", "Kayak", "Life Jacket"
        };

        string[] books = new[]
        {
            "Programming Guide", "Fiction Novel", "Biography", "History Book", "Science Textbook",
            "Cookbook", "Travel Guide", "Art Book", "Photography Book", "Dictionary", "Encyclopedia",
            "Language Learning", "Self-Help Book", "Business Guide", "Philosophy Book", "Poetry Collection",
            "Children's Book", "Comic Book", "Graphic Novel", "Technical Manual", "Reference Book"
        };

        string[] clothing = new[]
        {
            "T-Shirt", "Jeans", "Dress", "Sweater", "Jacket", "Coat", "Shorts", "Skirt", "Blouse",
            "Pants", "Hoodie", "Cardigan", "Blazer", "Suit", "Tie", "Scarf", "Hat", "Cap", "Gloves",
            "Socks", "Underwear", "Pajamas", "Swimwear", "Sportswear", "Uniform", "Vest", "Belt",
            "Shoes", "Boots", "Sandals", "Sneakers", "Slippers", "High Heels"
        };

        string[] homeKitchen = new[]
        {
            "Coffee Maker", "Blender", "Toaster", "Microwave", "Oven", "Refrigerator", "Dishwasher",
            "Vacuum Cleaner", "Iron", "Hair Dryer", "Washing Machine", "Dryer", "Air Conditioner",
            "Heater", "Humidifier", "Dehumidifier", "Air Purifier", "Kitchen Knife", "Cutting Board",
            "Pots and Pans", "Dinnerware Set", "Glassware", "Cutlery Set", "Storage Container",
            "Trash Can", "Cleaning Supplies", "Laundry Detergent", "Dish Soap", "Paper Towels"
        };

        string[] toys = new[]
        {
            "Action Figure", "Doll", "Board Game", "Puzzle", "LEGO Set", "Remote Control Car",
            "Stuffed Animal", "Building Blocks", "Educational Toy", "Art Supplies", "Musical Instrument",
            "Toy Car", "Train Set", "Dollhouse", "Playmat", "Ball", "Frisbee", "Kite", "Yo-Yo",
            "Magic Kit", "Science Kit", "Craft Kit", "Model Kit", "Trading Cards", "Video Game"
        };

        string[] beauty = new[]
        {
            "Shampoo", "Conditioner", "Body Wash", "Soap", "Moisturizer", "Sunscreen", "Face Mask",
            "Serum", "Toner", "Cleanser", "Makeup", "Lipstick", "Foundation", "Concealer", "Mascara",
            "Eyeshadow", "Nail Polish", "Perfume", "Cologne", "Brush Set", "Mirror", "Hair Straightener",
            "Curling Iron", "Hair Dryer", "Razor", "Toothbrush", "Toothpaste", "Mouthwash", "Deodorant"
        };

        // Combine all categories
        var allCategories = new Dictionary<string, string[]>
        {
            { "Electronics", electronics },
            { "Home & Garden", homeGarden },
            { "Sports & Outdoors", sportsOutdoors },
            { "Books", books },
            { "Clothing", clothing },
            { "Home & Kitchen", homeKitchen },
            { "Toys & Games", toys },
            { "Beauty & Personal Care", beauty }
        };

        // Brand names for each category
        string[] electronicsBrands = { "TechPro", "DigitalMax", "ElectroVibe", "SmartTech", "GadgetHub", "CyberCore" };
        string[] homeGardenBrands = { "GreenThumb", "HomePro", "GardenMax", "EcoLife", "OutdoorPlus", "NatureWorks" };
        string[] sportsBrands = { "ActiveGear", "SportMax", "FitPro", "AdventureTime", "PlayHard", "GoActive" };
        string[] bookBrands = { "ReadWell", "BookHub", "LearnMore", "WisdomPress", "KnowledgeBase", "StudyPro" };
        string[] clothingBrands = { "StyleMax", "FashionPro", "TrendyWear", "ComfortZone", "UrbanStyle", "ClassicFit" };
        string[] homeKitchenBrands = { "HomeCook", "KitchenPro", "ChefMax", "CookWell", "HomeHelper", "CulinaryPro" };
        string[] toyBrands = { "PlayTime", "FunMax", "ToyPro", "HappyKids", "JoyfulPlay", "KidsZone" };
        string[] beautyBrands = { "BeautyMax", "GlowPro", "PureSkin", "RadiantLife", "SkinCare+", "NaturalGlow" };

        var brandsByCategory = new Dictionary<string, string[]>
        {
            { "Electronics", electronicsBrands },
            { "Home & Garden", homeGardenBrands },
            { "Sports & Outdoors", sportsBrands },
            { "Books", bookBrands },
            { "Clothing", clothingBrands },
            { "Home & Kitchen", homeKitchenBrands },
            { "Toys & Games", toyBrands },
            { "Beauty & Personal Care", beautyBrands }
        };

        // Product model variations for more realistic names
        string[] modelVariations =
            { "Pro", "Max", "Plus", "Elite", "Premium", "Standard", "Basic", "Deluxe", "Ultra", "Advanced" };

        Console.WriteLine("Starting to seed products (creating a lot of products as requested)...");

        // Create products in batches
        const int batchSize = 100;
        const int totalProducts = 5000; // A LOT of products as requested!

        try
        {
            for (int batch = 0; batch < totalProducts / batchSize; batch++)
            {
                var products = new List<Product>();

                for (int i = 0; i < batchSize; i++)
                {
                    // Pick a random category - fix the KeyValuePair issue
                    var categoryList = allCategories.ToList();
                    KeyValuePair<string, string[]> selectedCategory = faker.PickRandom(categoryList);
                    string category = selectedCategory.Key;
                    string[] categoryProducts = selectedCategory.Value;
                    string[] categoryBrands = brandsByCategory[category];

                    // Generate product details - cleaner names without random codes
                    string baseProductName = faker.PickRandom(categoryProducts);
                    string brand = faker.PickRandom(categoryBrands);
                    string model = faker.PickRandom(modelVariations);
                    string productName = $"{brand} {baseProductName} {model}";

                    // Generate description
                    string description = GenerateProductDescription(baseProductName, brand, category, faker);

                    // Generate price based on category
                    decimal price = GeneratePriceByCategory(category, faker);

                    // Generate stock quantities - NO RESERVATIONS as requested!
                    int totalStock = faker.Random.Int(10, 500);
                    int reserved = 0; // Always 0 - reservations only happen through orders!

                    try
                    {
                        // Create value objects using proper factory methods
                        Result<ProductName> productNameResult = ProductName.Create(productName);
                        if (productNameResult.IsFailure)
                        {
                            Console.WriteLine(
                                $"Failed to create ProductName for {productName}: {productNameResult.Error}");
                            continue;
                        }

                        Result<ProductDescription> productDescriptionResult = ProductDescription.Create(description);
                        if (productDescriptionResult.IsFailure)
                        {
                            Console.WriteLine(
                                $"Failed to create ProductDescription for {productName}: {productDescriptionResult.Error}");
                            continue;
                        }

                        var priceVO = new Money(price, Currency.Usd);

                        Result<Quantity> reservedResult = Quantity.CreateQuantity(reserved);
                        if (reservedResult.IsFailure)
                        {
                            Console.WriteLine(
                                $"Failed to create Reserved Quantity for {productName}: {reservedResult.Error}");
                            continue;
                        }

                        Result<Quantity> totalStockResult = Quantity.CreateQuantity(totalStock);
                        if (totalStockResult.IsFailure)
                        {
                            Console.WriteLine(
                                $"Failed to create TotalStock Quantity for {productName}: {totalStockResult.Error}");
                            continue;
                        }

                        // Create product using domain factory method
                        var product = Product.Create(
                            productNameResult.Value,
                            productDescriptionResult.Value,
                            priceVO,
                            reservedResult.Value,
                            totalStockResult.Value
                        );

                        productRepository.Add(product);
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to create product {productName}: {ex.Message}");
                        // Continue with other products
                    }
                }

                // Save batch to database
                if (products.Any())
                {
                    await unitOfWork.SaveChangesAsync(CancellationToken.None);
                    Console.WriteLine(
                        $"Successfully created batch {batch + 1}/{totalProducts / batchSize} ({products.Count} products)");
                }
            }

            // Mark seed as completed
            const string insertSeedSql = """
                                         INSERT INTO seed_history (seed_name, version) 
                                         VALUES (@SeedName, @Version)
                                         """;

            connection.Execute(insertSeedSql, new
            {
                SeedName = "ProductsData",
                Version = "1.0"
            });

            Console.WriteLine($"Successfully seeded {totalProducts} products!");
            Console.WriteLine(
                "All products created with ZERO reservations - reservations will be handled through orders.");
            Console.WriteLine("ProductsData seed marked as completed in seed_history table.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProductsData seed failed: {ex.Message}");
            Console.WriteLine("Seed will not be marked as completed and can be retried.");
            throw;
        }
    }

    private static string GenerateProductDescription(string productName, string brand, string category, Faker faker)
    {
        string[] descriptions = new[]
        {
            $"High-quality {productName.ToLower()} from {brand}. Perfect for {category.ToLower()} enthusiasts.",
            $"Professional-grade {productName.ToLower()} designed for maximum performance and durability.",
            $"Premium {productName.ToLower()} featuring advanced technology and superior craftsmanship.",
            $"Innovative {productName.ToLower()} that combines style, functionality, and reliability.",
            $"Top-rated {productName.ToLower()} trusted by professionals and consumers worldwide.",
            $"Award-winning {productName.ToLower()} with exceptional build quality and performance.",
            $"State-of-the-art {productName.ToLower()} designed to exceed your expectations.",
            $"Bestselling {productName.ToLower()} known for its outstanding value and performance."
        };

        string baseDescription = faker.PickRandom(descriptions);

        string[] features = new[]
        {
            "Easy to use", "Durable construction", "Energy efficient", "Compact design",
            "User-friendly interface", "Advanced features", "Reliable performance", "Great value",
            "Long-lasting", "High-performance", "Professional quality", "Innovative design"
        };

        string additionalFeatures = string.Join(", ", faker.PickRandom(features, faker.Random.Int(2, 4)));

        return $"{baseDescription} Features: {additionalFeatures}. Backed by manufacturer warranty.";
    }

    private static decimal GeneratePriceByCategory(string category, Faker faker)
    {
        return category switch
        {
            "Electronics" => faker.Random.Decimal(29.99m, 2999.99m),
            "Home & Garden" => faker.Random.Decimal(9.99m, 899.99m),
            "Sports & Outdoors" => faker.Random.Decimal(14.99m, 1299.99m),
            "Books" => faker.Random.Decimal(7.99m, 89.99m),
            "Clothing" => faker.Random.Decimal(12.99m, 299.99m),
            "Home & Kitchen" => faker.Random.Decimal(19.99m, 1999.99m),
            "Toys & Games" => faker.Random.Decimal(5.99m, 199.99m),
            "Beauty & Personal Care" => faker.Random.Decimal(3.99m, 149.99m),
            _ => faker.Random.Decimal(9.99m, 499.99m)
        };
    }
}
