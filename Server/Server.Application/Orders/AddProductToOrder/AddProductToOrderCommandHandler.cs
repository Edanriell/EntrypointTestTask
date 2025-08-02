using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Orders.AddProductToOrder;

internal sealed class AddProductToOrderCommandHandler : ICommandHandler<AddProductToOrderCommand>
{
    private readonly IOrderProductRepository _orderProductRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductToOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IOrderProductRepository orderProductRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderProductRepository = orderProductRepository;
    }

    public async Task<Result> Handle(
        AddProductToOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.Products.Any())
        {
            return Result.Failure(OrderErrors.NoProductsToAdd);
        }

        // ✅ Get the order (tracked for modifications)
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        // ✅ Domain validation - can order be modified?
        if (!order.CanBeModified())
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        // ✅ Process each product using Rich Domain Model
        foreach (ProductItem productItem in request.Products)
        {
            // ✅ Create Quantity Value Object
            Result<Quantity> quantityResult = Quantity.CreateQuantity(productItem.Quantity);
            if (quantityResult.IsFailure)
            {
                return Result.Failure(quantityResult.Error);
            }

            Quantity requestedQuantity = quantityResult.Value;

            // ✅ Get product to validate it exists and get its properties
            Product? product = await _productRepository.GetByIdAsync(productItem.ProductId, cancellationToken);
            if (product is null)
            {
                return Result.Failure(OrderErrors.ProductNotFound);
            }

            // ✅ Check if product already exists in order
            if (order.HasProduct(productItem.ProductId))
            {
                // ✅ FEATURE: Increment quantity if product exists
                Result incrementResult = order.UpdateExistingProductQuantity(
                    productItem.ProductId,
                    requestedQuantity);

                if (incrementResult.IsFailure)
                {
                    return Result.Failure(incrementResult.Error);
                }

                // ✅ Update the existing OrderProduct in repository
                List<OrderProduct> existingOrderProducts =
                    await _orderProductRepository.GetByOrderIdAsync(order.Id, cancellationToken);

                OrderProduct? existingOrderProduct =
                    existingOrderProducts.FirstOrDefault(op => op.ProductId == productItem.ProductId);

                if (existingOrderProduct is not null)
                {
                    _orderProductRepository.Update(existingOrderProduct);
                }
            }
            else
            {
                // ✅ FEATURE: Add new product if it doesn't exist
                Result<OrderProduct> orderProductResult = OrderProduct.Create(
                    order.Id,
                    product.Id,
                    product.Name,
                    product.Price,
                    requestedQuantity);

                if (orderProductResult.IsFailure)
                {
                    return Result.Failure(orderProductResult.Error);
                }

                OrderProduct newOrderProduct = orderProductResult.Value;

                // ✅ Use Rich Domain Model to add product
                Result addResult = order.AddNewProduct(newOrderProduct);
                if (addResult.IsFailure)
                {
                    return Result.Failure(addResult.Error);
                }

                // ✅ Add to repository for persistence
                _orderProductRepository.Add(newOrderProduct);
            }
        }

        // ✅ Update order in repository (domain events and total recalculation handled by domain)
        _orderRepository.Update(order);

        // ✅ Save all changes in single transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
