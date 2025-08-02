using System.Data;
using Bogus;
using Dapper;
using Server.Application.Abstractions.Authentication;
using Server.Application.Abstractions.Data;
using Server.Domain.Abstractions;
using Server.Domain.Shared;
using Server.Domain.Users;

namespace Server.Api.Extensions;

internal static class SeedCustomersDataExtension
{
    public static async Task SeedCustomersDataAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        IAuthenticationService authenticationService =
            scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
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
        int seedCount = connection.QuerySingle<int>(checkSeedSql, new { SeedName = "CustomersData" });

        if (seedCount > 0)
        {
            Console.WriteLine("CustomersData seed has already been executed. Skipping.");
            return;
        }

        // Double-check by counting existing customers
        const string checkCustomersSql =
            "SELECT COUNT(*) FROM users u INNER JOIN role_user ru ON u.id = ru.users_id INNER JOIN roles r ON ru.roles_id = r.id WHERE r.name = 'Customer'";
        int existingCustomers = connection.QuerySingle<int>(checkCustomersSql);

        if (existingCustomers > 0)
        {
            Console.WriteLine(
                $"Customers already exist ({existingCustomers}). Marking seed as completed and skipping.");

            // Mark as completed since customers exist
            const string markCompletedSql = """
                                            INSERT INTO seed_history (seed_name, version) 
                                            VALUES (@SeedName, @Version)
                                            """;

            connection.Execute(markCompletedSql, new
            {
                SeedName = "CustomersData",
                Version = "1.0"
            });

            return;
        }

        var faker = new Faker();

        // Define arrays for more realistic data
        string[] firstNames = new[]
        {
            "John", "Jane", "Michael", "Sarah", "David", "Lisa", "Robert", "Jennifer", "William", "Mary", "James",
            "Patricia", "Richard", "Linda", "Charles", "Barbara", "Joseph", "Elizabeth", "Thomas", "Helen",
            "Christopher", "Nancy", "Daniel", "Betty", "Paul", "Dorothy", "Mark", "Sandra", "Donald", "Donna", "George",
            "Carol", "Kenneth", "Ruth", "Anthony", "Sharon", "Steven", "Michelle", "Kevin", "Laura", "Brian", "Sarah",
            "Edward", "Kimberly", "Ronald", "Deborah", "Timothy", "Dorothy", "Jason", "Lisa", "Jeffrey", "Nancy",
            "Ryan", "Karen", "Jacob", "Betty", "Gary", "Helen", "Nicholas", "Sandra", "Eric", "Donna", "Jonathan",
            "Carol", "Stephen", "Ruth", "Larry", "Sharon", "Justin", "Michelle", "Scott", "Laura", "Brandon", "Sarah",
            "Benjamin", "Kimberly", "Samuel", "Deborah", "Gregory", "Dorothy", "Frank", "Lisa", "Raymond", "Nancy",
            "Alexander", "Karen", "Patrick", "Betty", "Jack", "Helen", "Dennis", "Sandra", "Jerry", "Donna"
        };

