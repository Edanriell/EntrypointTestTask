namespace Server.Application.Users.GetClients;

// public sealed record GetCustomersResponse(
//     IReadOnlyList<Customer> Customers
// );
public sealed record GetCustomersResponse
{
    public IReadOnlyList<Customer> Customers { get; init; } = new List<Customer>();
    public string? NextCursor { get; init; }
    public string? PreviousCursor { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int TotalCount { get; init; }
    public int CurrentPageSize { get; init; }
    public int PageNumber { get; init; }
}

public sealed class Customer
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }

    // Order Summary
    public int TotalOrders { get; init; }
    public int CompletedOrders { get; init; }
    public int PendingOrders { get; init; }
    public int CancelledOrders { get; init; }
    public decimal TotalSpent { get; init; }
    public DateTime? LastOrderDate { get; init; }

    // Recent Orders (last 5 orders within 30 days)
    public IReadOnlyList<RecentOrder> RecentOrders { get; set; } = new List<RecentOrder>();

    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";
    public string FullAddress => $"{Street}, {City}, {ZipCode}, {Country}";
    public bool HasOrders => TotalOrders > 0;
    public bool HasRecentActivity => RecentOrders.Any();
    public decimal AverageOrderValue => TotalOrders > 0 ? TotalSpent / TotalOrders : 0;
    public string CustomerStatus => GetCustomerStatus();

    private string GetCustomerStatus()
    {
        if (TotalOrders == 0)
        {
            return "New";
        }

        if (HasRecentActivity)
        {
            return "Active";
        }

        if (LastOrderDate.HasValue && LastOrderDate.Value > DateTime.UtcNow.AddDays(-90))
        {
            return "Recent";
        }

        return "Inactive";
    }
}

public sealed class RecentOrder
{
    public Guid UserId { get; init; }
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
}
