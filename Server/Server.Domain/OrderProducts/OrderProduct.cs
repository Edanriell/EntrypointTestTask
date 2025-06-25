using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Domain.OrderItems;

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

        var orderProduct = new OrderProduct(
            Guid.NewGuid(),
            orderId,
            productId,
            productName,
            unitPrice,
            quantity,
            new Money(unitPrice.Amount * quantity.Value, unitPrice.Currency)
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
        RecalculateTotal();

        return Result.Success();
    }

    public Result UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice.Amount <= 0)
        {
            return Result.Failure(OrderProductErrors.InvalidOrderProductUnitPrice);
        }

        UnitPrice = newUnitPrice;
        RecalculateTotal();

        return Result.Success();
    }

    private void RecalculateTotal()
    {
        TotalPrice = new Money(UnitPrice.Amount * Quantity.Value, UnitPrice.Currency);
    }
}
