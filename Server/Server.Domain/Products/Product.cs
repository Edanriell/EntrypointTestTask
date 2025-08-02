using Server.Domain.Abstractions;
using Server.Domain.Products.Events;
using Server.Domain.Shared;

namespace Server.Domain.Products;

public sealed class Product : Entity
{
    private Product(
        Guid id, ProductName name, ProductDescription description, Money price, Quantity reserved, Quantity totalStock,
        ProductStatus status, DateTime lastUpdatedAt, DateTime createdAt, DateTime? lastRestockedAt) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        TotalStock = totalStock;
        Reserved = reserved;
        Status = status;
        LastUpdatedAt = lastUpdatedAt;
        CreatedAt = createdAt;
        LastRestockedAt = lastRestockedAt;
    }

    private Product() { }

    public ProductName Name { get; private set; }
    public ProductDescription Description { get; private set; }
    public Money Price { get; private set; }
    public Quantity TotalStock { get; private set; }
    public Quantity Reserved { get; private set; }
    public Quantity Available => TotalStock - Reserved;
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public DateTime? LastRestockedAt { get; private set; }

    public bool IsOutOfStock => Available.Value == 0;
    public bool HasReservations => Reserved.Value > 0;
    public bool IsInStock => Available.Value > 0;

    public static Product Create(
        ProductName name, ProductDescription description, Money price, Quantity reserved, Quantity totalStock)
    {
        var product = new Product(
            Guid.NewGuid(),
            name,
            description,
            price,
            reserved,
            totalStock,
            ProductStatus.Available,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null
        );

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public Result UpdateBasicInfo(ProductName? name, ProductDescription? description)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        bool hasChanges = false;

        if (name is not null && !Name.Equals(name))
        {
            Name = name;
            hasChanges = true;
        }

        if (description is not null && !Description.Equals(description))
        {
            Description = description;
            hasChanges = true;
        }

        if (hasChanges)
        {
            LastUpdatedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ProductUpdatedDomainEvent(Id));
        }

        return Result.Success();
    }

    public Result UpdatePrice(Money? newPrice)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (Price.Equals(newPrice) || newPrice is null)
        {
            return Result.Success();
        }

        Money oldPrice = Price;
        Price = newPrice;
        // Make sure the product status is available because
        // it could be discounted. Discounted product status can only
        // be achieved through Discount method - route; 
        Status = ProductStatus.Available;
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, newPrice));

        return Result.Success();
    }

    public Result AdjustStock(Quantity? stockChange)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (stockChange is null || stockChange.Value == 0)
        {
            return Result.Success();
        }

        Quantity oldTotalStock = TotalStock;
        Quantity newTotalStock = TotalStock + stockChange;

        // Validate that total stock doesn't go negative
        if (newTotalStock.Value < 0)
        {
            return Result.Failure(ProductErrors.ProductStockCannotBeNegative);
        }

        // Validate that we have enough stock to cover reservations
        if (newTotalStock.Value < Reserved.Value)
        {
            return Result.Failure(ProductErrors.CannotReduceStockBelowReservedAmount);
        }

        TotalStock = newTotalStock;
        LastUpdatedAt = DateTime.UtcNow;

        HandleStockStatusChanges(oldTotalStock, TotalStock);

        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result Update(
        ProductName? name = null,
        ProductDescription? description = null,
        Money? price = null,
        Quantity? stockChange = null)
    {
        if (name is not null || description is not null)
        {
            Result basicInfoResult = UpdateBasicInfo(name, description);
            if (basicInfoResult.IsFailure)
            {
                return Result.Failure(basicInfoResult.Error);
            }
        }

        if (price is not null)
        {
            Result priceResult = UpdatePrice(price);
            if (priceResult.IsFailure)
            {
                return Result.Failure(priceResult.Error);
            }
        }

        if (stockChange is not null)
        {
            Result stockResult = AdjustStock(stockChange);
            if (stockResult.IsFailure)
            {
                return Result.Failure(stockResult.Error);
            }
        }

        return Result.Success();
    }

    public Result ReserveStock(Quantity quantity)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (quantity.Value <= 0)
        {
            return Result.Failure(ProductErrors.InvalidQuantity);
        }

        if (Available.Value < quantity.Value)
        {
            return Result.Failure(ProductErrors.InsufficientStock);
        }

        Reserved += quantity;
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ProductStockReservedDomainEvent(Id, quantity));

        return Result.Success();
    }

    public Result ReleaseReservedStock(Quantity quantity)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (quantity.Value <= 0)
        {
            return Result.Failure(ProductErrors.InvalidQuantity);
        }

        if (Reserved.Value < quantity.Value)
        {
            return Result.Failure(ProductErrors.CannotReleaseMoreThanReserved);
        }

        // Store old values for status change detection
        Quantity oldTotalStock = TotalStock;

        Reserved -= quantity;
        TotalStock -= quantity;

        LastUpdatedAt = DateTime.UtcNow;

        // Handle status changes based on new total stock
        HandleStockStatusChanges(oldTotalStock, TotalStock);

        RaiseDomainEvent(new ProductReservationReleasedDomainEvent(Id, quantity));

        return Result.Success();
    }

    public Result CancelReservation(Quantity quantity)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (quantity.Value <= 0)
        {
            return Result.Failure(ProductErrors.InvalidQuantity);
        }

        if (Reserved.Value < quantity.Value)
        {
            return Result.Failure(ProductErrors.CannotReleaseMoreThanReserved);
        }

        // Store old available quantity to detect status change
        Quantity oldAvailable = Available;

        // Only reduce Reserved, TotalStock stays the same (item stays in warehouse)
        Reserved -= quantity;

        LastUpdatedAt = DateTime.UtcNow;

        // Check if product became available again due to cancellation
        HandleReservationCancelStatusChanges(oldAvailable, Available);

        RaiseDomainEvent(new ProductReservationCancelledDomainEvent(Id, quantity));

        return Result.Success();
    }

    public Result Delete()
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.AlreadyDeleted);
        }

        if (TotalStock.Value > 0)
        {
            return Result.Failure(ProductErrors.CannotDeleteProductWithStock);
        }

        if (Reserved.Value > 0)
        {
            return Result.Failure(ProductErrors.CannotDeleteProductWithActiveReservations);
        }

        Status = ProductStatus.Deleted;
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ProductDeletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Restore()
    {
        if (Status is not ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.UnableToRestoreProduct);
        }

        Status = ProductStatus.Available;
        LastUpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Discount(Money newPrice)
    {
        Money oldPrice = Price;

        if (newPrice < oldPrice)
        {
            Status = ProductStatus.Discontinued;
            RaiseDomainEvent(new ProductDiscountedDomainEvent(Id, newPrice, oldPrice));
        }
        else
        {
            Status = ProductStatus.Available;
        }

        Price = newPrice;
        LastUpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private void HandleStockStatusChanges(Quantity oldStock, Quantity newStock)
    {
        // Stock went from 0 to > 0 (back in stock)
        if (oldStock.Value == 0 && newStock.Value > 0)
        {
            RaiseDomainEvent(new ProductBackInStockDomainEvent(Id));
            Status = ProductStatus.Available;
            LastRestockedAt = DateTime.UtcNow;
        }
        // Stock increase (restocked)
        else if (newStock.Value > oldStock.Value)
        {
            Status = ProductStatus.Available;
            LastRestockedAt = DateTime.UtcNow;
        }
        // Stock went from > 0 to 0 (out of stock)
        else if (oldStock.Value > 0 && newStock.Value == 0)
        {
            RaiseDomainEvent(new ProductOutOfStockDomainEvent(Id));
            Status = ProductStatus.OutOfStock;
        }
    }

    private void HandleReservationCancelStatusChanges(Quantity oldAvailable, Quantity newAvailable)
    {
        // Product went from out of stock to available (when reservation was cancelled)
        if (oldAvailable.Value == 0 && newAvailable.Value > 0)
        {
            if (Status != ProductStatus.OutOfStock)
            {
                return;
            }

            Status = ProductStatus.Available;
            RaiseDomainEvent(new ProductBackInStockDomainEvent(Id));
        }
    }

    public bool HasSufficientStock(Quantity quantity)
    {
        return Available.Value >= quantity.Value;
    }
}