        string[] lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker",
            "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams",
            "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts", "Gomez", "Phillips",
            "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes", "Stewart", "Morris", "Morales",
            "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper", "Peterson", "Bailey", "Reed", "Kelly",
            "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson", "Watson", "Brooks", "Chavez", "Wood", "James",
            "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes", "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers",
            "Long", "Ross", "Foster", "Jimenez"
        };

        string[] countries = new[]
        {
            "United States", "Canada", "United Kingdom", "Germany", "France", "Australia", "Netherlands", "Sweden",
            "Norway", "Denmark", "Switzerland", "Belgium", "Austria", "Ireland", "New Zealand", "Finland", "Italy",
            "Spain", "Portugal", "Japan"
        };

        string[] cities = new[]
        {
            "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
            "Dallas", "San Jose", "Austin", "Jacksonville", "San Francisco", "Columbus", "Charlotte", "Fort Worth",
            "Indianapolis", "Seattle", "Denver", "Washington", "Boston", "El Paso", "Detroit", "Nashville", "Portland",
            "Memphis", "Oklahoma City", "Las Vegas", "Louisville", "Baltimore", "Milwaukee", "Albuquerque", "Tucson",
            "Fresno", "Mesa", "Sacramento", "Atlanta", "Kansas City", "Colorado Springs", "Miami", "Raleigh", "Omaha",
            "Long Beach", "Virginia Beach", "Oakland", "Minneapolis", "Tulsa", "Arlington", "Tampa", "New Orleans"
        };

        Console.WriteLine("Starting to seed customers (this may take a while due to Keycloak integration)...");

        // Create customers in smaller batches to avoid overwhelming Keycloak
        const int batchSize = 50;
        const int totalCustomers = 200; // Reduced for faster seeding

        try
        {
            for (int batch = 0; batch < totalCustomers / batchSize; batch++)
            {
                var customers = new List<User>();

                for (int i = 0; i < batchSize; i++)
                {
                    int offset = batch * batchSize;
                    int customerIndex = offset + i;
                    string? firstName = faker.PickRandom(firstNames);
                    string? lastName = faker.PickRandom(lastNames);
                    string email = $"{firstName.ToLower()}.{lastName.ToLower()}{customerIndex}@example.com";
                    string? country = faker.PickRandom(countries);
                    string? city = faker.PickRandom(cities);
                    string password = "password!@#Q1"; // Default password for all seeded users

                    try
                    {
                        // Create user domain object
                        var user = User.CreateCustomer(
                            new FirstName(firstName),
                            new LastName(lastName),
                            new Email(email),
                            new PhoneNumber(GenerateRandomInternationalPhone(faker)),
                            faker.Random.Enum<Gender>(),
                            new Address(country, city, faker.Address.ZipCode(), faker.Address.StreetAddress())
                        );

                        // Register in Keycloak and get identity ID
                        string identityId = await authenticationService.RegisterAsync(
                            user,
                            password,
                            CancellationToken.None
                        );

                        // Set the identity ID from Keycloak
                        user.SetIdentityId(identityId);

                        // Add to repository
                        userRepository.Add(user);
                        customers.Add(user);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to create customer {email}: {ex.Message}");
                        // Continue with other customers
                    }
                }

                // Save batch to database
                if (customers.Any())
                {
                    await unitOfWork.SaveChangesAsync(CancellationToken.None);
                    Console.WriteLine(
                        $"Successfully created batch {batch + 1}/{totalCustomers / batchSize} ({customers.Count} customers)");
                }
            }

            // Mark seed as completed
            const string insertSeedSql = """
                                         INSERT INTO seed_history (seed_name, version) 
                                         VALUES (@SeedName, @Version)
                                         """;

            connection.Execute(insertSeedSql, new
            {
                SeedName = "CustomersData",
                Version = "1.0"
            });

            Console.WriteLine("Successfully seeded customers with Keycloak integration!");
            Console.WriteLine("Default password for all seeded customers: password!@#Q1");
            Console.WriteLine("CustomersData seed marked as completed in seed_history table.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CustomersData seed failed: {ex.Message}");
            Console.WriteLine("Seed will not be marked as completed and can be retried.");
            throw;
        }
    }

    private static string GenerateRandomInternationalPhone(Faker faker)
    {
        return faker.Random.Int(1, 4) switch
        {
            1 => $"+371{faker.Random.Long(20000000L, 29999999L)}", // Latvia
            2 => $"+1{faker.Random.Long(2000000000L, 9999999999L)}", // US/Canada
            3 => $"+44{faker.Random.Long(1000000000L, 9999999999L)}", // UK
            _ => $"+49{faker.Random.Long(1000000000L, 9999999999L)}" // Germany
        };
    }
}
