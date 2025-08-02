// using System.Data;
// using Bogus;
// using Dapper;
// using Server.Application.Abstractions.Data;
// using Server.Domain.Abstractions;
// using Server.Domain.OrderItems;
// using Server.Domain.Orders;
// using Server.Domain.Payments;
// using Server.Domain.Products;
// using Server.Domain.Shared;
// using Server.Domain.Users;
//
// namespace Server.Api.Extensions;
//
// internal static class SeedOrdersDataExtension
// {
//     public static async Task SeedOrdersDataAsync(this IApplicationBuilder app)
//     {
//         using IServiceScope scope = app.ApplicationServices.CreateScope();
//
//         ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
//         IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
//         IProductRepository productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
//         IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
//         IPaymentRepository paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
//         IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//
//         using IDbConnection connection = sqlConnectionFactory.CreateConnection();
//
//         Console.WriteLine("=== Starting Orders Seeding Process ===");
//
//         // Check if seed_history table exists, if not create it
//         const string checkTableExistsSql = """
//                                            SELECT EXISTS (
//                                                SELECT FROM information_schema.tables 
//                                                WHERE table_schema = 'public' 
//                                                AND table_name = 'seed_history'
//                                            )
//                                            """;
//
//         bool tableExists = connection.QuerySingle<bool>(checkTableExistsSql);
//
//         if (!tableExists)
//         {
//             const string createTableSql = """
//                                           CREATE TABLE seed_history (
//                                               id SERIAL PRIMARY KEY,
//                                               seed_name VARCHAR(255) NOT NULL UNIQUE,
//                                               executed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
//                                               version VARCHAR(50) NOT NULL DEFAULT '1.0'
//                                           )
//                                           """;
//
//             connection.Execute(createTableSql);
//             Console.WriteLine("Created seed_history table.");
//         }
//
//         // Check if this specific seed has been run
//         const string checkSeedSql = "SELECT COUNT(*) FROM seed_history WHERE seed_name = @SeedName";
//         int seedCount = connection.QuerySingle<int>(checkSeedSql, new { SeedName = "OrdersData" });
//
//         if (seedCount > 0)
//         {
//             Console.WriteLine("OrdersData seed has already been executed. Skipping.");
//             return;
//         }
//
//         // Double-check by counting existing orders
//         const string checkOrdersSql = "SELECT COUNT(*) FROM orders";
//         int existingOrders = connection.QuerySingle<int>(checkOrdersSql);
//
//         if (existingOrders > 0)
//         {
//             Console.WriteLine($"Orders already exist ({existingOrders}). Marking seed as completed and skipping.");
//
//             // Mark as completed since orders exist
//             const string markCompletedSql = """
//                                             INSERT INTO seed_history (seed_name, version) 
//                                             VALUES (@SeedName, @Version)
//                                             """;
//
//             connection.Execute(markCompletedSql, new
//             {
//                 SeedName = "OrdersData",
//                 Version = "1.0"
//             });
//
//             return;
//         }
//
//         Console.WriteLine("Getting existing users and products...");
//
//         // Get existing users and products
//         IEnumerable<User> users = await userRepository.GetAllCustomersAsync(CancellationToken.None);
//         IEnumerable<Product> products = await productRepository.GetAllAsync(CancellationToken.None);
//
//         Console.WriteLine($"Found {users.Count()} customers and {products.Count()} products");
//
//         if (!users.Any())
//         {
//             Console.WriteLine("ERROR: No users found. Please seed customers first before seeding orders.");
//             return;
//         }
//
//         if (!products.Any())
//         {
//             Console.WriteLine("ERROR: No products found. Please seed products first before seeding orders.");
//             return;
//         }
//
//         var faker = new Faker();
//
//         Console.WriteLine("Starting to seed orders and payments...");
//
//         // Create orders in smaller batches for better error tracking
//         const int batchSize = 10; // Reduced for better debugging
//         const int totalOrders = 50; // Reduced for initial testing
//
//         // Order status distribution weights
//         var orderStatuses = new WeightedOrderStatus[]
//         {
//             new(OrderStatus.Completed, 30),
//             new(OrderStatus.Delivered, 25),
//             new(OrderStatus.Shipped, 15),
//             new(OrderStatus.Processing, 10),
//             new(OrderStatus.Confirmed, 8),
//             new(OrderStatus.Pending, 7),
//             new(OrderStatus.Cancelled, 5)
//         };
//
//         int totalCreatedOrders = 0;
//         int totalCreatedPayments = 0;
//
//         try
//         {
//             for (int batch = 0; batch < totalOrders / batchSize; batch++)
//             {
//                 Console.WriteLine($"\n--- Processing batch {batch + 1}/{totalOrders / batchSize} ---");
//
//                 var orders = new List<Order>();
//                 var payments = new List<Payment>();
//
//                 for (int i = 0; i < batchSize; i++)
//                 {
//                     int orderIndex = batch * batchSize + i + 1;
//
//                     try
//                     {
//                         Console.WriteLine($"Creating order {orderIndex}...");
//
//                         // Select random user
//                         User? user = faker.PickRandom(users);
//                         Console.WriteLine($"Selected user: {user.Id}");
//
//                         // Generate order details
//                         OrderStatus targetStatus = GetWeightedOrderStatus(orderStatuses, faker);
//                         Currency currency = Currency.Usd;
//
//                         Console.WriteLine($"Target status: {targetStatus}");
//
//                         // Generate shipping address
//                         Address shippingAddress = GenerateShippingAddress(faker);
//                         Console.WriteLine($"Generated address: {shippingAddress.Street}, {shippingAddress.City}");
//
//                         // TODO 
//                         // Random OrderInfo
//                         // Reserve products ! 
//
//                         // Create order
//                         Result<Order> orderResult = Order.Create(
//                             user.Id,
//                             GenerateOrderNumber(orderIndex),
//                             currency,
//                             shippingAddress
//                         );
//
//                         if (orderResult.IsFailure)
//                         {
//                             Console.WriteLine($"ERROR: Failed to create order {orderIndex}: {orderResult.Error}");
//                             continue;
//                         }
//
//                         Order order = orderResult.Value;
//                         Console.WriteLine($"Order created with ID: {order.Id}");
//
//                         // Select 1-3 random products for this order
//                         int itemCount = faker.Random.Int(1, 3);
//                         var selectedProducts = faker.PickRandom(products, itemCount).ToList();
//
//                         Console.WriteLine($"Selected {selectedProducts.Count} products for order");
//
//                         // Add products to order
//                         bool hasValidProducts = false;
//                         foreach (Product? product in selectedProducts)
//                         {
//                             try
//                             {
//                                 int quantity = faker.Random.Int(1, 2); // Reduced quantity for testing
//                                 Result<Quantity> quantityResult = Quantity.CreateQuantity(quantity);
//
//                                 if (quantityResult.IsFailure)
//                                 {
//                                     Console.WriteLine($"ERROR: Failed to create quantity: {quantityResult.Error}");
//                                     continue;
//                                 }
//
//                                 Console.WriteLine($"Adding product {product.Name.Value} (qty: {quantity}) to order");
//
//                                 // Create order product
//                                 Result<OrderProduct> orderProductResult = OrderProduct.Create(
//                                     order.Id,
//                                     product.Id,
//                                     product.Name,
//                                     product.Price,
//                                     quantityResult.Value
//                                 );
//
//                                 if (orderProductResult.IsFailure)
//                                 {
//                                     Console.WriteLine(
//                                         $"ERROR: Failed to create order product: {orderProductResult.Error}");
//                                     continue;
//                                 }
//
//                                 // Add product to order
//                                 Result addResult = order.AddProduct(orderProductResult.Value);
//                                 if (addResult.IsFailure)
//                                 {
//                                     Console.WriteLine($"ERROR: Failed to add product to order: {addResult.Error}");
//                                     continue;
//                                 }
//
//                                 hasValidProducts = true;
//                                 Console.WriteLine("Successfully added product to order");
//                             }
//                             catch (Exception ex)
//                             {
//                                 Console.WriteLine($"ERROR: Exception adding product to order: {ex.Message}");
//                             }
//                         }
//
//                         // Only proceed if order has products
//                         if (!hasValidProducts || !order.OrderProducts.Any())
//                         {
//                             Console.WriteLine($"WARNING: Order {orderIndex} has no valid products, skipping");
//                             continue;
//                         }
//
//                         Console.WriteLine(
//                             $"Order total amount: {order.TotalAmount.Amount} {order.TotalAmount.Currency}");
//
//                         // Create payments (simplified for debugging)
//                         if (targetStatus != OrderStatus.Cancelled)
//                         {
//                             try
//                             {
//                                 Console.WriteLine("Creating payment for order...");
//                                 List<Payment> createdPayments = await CreateSimplePayment(order, targetStatus, faker);
//                                 payments.AddRange(createdPayments);
//                                 Console.WriteLine($"Created {createdPayments.Count} payments for order");
//                             }
//                             catch (Exception ex)
//                             {
//                                 Console.WriteLine($"ERROR: Exception creating payments: {ex.Message}");
//                             }
//                         }
//
//                         // Progress order through statuses (simplified)
//                         try
//                         {
//                             Console.WriteLine($"Progressing order to status: {targetStatus}");
//                             await ProgressOrderToTargetStatus(order, targetStatus, faker);
//                             Console.WriteLine($"Order progressed successfully to: {order.Status}");
//                         }
//                         catch (Exception ex)
//                         {
//                             Console.WriteLine($"ERROR: Exception progressing order: {ex.Message}");
//                         }
//
//                         orders.Add(order);
//                         orderRepository.Add(order);
//                         Console.WriteLine($"Order {orderIndex} added to repository");
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine($"ERROR: Exception creating order {orderIndex}: {ex.Message}");
//                         Console.WriteLine($"Stack trace: {ex.StackTrace}");
//                     }
//                 }
//
//                 // Add payments to repository
//                 foreach (Payment payment in payments)
//                 {
//                     paymentRepository.Add(payment);
//                 }
//
//                 Console.WriteLine($"Batch {batch + 1}: Created {orders.Count} orders and {payments.Count} payments");
//
//                 // Save batch to database
//                 if (orders.Any())
//                 {
//                     try
//                     {
//                         Console.WriteLine("Saving batch to database...");
//                         await unitOfWork.SaveChangesAsync(CancellationToken.None);
//
//                         totalCreatedOrders += orders.Count;
//                         totalCreatedPayments += payments.Count;
//
//                         Console.WriteLine($"✓ Batch {batch + 1} saved successfully!");
//                         Console.WriteLine($"  Orders: {orders.Count}, Payments: {payments.Count}");
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine($"ERROR: Failed to save batch {batch + 1}: {ex.Message}");
//                         Console.WriteLine($"Stack trace: {ex.StackTrace}");
//                         throw; // Re-throw to stop the seeding process
//                     }
//                 }
//                 else
//                 {
//                     Console.WriteLine($"WARNING: Batch {batch + 1} has no orders to save");
//                 }
//
//                 // Add a small delay between batches
//                 await Task.Delay(100);
//             }
//
//             // Verify orders were created
//             const string verifyOrdersSql = "SELECT COUNT(*) FROM orders";
//             int finalOrderCount = connection.QuerySingle<int>(verifyOrdersSql);
//
//             Console.WriteLine("\n=== Seeding Summary ===");
//             Console.WriteLine($"Orders created in this session: {totalCreatedOrders}");
//             Console.WriteLine($"Payments created in this session: {totalCreatedPayments}");
//             Console.WriteLine($"Total orders in database: {finalOrderCount}");
//
//             if (finalOrderCount > 0)
//             {
//                 // Mark seed as completed
//                 const string insertSeedSql = """
//                                              INSERT INTO seed_history (seed_name, version) 
//                                              VALUES (@SeedName, @Version)
//                                              """;
//
//                 connection.Execute(insertSeedSql, new
//                 {
//                     SeedName = "OrdersData",
//                     Version = "1.0"
//                 });
//
//                 Console.WriteLine("✓ OrdersData seed marked as completed in seed_history table.");
//             }
//             else
//             {
//                 Console.WriteLine("WARNING: No orders were created, seed not marked as completed.");
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"FATAL ERROR: OrdersData seed failed: {ex.Message}");
//             Console.WriteLine($"Stack trace: {ex.StackTrace}");
//             Console.WriteLine("Seed will not be marked as completed and can be retried.");
//             throw;
//         }
//     }
//
//     private static OrderNumber GenerateOrderNumber(int orderIndex)
//     {
//         try
//         {
//             var orderGuid = Guid.NewGuid();
//             Result<OrderNumber> result = OrderNumber.Create(orderGuid);
//
//             if (result.IsSuccess)
//             {
//                 return result.Value;
//             }
//
//             Console.WriteLine($"WARNING: First order number creation failed: {result.Error}");
//
//             // Fallback
//             Result<OrderNumber> fallbackResult = OrderNumber.Create(Guid.NewGuid());
//             return fallbackResult.IsSuccess
//                 ? fallbackResult.Value
//                 : throw new InvalidOperationException($"Failed to create order number: {fallbackResult.Error}");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"ERROR: Exception in GenerateOrderNumber: {ex.Message}");
//             throw;
//         }
//     }
//
//     private static Address GenerateShippingAddress(Faker faker)
//     {
//         try
//         {
//             string[] countries = { "United States", "Canada", "United Kingdom" };
//             string[] cities = { "New York", "Toronto", "London" };
//
//             return new Address(
//                 faker.PickRandom(countries),
//                 faker.PickRandom(cities),
//                 faker.Address.ZipCode(),
//                 faker.Address.StreetAddress()
//             );
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"ERROR: Exception in GenerateShippingAddress: {ex.Message}");
//             throw;
//         }
//     }
//
//     // Simplified payment creation for debugging
//     private static async Task<List<Payment>> CreateSimplePayment(Order order, OrderStatus targetStatus, Faker faker)
//     {
//         var payments = new List<Payment>();
//
//         try
//         {
//             if (!ShouldPaymentBeProcessed(targetStatus))
//             {
//                 return payments;
//             }
//
//             PaymentMethod paymentMethod = PaymentMethod.CreditCard; // Simplified
//             Result<Payment> paymentResult = Payment.Create(order.Id, order.TotalAmount, paymentMethod);
//
//             if (paymentResult.IsFailure)
//             {
//                 Console.WriteLine($"ERROR: Failed to create payment: {paymentResult.Error}");
//                 return payments;
//             }
//
//             Payment payment = paymentResult.Value;
//             Console.WriteLine($"Payment created with ID: {payment.Id}, Amount: {payment.Amount.Amount}");
//
//             // Don't process payment during seeding to avoid delays - just mark as paid
//             if (targetStatus != OrderStatus.Pending && targetStatus != OrderStatus.Cancelled)
//             {
//                 // Simulate successful payment without the Thread.Sleep
//                 Result processResult = payment.UpdateStatus(PaymentStatus.Paid);
//                 if (processResult.IsSuccess)
//                 {
//                     Console.WriteLine("Payment marked as paid");
//                     order.RecordOrderPayment(payment.Id, payment.Amount);
//                     order.UpdateOrderTotalPaidAmount(payment.Amount);
//                     order.UpdatePaymentStatusFlags(false);
//                     Console.WriteLine("Payment recorded in order");
//                 }
//             }
//
//             payments.Add(payment);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"ERROR: Exception in CreateSimplePayment: {ex.Message}");
//         }
//
//         return payments;
//     }
//
//     private static bool ShouldPaymentBeProcessed(OrderStatus targetStatus)
//     {
//         return targetStatus switch
//         {
//             OrderStatus.Pending => false,
//             OrderStatus.Cancelled => false,
//             _ => true
//         };
//     }
//
//     private static async Task ProgressOrderToTargetStatus(Order order, OrderStatus targetStatus, Faker faker)
//     {
//         try
//         {
//             switch (targetStatus)
//             {
//                 case OrderStatus.Pending:
//                     // Order is already pending
//                     break;
//
//                 case OrderStatus.Confirmed:
//                     if (order.IsFullyPaid())
//                     {
//                         order.Confirm();
//                     }
//
//                     break;
//
//                 case OrderStatus.Processing:
//                     if (order.IsFullyPaid())
//                     {
//                         order.Confirm();
//                         order.StartProcessing();
//                     }
//
//                     break;
//
//                 case OrderStatus.Cancelled:
//                     Result<CancellationReason> reasonResult =
//                         CancellationReason.Create("Customer requested cancellation");
//                     if (reasonResult.IsSuccess)
//                     {
//                         order.Cancel(reasonResult.Value);
//                     }
//
//                     break;
//
//                 // Simplified - only handle basic statuses for now
//                 default:
//                     if (order.IsFullyPaid())
//                     {
//                         order.Confirm();
//                     }
//
//                     break;
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"ERROR: Exception progressing order to status {targetStatus}: {ex.Message}");
//             throw;
//         }
//     }
//
//     private static OrderStatus GetWeightedOrderStatus(WeightedOrderStatus[] weightedStatuses, Faker faker)
//     {
//         try
//         {
//             // Simplified for debugging - just return a basic status
//             OrderStatus[] basicStatuses = new[]
//                 { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Cancelled };
//             return faker.PickRandom(basicStatuses);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"ERROR: Exception in GetWeightedOrderStatus: {ex.Message}");
//             return OrderStatus.Pending;
//         }
//     }
//
//     private record WeightedOrderStatus(OrderStatus Status, int Weight);
// }


