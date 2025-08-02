using Server.Domain.Abstractions;

namespace Server.Domain.Products;

public static class ProductErrors
{
    public static readonly Error NotFound = new(
        "Product.NotFound",
        "The product with the specified identifier was not found"
    );

    public static readonly Error AlreadyDeleted = new(
        "Product.AlreadyDeleted",
        "Product is already deleted");

    public static readonly Error CannotDeleteProductWithStock = new(
        "Product.CannotDeleteWithStock",
        "Cannot delete product that has stock remaining");

    public static readonly Error CannotDeleteProductWithActiveOrders = new(
        "Product.CannotDeleteWithActiveOrders",
        "Cannot delete product that has active orders");

    public static readonly Error CannotReserveProduct = new(
        "Product.CannotReserveProduct",
        "Cannot reserve product, because not enough quantity is available");

    public static readonly Error CannotUnreserveReservedStock = new(
        "Product.CannotUnreserveReservedStock",
        "Reserved stock value cannot be less than zero");

    public static readonly Error UnableToRestoreProduct = new(
        "Product.UnableToRestoreProduct",
        "Unable to restore product, because it is not deleted");

    public static readonly Error CouldNotUpdateProduct = new(
        "Product.CouldNotUpdateProduct",
        "Could not update deleted product");

    public static readonly Error ProductStockCannotBeNegative = new(
        "Product.StockCannotBeNegative",
        "Product stock cannot be less than zero");

    public static readonly Error InvalidQuantity = new(
        "Product.InvalidQuantity",
        "Quantity must be greater than zero");

    public static readonly Error InsufficientStock = new(
        "Product.InsufficientStock",
        "Insufficient stock available");

    public static readonly Error CannotReleaseMoreThanReserved = new(
        "Product.CannotReleaseMoreThanReserved",
        "Cannot release more than reserved");

    public static readonly Error CannotReduceStockBelowReservedAmount = new(
        "Product.CannotReduceStockBelowReservedAmount",
        "Cannot reduce stock below reserved amount");

    public static readonly Error CannotDeleteProductWithActiveReservations = new(
        "Product.CannotDeleteProductWithActiveReservations",
        "Cannot delete product with active reservations");

    public static readonly Error ProductNotAvailable = new(
        "Products.ProductNotAvailable",
        "The product is not available for purchase");
}
