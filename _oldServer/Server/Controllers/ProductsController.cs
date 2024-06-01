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
using Server.DTO.Products;
using System.Numerics;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMemoryCache _memoryCache;

        public ProductsController(
            ApplicationDbContext context,
            ILogger<OrdersController> logger,
            IMemoryCache memoryCahce
        )
        {
            _context = context;
            _logger = logger;
            _memoryCache = memoryCahce;
        }

        [HttpGet(Name = "GetProducts")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a list of products.",
            Description = "Retrieves a list of products."
                + "with custom paging, sorting, and filtering rules."
        )]
        [SwaggerResponse(200, "Success", typeof(RestDTO<ProductDTO[]>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestDTO<ProductDTO[]>>> GetProducts(
            [FromQuery] GetRequestDTO<ProductDTO, ProductsFiltersDTO> input
        )
        {
            (int recordCount, Product[]? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context!.Products
                        ?.Include(product => product.ProductOrders)!
                        .ThenInclude(productOrderLink => productOrderLink.Order)
                        .ThenInclude(order => order!.User)
                        .AsQueryable();

                    var predicate = PredicateBuilder.New<Product>(true);

                    #region Custom Products Filters
                    if (input.CustomFilters is not null)
                    {
                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.Code))
                            predicate = predicate.And(
                                product =>
                                    product.Code
                                        .ToLower()
                                        .Contains(input.CustomFilters.Code.ToLower())
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.ProductName))
                            predicate = predicate.And(
                                product =>
                                    product.ProductName
                                        .ToLower()
                                        .Contains(input.CustomFilters.ProductName.ToLower())
                            );

                        if (input.CustomFilters.UnitsInStock >= 0)
                            predicate = predicate.And(
                                product => product.UnitsInStock >= input.CustomFilters.UnitsInStock
                            );

                        if (input.CustomFilters.UnitsOnOrder >= 0)
                            predicate = predicate.And(
                                product => product.UnitsOnOrder >= input.CustomFilters.UnitsOnOrder
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.CustomerName))
                            predicate = predicate.And(
                                product =>
                                    product.ProductOrders!.Any(
                                        productOrderLink =>
                                            productOrderLink.Order!.User!.Name
                                                .ToLower()
                                                .Contains(
                                                    input.CustomFilters.CustomerName.ToLower()
                                                )
                                    )
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.CustomerSurname))
                            predicate = predicate.And(
                                product =>
                                    product.ProductOrders!.Any(
                                        productOrderLink =>
                                            productOrderLink.Order!.User!.Surname
                                                .ToLower()
                                                .Contains(
                                                    input.CustomFilters.CustomerSurname.ToLower()
                                                )
                                    )
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.CustomerEmail))
                            predicate = predicate.And(
                                product =>
                                    product.ProductOrders!.Any(
                                        productOrderLink =>
                                            productOrderLink.Order!.User!.Email!
                                                .ToLower()
                                                .Contains(
                                                    input.CustomFilters.CustomerEmail.ToLower()
                                                )
                                    )
                            );
                    }
                    #endregion

                    dataTuple.recordCount = await query!.Where(predicate).CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested products are not found." }
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

                return new RestDTO<ProductDTO[]>()
                {
                    Data = dataTuple.result!.Select(product => MapToProductDTO(product)).ToArray(),
                    PageIndex = input.PageIndex,
                    PageSize = input.PageSize,
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(
                                null,
                                "Products",
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
                _logger.LogError(exception, "Error occurred while processing GetProducts.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet(Name = "GetAllProducts")]
        [ResponseCache(CacheProfileName = "Client-120")]
        [SwaggerOperation(
            Summary = "Get a full list of products.",
            Description = "Retrieves a full list of products."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<ProductDTO[]>))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<ProductDTO[]>>> GetAllProducts()
        {
            (int recordCount, Product[]? result) dataTuple = (0, null);
            var cacheKey = "AllProducts";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context.Products
                        .Include(product => product.ProductOrders)!
                        .ThenInclude(productOrderLink => productOrderLink.Order)
                        .ThenInclude(order => order!.User);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested products are not found.", }
                        );
                    }

                    dataTuple.result = await query!.ToArrayAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
                }

                return new RestBasicDTO<ProductDTO[]>()
                {
                    Data = dataTuple.result!.Select(product => MapToProductDTO(product)).ToArray(),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Products", null, Request.Scheme)!,
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

        [HttpGet(Name = "GetProduct")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a single product by product id.",
            Description = "Retrieves one product."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<ProductDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<ProductDTO>>> GetProduct(
            [FromQuery] GetProductByIdDTO input
        )
        {
            (int recordCount, Product? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context.Products
                        .Include(product => product.ProductOrders)!
                        .ThenInclude(productOrderLink => productOrderLink.Order)
                        .ThenInclude(order => order!.User)
                        .Where(product => product.Id == product.Id);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO
                            {
                                Message = $"Requested product is not found. Product id {input.Id}.",
                            }
                        );
                    }

                    dataTuple.result = await query!.FirstOrDefaultAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
                }

                return new RestBasicDTO<ProductDTO>()
                {
                    Data = MapToProductDTO(dataTuple.result!),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Products", new { input.Id }, Request.Scheme)!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetProduct.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPost(Name = "CreateProduct")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(Summary = "Create new product.", Description = "Creates an new product.")]
        [SwaggerResponse(201, "Created", typeof(RestBasicDTO<ProductDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<ProductDTO>>> CreateProduct(
            [FromForm] CreateProductDTO input
        )
        {
            try
            {
                var newProduct = new Product
                {
                    Code = input.Code,
                    ProductName = input.ProductName,
                    Description = input.Description,
                    UnitPrice = input.UnitPrice,
                    UnitsInStock = input.UnitsInStock,
                    UnitsOnOrder = input.UnitsOnOrder,
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                var savedProduct = await _context.Products
                    .Include(product => product.ProductOrders)!
                    .ThenInclude(productOrderLink => productOrderLink.Order)
                    .ThenInclude(order => order!.User)
                    .FirstOrDefaultAsync(product => product.Id == newProduct.Id);

                return Created(
                    nameof(CreateProduct),
                    new RestBasicDTO<ProductDTO>()
                    {
                        Data = MapToProductDTO(savedProduct!),
                        RecordCount = 1,
                        Links = new List<LinkDTO>
                        {
                            new LinkDTO(
                                Url.Action(null, "Products", null, Request.Scheme)!,
                                "self",
                                "POST"
                            ),
                        }
                    }
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing CreateProduct.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPut(Name = "UpdateProduct")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Update existing product by product id.",
            Description = "Updates product by product id."
        )]
        [SwaggerResponse(204, "NoContentResult", typeof(RestBasicDTO<ProductDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> UpdateProduct([FromForm] UpdateProductDTO input)
        {
            try
            {
                var product = await _context.Products
                    .Where(product => product.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (product is not null)
                {
                    product.Code = input.Code;
                    product.ProductName = input.ProductName;
                    product.Description = input.Description;
                    product.UnitPrice = input.UnitPrice;
                    product.UnitsInStock = input.UnitsInStock;
                    product.UnitsOnOrder = input.UnitsOnOrder;

                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return NotFound(
                        new NotFoundDTO { Message = $"Product with id '{input.Id}' is not found.", }
                    );
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing UpdateProduct.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "DeleteProduct")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Delete product by product id.",
            Description = "Deletes product by product id."
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> DeleteOrder([FromBody] DeleteProductDTO input)
        {
            try
            {
                var product = await _context.Products
                    .Where(product => product.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (product is null)
                    return NotFound(
                        new NotFoundDTO { Message = $"Product with id {input.Id} is not found.", }
                    );

                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing DeleteProduct.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "WipeOutAllProducts")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Deletes all products from database. !USE ONLY FOR DEV PURPOSES!",
            Description = "Deletes all products. !USE ONLY FOR DEV PURPOSES!"
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> WipeOutAllProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                if (products.Count == 0)
                    return NotFound(
                        new NotFoundDTO { Message = "There is ain't no products in database.", }
                    );

                _context.Products.RemoveRange(products);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing WipeOutAllProducts.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        private ProductDTO MapToProductDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Code = product.Code,
                ProductName = product.ProductName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
                ProductOrders = product.ProductOrders!
                    .Select(
                        productOrderLink =>
                            new ProductOrdersDTO
                            {
                                Id = productOrderLink.OrderId,
                                OrderStatus = productOrderLink.Order!.Status,
                                CustomerName = productOrderLink.Order!.User!.Name,
                                CustomerSurname = productOrderLink.Order!.User!.Surname,
                                CustomerEmail = productOrderLink.Order!.User!.Email,
                                CustomerPhoneNumber = productOrderLink.Order!.User!.PhoneNumber,
                                CustomerAddress = productOrderLink.Order!.User!.Address,
                                ProductOrderQuantity = productOrderLink.Quantity
                            }
                    )
                    .ToList()
            };
        }
    }
}

