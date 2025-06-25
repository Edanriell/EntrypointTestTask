using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderItems;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;

namespace Server.Application.Orders.AddProductToOrder;

// This commands needs testing !
internal sealed class AddProductToOrderCommandHandler : ICommandHandler<AddProductToOrderCommand>
{
    private readonly OrderProductService _orderProductService;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductToOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        OrderProductService orderProductService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderProductService = orderProductService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AddProductToOrderCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound);
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.CannotModifyNonPendingOrder);
        }

        // Convert request items to the format expected by OrderProductService
        var orderItems = new List<(Guid ProductId, Quantity Quantity)>();
        foreach (ProductItem product in request.Products)
        {
            Result<Quantity> quantityResult = Quantity.CreateQuantity(product.Quantity);
            if (quantityResult.IsFailure)
            {
                return Result.Failure(quantityResult.Error);
            }

            orderItems.Add((product.ProductId, quantityResult.Value));
        }

        // Create order products with stock reservation
        Result<List<OrderProduct>> orderProductsResult = await _orderProductService
            .CreateOrderProductsAsync(
                request.OrderId,
                orderItems,
                _productRepository,
                cancellationToken);

        if (orderProductsResult.IsFailure)
        {
            return Result.Failure(orderProductsResult.Error);
        }

        // Add products to the order
        foreach (OrderProduct orderProduct in orderProductsResult.Value)
        {
            Result addResult = order.AddProduct(orderProduct);
            if (addResult.IsFailure)
            {
                return Result.Failure(addResult.Error);
            }
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
