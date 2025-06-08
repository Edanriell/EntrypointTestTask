using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
// For Caching
using Microsoft.Extensions.Caching.Memory;
// For OrderBy
using System.Linq.Dynamic.Core;
// For Annotations
using Swashbuckle.AspNetCore.Annotations;
// For Predicate (advanced filtering)
using LinqKit;

using Server.Entities;
using Server.DTO.Shared;
using Server.DTO.Orders;
using Server.DTO.Products;
using Server.DTO.Users;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMemoryCache _memoryCache;

        public OrdersController(
            ApplicationDbContext context,
            ILogger<OrdersController> logger,
            IMemoryCache memoryCache
        )
        {
            _context = context;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetOrders")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a list of orders.",
            Description = "Retrieves a list of orders."
                + "with custom paging, sorting, and filtering rules."
        )]
        [SwaggerResponse(200, "Success", typeof(RestDTO<OrderDTO[]>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestDTO<OrderDTO[]>>> GetOrders(
            [FromQuery] GetRequestDTO<OrderDTO, OrdersFiltersDTO> input
        )
        {
            (int recordCount, Order[]? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context!.Orders
                        ?.Include(order => order.User)
                        ?.Include(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product)
                        .AsQueryable();

                    var predicate = PredicateBuilder.New<Order>(true);

                    #region Custom Orders Filters
                    if (input.CustomFilters is not null)
                    {
                        if (input.CustomFilters.Status is not null)
                            predicate = predicate.And(
                                order => order.Status == input.CustomFilters.Status.Value
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.ShipAddress))
                            predicate = predicate.And(
                                order => order.ShipAddress.Contains(input.CustomFilters.ShipAddress)
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.UserName))
                            predicate = predicate.And(
                                order =>
                                    order.User!.Name
                                        .ToLower()
                                        .Contains(input.CustomFilters.UserName.ToLower())
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.UserSurname))
                            predicate = predicate.And(
                                order =>
                                    order.User!.Surname
                                        .ToLower()
                                        .Contains(input.CustomFilters.UserSurname.ToLower())
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.UserEmail))
                            predicate = predicate.And(
                                order =>
                                    order.User!.Email!
                                        .ToLower()
                                        .Contains(input.CustomFilters.UserEmail.ToLower())
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.ProductName))
                            predicate = predicate.And(
                                order =>
                                    order.OrderProducts!.Any(
                                        productOrderLink =>
                                            productOrderLink.Product!.ProductName
                                                .ToLower()
                                                .Contains(input.CustomFilters.ProductName.ToLower())
                                    )
                            );
                    }
                    #endregion

                    dataTuple.recordCount = await query!.Where(predicate).CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested orders are not found." }
                        );
                    }

                    query = query!
                        .Where(predicate)
                        .OrderBy($"{input.SortColumn} {input.SortOrder}")
                        .Skip(input.PageIndex * input.PageSize)
                        .Take(input.PageSize);

                    dataTuple.result = await query.ToArrayAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
                }

                return new RestDTO<OrderDTO[]>()
                {
                    Data = dataTuple.result!.Select(order => MapToOrderDTO(order)).ToArray(),
                    PageIndex = input.PageIndex,
                    PageSize = input.PageSize,
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(
                                null,
                                "Orders",
                                new { input.PageIndex, input.PageSize },
                                Request.Scheme
                            )!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetOrders.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet(Name = "GetAllOrders")]
        [ResponseCache(CacheProfileName = "Client-120")]
        [SwaggerOperation(
            Summary = "Get a full list of orders.",
            Description = "Retrieves a full list of orders."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<OrderDTO[]>))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<OrderDTO[]>>> GetAllOrders()
        {
            (int recordCount, Order[]? result) dataTuple = (0, null);
            var cacheKey = "AllOrders";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context.Orders
                        .Include(order => order.User)
                        .Include(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested orders are not found.", }
                        );
                    }

                    dataTuple.result = await query!.ToArrayAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
                }

                return new RestBasicDTO<OrderDTO[]>()
                {
                    Data = dataTuple.result!.Select(order => MapToOrderDTO(order)).ToArray(),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Orders", null, Request.Scheme)!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetAllOrders.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet(Name = "GetOrder")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a single order by order id.",
            Description = "Retrieves one order."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<OrderDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<OrderDTO>>> GetOrder(
            [FromQuery] GetOrderByIdDTO input
        )
        {
            (int recordCount, Order? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context.Orders
                        .Include(order => order.User)
                        .Include(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product)
                        .Where(order => order.Id == input.Id);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO
                            {
                                Message = $"Requested order is not found. Order id {input.Id}.",
                            }
                        );
                    }

                    dataTuple.result = await query!.FirstOrDefaultAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
                }

                return new RestBasicDTO<OrderDTO>()
                {
                    Data = MapToOrderDTO(dataTuple.result!),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Orders", new { input.Id }, Request.Scheme)!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetOrder.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPost(Name = "CreateOrder")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(Summary = "Create new order.", Description = "Creates an new order.")]
        [SwaggerResponse(201, "Created", typeof(RestBasicDTO<OrderDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<OrderDTO>>> CreateOrder(
            [FromForm] CreateOrderDTO input
        )
        {
            try
            {
                var newOrder = new Order
                {
                    UserId = input.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ShipAddress = input.ShipAddress,
                    OrderInformation = input.OrderInformation,
                    OrderProducts = input.ProductIdsWithQuantities
                        .Select(
                            product =>
                                new ProductOrderLink
                                {
                                    ProductId = product.ProductId,
                                    Quantity = product.Quantity,
                                }
                        )
                        .ToList()
                };

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                var savedOrder = await _context.Orders
                    .Include(order => order.User)
                    .Include(order => order.OrderProducts)!
                    .ThenInclude(productOrderLink => productOrderLink.Product)
                    .FirstOrDefaultAsync(order => order.Id == newOrder.Id);

                return Created(
                    nameof(CreateOrder),
                    new RestBasicDTO<OrderDTO>()
                    {
                        Data = MapToOrderDTO(savedOrder!),
                        RecordCount = 1,
                        Links = new List<LinkDTO>
                        {
                            new LinkDTO(
                                Url.Action(null, "Orders", null, Request.Scheme)!,
                                "self",
                                "POST"
                            ),
                        }
                    }
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing CreateOrder.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPut(Name = "UpdateOrder")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Update existing order by order id.",
            Description = "Updates order by order id."
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> UpdateOrder([FromForm] UpdateOrderDTO input)
        {
            try
            {
                var order = await _context.Orders
                    .Where(order => order.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (order is not null)
                {
                    order.Status = input.Status;
                    order.OrderInformation = input.OrderInformation!;

                    order.UpdatedAt = DateTime.UtcNow;

                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return NotFound(
                        new NotFoundDTO { Message = $"Order with id '{input.Id}' is not found.", }
                    );
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing UpdateOrder.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "DeleteOrder")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Delete order by order id.",
            Description = "Deletes order by order id."
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> DeleteOrder([FromBody] DeleteOrderDTO input)
        {
            try
            {
                var order = await _context.Orders
                    .Where(order => order.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (order is null)
                    return NotFound(
                        new NotFoundDTO { Message = $"Order with id {input.Id} is not found.", }
                    );

                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing DeleteOrder.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "WipeOutAllOrders")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Deletes all orders from database. !USE ONLY FOR DEV PURPOSES!",
            Description = "Deletes all orders. !USE ONLY FOR DEV PURPOSES!"
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> WipeOutAllOrders()
        {
            try
            {
                var orders = await _context.Orders.ToListAsync();

                if (orders.Count == 0)
                    return NotFound(
                        new NotFoundDTO { Message = "There is ain't no orders in database.", }
                    );

                _context.Orders.RemoveRange(orders);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing WipeOutAllOrders.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        private OrderDTO MapToOrderDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ShipAddress = order.ShipAddress,
                OrderInformation = order.OrderInformation,
                Customer = new CustomerDTO
                {
                    Id = order.User!.Id,
                    Name = order.User.Name,
                    Surname = order.User.Surname,
                    Email = order.User.Email,
                    Address = order.User.Address,
                    BirthDate = order.User.BirthDate,
                    Gender = order.User.Gender,
                },
                Products = order.OrderProducts!
                    .Select(
                        productOrderLink =>
                            new ProductBasicDTO
                            {
                                Id = productOrderLink.ProductId,
                                ProductName = productOrderLink.Product!.ProductName,
                                Quantity = productOrderLink.Quantity,
                                UnitPrice = productOrderLink.Product.UnitPrice,
                            }
                    )
                    .ToList()
            };
        }
    }
}

