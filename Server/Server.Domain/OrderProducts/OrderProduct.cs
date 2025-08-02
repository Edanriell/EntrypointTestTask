using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Domain.OrderProducts;

public sealed class OrderProduct : Entity
{
    private OrderProduct(
        Guid id,
        Guid orderId,
        Guid productId,
        ProductName productName,
        Money unitPrice,
        Quantity quantity,
        Money totalPrice) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TotalPrice = totalPrice;
    }

    private OrderProduct() { }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public ProductName ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money TotalPrice { get; private set; }

    // Navigation properties
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    // We use domain service to create OrderProduct
    public static Result<OrderProduct> Create(
        Guid orderId,
        Guid productId,
        ProductName productName,
        Money unitPrice,
        Quantity quantity)
    {
        if (string.IsNullOrWhiteSpace(productName.Value))
        {
            return Result.Failure<OrderProduct>(OrderProductErrors.InvalidOrderProductName);
        }

        if (unitPrice.Amount <= 0)
        {
            return Result.Failure<OrderProduct>(OrderProductErrors.InvalidOrderProductUnitPrice);
        }

        if (quantity.Value <= 0)
        {
            return Result.Failure<OrderProduct>(OrderProductErrors.InvalidOrderProductQuantity);
        }


        Result<Money> unitPriceResult = Money.Create(unitPrice.Amount, unitPrice.Currency);
        if (unitPriceResult.IsFailure)
        {
            return Result.Failure<OrderProduct>(unitPriceResult.Error);
        }

        Result<Money> totalPriceResult = Money.Create(unitPrice.Amount * quantity.Value, unitPrice.Currency);
        if (totalPriceResult.IsFailure)
        {
            return Result.Failure<OrderProduct>(totalPriceResult.Error);
        }

        var orderProduct = new OrderProduct(
            Guid.NewGuid(),
            orderId,
            productId,
            productName,
            unitPriceResult.Value,
            quantity,
            totalPriceResult.Value
        );

        return Result.Success(orderProduct);
    }

    public Result UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity.Value <= 0)
        {
            return Result.Failure(OrderProductErrors.InvalidOrderProductQuantity);
        }

        Quantity = newQuantity;
        RecalculateTotalPrice();

        return Result.Success();
    }

    public Result UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice.Amount <= 0)
        {
            return Result.Failure(OrderProductErrors.InvalidOrderProductUnitPrice);
        }

        UnitPrice = newUnitPrice;
        RecalculateTotalPrice();

        return Result.Success();
    }

    private void RecalculateTotalPrice()
    {
        Result<Money> totalPriceResult = Money.Create(UnitPrice.Amount * Quantity.Value, UnitPrice.Currency);
        if (totalPriceResult.IsSuccess)
        {
            TotalPrice = totalPriceResult.Value;
        }
    }
}
