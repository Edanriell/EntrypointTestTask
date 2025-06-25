using Server.Domain.Abstractions;
using Server.Domain.Products.Events;
using Server.Domain.Shared;

namespace Server.Domain.Products;

public sealed class Product : Entity
{
    private Product(
        Guid id, ProductName name, ProductDescription description, Money price, Quantity reserved, Quantity stock,
        ProductStatus status, DateTime lastUpdatedAt, DateTime createdAt, DateTime? lastRestockedAt) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Reserved = reserved;
        Stock = stock;
        Status = status;
        LastUpdatedAt = lastUpdatedAt;
        CreatedAt = createdAt;
        LastRestockedAt = lastRestockedAt;
    }

    private Product() { }

    public ProductName Name { get; private set; }
    public ProductDescription Description { get; private set; }
    public Money Price { get; private set; }
    public Quantity Reserved { get; private set; }
    public Quantity Stock { get; private set; }
    public ProductStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public DateTime? LastRestockedAt { get; private set; }

    // Helper property
    public bool IsAvailable => Status == ProductStatus.Available && Stock.Value > 0;

    // Helper method
    public bool HasSufficientStock(Quantity quantity)
    {
        return Stock.Value >= quantity.Value;
    }

    public static Product Create(
        ProductName name, ProductDescription description, Money price, Quantity reserved, Quantity stock)
    {
        var product = new Product(
            Guid.NewGuid(),
            name,
            description,
            price,
            reserved,
            stock,
            ProductStatus.Available,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null
        );

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public Result Update(
        ProductName? name, ProductDescription? description, Money? price, Quantity? stock, Quantity? reserved)
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.CouldNotUpdateProduct);
        }

        if (name is not null && !Name.Equals(name))
        {
            Name = name;
        }

        if (description is not null && !Description.Equals(description))
        {
            Description = description;
        }

        if (price is not null && !Price.Equals(price))
        {
            Money oldPrice = Price;
            Price = price;
            Status = ProductStatus.Available;

            RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice, price));
        }

        if (stock is not null && stock.Value != 0)
        {
            Quantity oldStock = Stock;

            if (oldStock.Value == 0 && stock.Value > 0)
            {
                RaiseDomainEvent(new ProductBackInStockDomainEvent(Id));
                Status = ProductStatus.Available;
                LastRestockedAt = DateTime.UtcNow;
            }

            if (oldStock.Value + stock.Value > oldStock.Value)
            {
                Status = ProductStatus.Available;
                LastRestockedAt = DateTime.UtcNow;
            }

            if (oldStock.Value > 0 && oldStock.Value + stock.Value == 0)
            {
                RaiseDomainEvent(new ProductOutOfStockDomainEvent(Id));
                Status = ProductStatus.OutOfStock;
            }

            if (oldStock.Value + stock.Value < 0)
            {
                return Result.Failure(ProductErrors.ProductStockCannotBeNegative);
            }

            Stock += stock;
        }

        if (reserved is not null && reserved.Value != 0)
        {
            if (Reserved.Value + reserved.Value > Stock.Value)
            {
                return Result.Failure(ProductErrors.CannotReserveProduct);
            }

            if (Reserved.Value + reserved.Value < 0)
            {
                return Result.Failure(ProductErrors.CannotUnreserveReservedStock);
            }

            Reserved += reserved;
            Stock -= reserved;
        }

        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result UpdatePrice(Money newPrice)
    {
        Money oldPrice = Price;
        Price = newPrice;
        Status = ProductStatus.Available;
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, newPrice, oldPrice));

        return Result.Success();
    }

    public Result UpdateStock(Quantity newStock)
    {
        Quantity oldStock = Stock;

        if (oldStock.Value == 0 && newStock.Value > 0)
        {
            RaiseDomainEvent(new ProductBackInStockDomainEvent(Id));
            Status = ProductStatus.Available;
            LastRestockedAt = DateTime.UtcNow;
        }

        if (oldStock.Value + newStock.Value > oldStock.Value)
        {
            Status = ProductStatus.Available;
            LastRestockedAt = DateTime.UtcNow;
        }

        if (oldStock.Value > 0 && oldStock.Value + newStock.Value == 0)
        {
            RaiseDomainEvent(new ProductOutOfStockDomainEvent(Id));
            Status = ProductStatus.OutOfStock;
        }

        if (oldStock.Value + newStock.Value < 0)
        {
            return Result.Failure(ProductErrors.ProductStockCannotBeNegative);
        }

        Stock += newStock;
        LastUpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateReservedStock(Quantity newReservedStock)
    {
        if (Reserved.Value + newReservedStock.Value > Stock.Value)
        {
            return Result.Failure(ProductErrors.CannotReserveProduct);
        }

        if (Reserved.Value + newReservedStock.Value < 0)
        {
            return Result.Failure(ProductErrors.CannotUnreserveReservedStock);
        }

        Reserved += newReservedStock;
        Stock -= newReservedStock;
        LastUpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Delete()
    {
        if (Status == ProductStatus.Deleted)
        {
            return Result.Failure(ProductErrors.AlreadyDeleted);
        }

        if (Stock.Value > 0)
        {
            return Result.Failure(ProductErrors.CannotDeleteProductWithStock);
        }

        if (Reserved.Value > 0 && Stock.Value == 0)
        {
            return Result.Failure(ProductErrors.CannotDeleteProductWithActiveOrders);
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
}
