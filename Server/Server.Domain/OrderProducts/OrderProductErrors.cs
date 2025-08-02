using Server.Domain.Abstractions;

namespace Server.Domain.OrderProducts;

public static class OrderProductErrors
{
    public static readonly Error InvalidOrderProductQuantity = new(
        "OrderProduct.InvalidQuantity",
        "Quantity must be greater than zero");

    public static readonly Error InvalidOrderProductName = new(
        "OrderProduct.InvalidName",
        "Product name must be provided and it cannot be empty");

    public static readonly Error InvalidOrderProductUnitPrice = new(
        "OrderProduct.InvalidUnitPrice",
        "Unit price must be greater than zero");

    public static readonly Error InsufficientStock = new(
        "OrderProduct.InsufficientStock",
        "The requested quantity exceeds the available stock for this product");

    public static readonly Error MixedCurrenciesNotAllowed = new(
        "OrderProduct.MixedCurrenciesNotAllowed",
        "Cannot add products with different currencies to the same order");
}
