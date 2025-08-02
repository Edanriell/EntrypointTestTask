using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.OrderProducts;
using Server.Domain.Orders;
using Server.Domain.Products;
using Server.Domain.Shared;
using Server.Domain.Users;

namespace Server.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    // MANY PAYMENTS
    // private readonly OrderPaymentService _orderPaymentService;
    private readonly OrderProductService _orderProductService;

    private readonly IOrderRepository _orderRepository;

    // MANY PAYMENTS
    // private readonly IPaymentRepository _paymentRepository;
    private readonly IProductRepository _productRepository;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        ProductService productService,
        OrderProductService orderProductService,
        // MANY PAYMENTS
        // IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        // MANY PAYMENTS
        // _paymentRepository = paymentRepository;
        _orderProductService = orderProductService;
        _productService = productService;
        _unitOfWork = unitOfWork;
        // MANY PAYMENTS
        // _orderPaymentService = new OrderPaymentService();
    }

    public async Task<Result<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // If there is no order items return early
        if (request.OrderItems is null || request.OrderItems.Count == 0)
        {
            return Result.Failure<Guid>(OrderErrors.EmptyOrder);
        }

        Result<Currency> currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }

        // Get Client
        User? client = await _userRepository.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        // Create a new order number
        Result<OrderNumber> orderNumberResult = OrderNumber.Create(client.Id);
        if (orderNumberResult.IsFailure)
        {
            return Result.Failure<Guid>(orderNumberResult.Error);
        }

        OrderInfo orderInfoResult = null;
        if (request.Info is not null)
        {
            orderInfoResult = OrderInfo.Create(request.Info);
        }

        // Create a shipping address
        var shippingAddress = new Address(
            request.ShippingAddress.Country,
            request.ShippingAddress.City,
            request.ShippingAddress.ZipCode,
            request.ShippingAddress.Street);

        // Create an empty order without any products or payment
        Result<Order> orderResult = Order.Create(
            request.ClientId,
            orderNumberResult.Value,
            currencyResult.Value,
            orderInfoResult,
            shippingAddress);
        if (orderResult.IsFailure)
        {
            return Result.Failure<Guid>(orderResult.Error);
        }

        Order order = orderResult.Value;

        // Creating an empty orderItems list
        // Loop over all of the OrderItems and create quantity for each
        // Quantity value object is required to create OrderProduct
        var orderItems = new List<(Guid ProductId, Quantity Quantity)>();
        foreach (OrderItem item in request.OrderItems)
        {
            Result<Quantity> quantityResult = Quantity.CreateQuantity(item.Quantity);
            if (quantityResult.IsFailure)
            {
                return Result.Failure<Guid>(quantityResult.Error);
            }

            orderItems.Add((item.ProductId, quantityResult.Value));
        }

        // Use OrderProductService to create OrderProducts with all required product details
        Result<List<OrderProduct>> orderProductsResult = await _orderProductService
            .CreateOrderProductsAsync(order.Id, orderItems, _productRepository, cancellationToken);
        if (orderProductsResult.IsFailure)
        {
            return Result.Failure<Guid>(orderProductsResult.Error);
        }

        // Add newly created products to the order
        foreach (OrderProduct orderProduct in orderProductsResult.Value)
        {
            Result addProductResult = order.AddProduct(orderProduct);
            if (addProductResult.IsFailure)
            {
                return Result.Failure<Guid>(addProductResult.Error);
            }

            // This works TODO22 ?? 
            Result reserveStockResult =
                await _productService.ReserveStockAsync(orderProduct.ProductId, orderProduct.Quantity.Value);
            if (reserveStockResult.IsFailure)
            {
                return Result.Failure<Guid>(reserveStockResult.Error);
            }
        }

        // Create unpaid payment
        // MANY PAYMENTS
        // Result<Payment> paymentResult = Payment.Create(order.Id);
        // if (paymentResult.IsFailure)
        // {
        //     return Result.Failure<Guid>(paymentResult.Error);
        // }

        // MANY PAYMENTS
        // Payment payment = paymentResult.Value;
        // FIX ALL TODO222 Currency issues everything should work
        // Migrate to payments multiple !
        // TODO222 Test
        // MANY PAYMENTS
        // Result paymentResult2 = order.SetPaymentId(payment.Id);
        // if (paymentResult2.IsFailure)
        // {
        // return Result.Failure<Guid>(paymentResult2.Error);
        // }

        // Update payment TotalAmount using OrderPaymentService
        // MANY PAYMENTS
        // Result updatePaymentResult = _orderPaymentService.UpdatePaymentTotalFromOrder(payment, order);
        // if (updatePaymentResult.IsFailure)
        // {
        //     return Result.Failure<Guid>(updatePaymentResult.Error);
        // }

        // order.PaymentId = payment.Id;

        // Add entities to repositories for tracking
        // MANY PAYMENTS
        // _paymentRepository.Add(payment);
        _orderRepository.Add(order);

        // Save changes in database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(order.Id);
    }
}
