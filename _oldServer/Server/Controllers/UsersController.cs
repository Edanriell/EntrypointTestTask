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
using Server.Shared.Enums;
using System.Net;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMemoryCache _memoryCache;

        public UsersController(
            ApplicationDbContext context,
            ILogger<OrdersController> logger,
            IMemoryCache memoryCahce
        )
        {
            _context = context;
            _logger = logger;
            _memoryCache = memoryCahce;
        }

        [HttpGet(Name = "GetUsers")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a list of users.",
            Description = "Retrieves a list of users."
                + "with custom paging, sorting, and filtering rules."
        )]
        [SwaggerResponse(200, "Success", typeof(RestDTO<CustomerDTO[]>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestDTO<CustomerDTO[]>>> GetUsers(
            [FromQuery] GetRequestDTO<CustomerDTO, UsersFiltersDTO> input
        )
        {
            (int recordCount, User[]? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context!.Users
                        ?.Include(user => user.Orders)!
                        .ThenInclude(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product)
                        .AsQueryable();

                    var predicate = PredicateBuilder.New<User>(true);

                    #region Custom Users Filters
                    if (input.CustomFilters is not null)
                    {
                        if (input.CustomFilters.Role >= 0)
                            predicate = predicate.And(
                                user => user.Role == input.CustomFilters.Role
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.Name))
                            predicate = predicate.And(
                                user => user.Name.ToLower() == input.CustomFilters.Name!.ToLower()
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.Surname))
                            predicate = predicate.And(
                                user =>
                                    user.Surname.ToLower() == input.CustomFilters.Surname!.ToLower()
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.Email))
                            predicate = predicate.And(
                                user =>
                                    user.Email!.ToLower() == input.CustomFilters.Email!.ToLower()
                            );

                        //if (!string.IsNullOrWhiteSpace(input.CustomFilters.Username))
                        //    predicate = predicate.And(
                        //        user =>
                        //            user.Username.ToLower()
                        //            == input.CustomFilters.Username!.ToLower()
                        //    );

                        if (input.CustomFilters.Gender >= 0)
                            predicate = predicate.And(
                                user => user.Gender == input.CustomFilters.Gender
                            );

                        if (input.CustomFilters.CreatedAt is not null)
                            predicate = predicate.And(
                                user =>
                                    user.Orders!.Any(
                                        order => order.CreatedAt == input.CustomFilters.CreatedAt
                                    )
                            );

                        if (input.CustomFilters.UpdatedAt is not null)
                            predicate = predicate.And(
                                user =>
                                    user.Orders!.Any(
                                        order => order.UpdatedAt == input.CustomFilters.UpdatedAt
                                    )
                            );

                        if (input.CustomFilters.Status >= 0)
                            predicate = predicate.And(
                                user =>
                                    user.Orders!.Any(
                                        order => order.Status == input.CustomFilters.Status
                                    )
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.ProductCode))
                            predicate = predicate.And(
                                user =>
                                    user.Orders!.Any(
                                        order =>
                                            order.OrderProducts!.Any(
                                                productOrderLink =>
                                                    productOrderLink.Product!.Code
                                                        .ToLower()
                                                        .Contains(
                                                            input.CustomFilters.ProductCode.ToLower()
                                                        )
                                            )
                                    )
                            );

                        if (!string.IsNullOrWhiteSpace(input.CustomFilters.ProductName))
                            predicate = predicate.And(
                                user =>
                                    user.Orders!.Any(
                                        order =>
                                            order.OrderProducts!.Any(
                                                productOrderLink =>
                                                    productOrderLink.Product!.ProductName
                                                        .ToLower()
                                                        .Contains(
                                                            input.CustomFilters.ProductName.ToLower()
                                                        )
                                            )
                                    )
                            );
                    }
                    #endregion

                    dataTuple.recordCount = await query!.Where(predicate).CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested users are not found." }
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

                return new RestDTO<CustomerDTO[]>()
                {
                    Data = dataTuple.result!.Select(user => MapToUserDTO(user)).ToArray(),
                    PageIndex = input.PageIndex,
                    PageSize = input.PageSize,
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(
                                null,
                                "Users",
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
                _logger.LogError(exception, "Error occurred while processing GetUsers.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet(Name = "GetAllUsers")]
        [ResponseCache(CacheProfileName = "Client-120")]
        [SwaggerOperation(
            Summary = "Get a full list of users.",
            Description = "Retrieves a full list of users."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<CustomerDTO[]>))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<CustomerDTO[]>>> GetAllUsers()
        {
            (int recordCount, User[]? result) dataTuple = (0, null);
            var cacheKey = "AllUsers";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context!.Users
                        ?.Include(user => user.Orders)!
                        .ThenInclude(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO { Message = "Requested users are not found.", }
                        );
                    }

                    dataTuple.result = await query!.ToArrayAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
                }

                return new RestBasicDTO<CustomerDTO[]>()
                {
                    Data = dataTuple.result!.Select(user => MapToUserDTO(user)).ToArray(),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Users", null, Request.Scheme)!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetAllUsers.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet(Name = "GetUser")]
        [ResponseCache(CacheProfileName = "Any-60")]
        [SwaggerOperation(
            Summary = "Get a single user by user id.",
            Description = "Retrieves one user."
        )]
        [SwaggerResponse(200, "Success", typeof(RestBasicDTO<CustomerDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<CustomerDTO>>> GetUser(
            [FromQuery] GetUserByIdDTO input
        )
        {
            (int recordCount, User? result) dataTuple = (0, null);
            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";

            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out dataTuple))
                {
                    var query = _context!.Users
                        ?.Include(user => user.Orders)!
                        .ThenInclude(order => order.OrderProducts)!
                        .ThenInclude(productOrderLink => productOrderLink.Product)
                        .Where(order => order.Id == input.Id);

                    dataTuple.recordCount = await query!.CountAsync();

                    if (dataTuple.recordCount == 0)
                    {
                        return NotFound(
                            new NotFoundDTO
                            {
                                Message = $"Requested user is not found. User id {input.Id}.",
                            }
                        );
                    }

                    dataTuple.result = await query!.FirstOrDefaultAsync();

                    _memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
                }

                return new RestBasicDTO<CustomerDTO>()
                {
                    Data = MapToUserDTO(dataTuple.result!),
                    RecordCount = dataTuple.recordCount,
                    Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null, "Users", new { input.Id }, Request.Scheme)!,
                            "self",
                            "GET"
                        ),
                    }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing GetUser.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPost(Name = "CreateUser")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(Summary = "Create new user.", Description = "Creates an new user.")]
        [SwaggerResponse(201, "Created", typeof(RestBasicDTO<CustomerDTO>))]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult<RestBasicDTO<CustomerDTO>>> CreateUser(
            [FromForm] CreateUserDTO input
        )
        {
            try
            {
                var newUser = new User
                {
                    Name = input.Name,
                    Surname = input.Surname,
                    Email = input.Email,
                    // Username = input.Username,
                    PhoneNumber = input.PhoneNumber,
                    Address = input.Address,
                    BirthDate = input.BirthDate,
                    Gender = input.Gender,
                    Photo = await ConvertIFormFileToByteArray(input.Photo!)
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var savedUser = await _context!.Users
                    ?.Include(user => user.Orders)!
                    .ThenInclude(order => order.OrderProducts)!
                    .ThenInclude(productOrderLink => productOrderLink.Product)!
                    .FirstOrDefaultAsync(user => user.Id == newUser.Id)!;

                return Created(
                    nameof(CreateUser),
                    new RestBasicDTO<CustomerDTO>()
                    {
                        Data = MapToUserDTO(savedUser!),
                        RecordCount = 1,
                        Links = new List<LinkDTO>
                        {
                            new LinkDTO(
                                Url.Action(null, "Users", null, Request.Scheme)!,
                                "self",
                                "POST"
                            ),
                        }
                    }
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing CreateUser.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPut(Name = "UpdateUser")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Update existing user by user id.",
            Description = "Updates user by user id."
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> UpdateUser([FromForm] UpdateUserDTO input)
        {
            try
            {
                var user = await _context.Users
                    .Where(user => user.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (user is not null)
                {
                    user.Name = input.Name;
                    user.Surname = input.Surname;
                    user.Email = input.Email;
                    //user.Username = input.Username;
                    user.PhoneNumber = input.PhoneNumber;
                    user.Address = input.Address;
                    user.BirthDate = input.BirthDate;
                    user.Gender = input.Gender;
                    user.Photo = await ConvertIFormFileToByteArray(input.Photo!);

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return NotFound(
                        new NotFoundDTO { Message = $"User with id '{input.Id}' is not found.", }
                    );
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing UpdateUser.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "DeleteUser")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Delete user by user id.",
            Description = "Deletes user by user id."
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserDTO input)
        {
            try
            {
                var user = await _context.Users
                    .Where(user => user.Id == input.Id)
                    .FirstOrDefaultAsync();

                if (user is null)
                    return NotFound(
                        new NotFoundDTO { Message = $"User with id {input.Id} is not found.", }
                    );

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing DeleteUser.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        [HttpDelete(Name = "WipeOutAllUsers")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [SwaggerOperation(
            Summary = "Deletes all users from database. !USE ONLY FOR DEV PURPOSES!",
            Description = "Deletes all users. !USE ONLY FOR DEV PURPOSES!"
        )]
        [SwaggerResponse(204, "NoContentResult")]
        [SwaggerResponse(400, "Bad Request", typeof(BadRequestDTO))]
        [SwaggerResponse(404, "Not Found", typeof(NotFoundDTO))]
        [SwaggerResponse(500, "Internal Server Error", typeof(InternalServerErrorDTO))]
        public async Task<ActionResult> WipeOutAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                if (users.Count == 0)
                    return NotFound(
                        new NotFoundDTO { Message = "There is ain't no users in database.", }
                    );

                _context.Users.RemoveRange(users);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occurred while processing WipeOutAllUsers.");

                return StatusCode(
                    500,
                    new InternalServerErrorDTO { Message = "An unexpected error occurred." }
                );
            }
        }

        private async Task<byte[]> ConvertIFormFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private CustomerDTO MapToUserDTO(User user)
        {
            return new CustomerDTO
            {
                Id = user.Id,
                Role = user.Role,
                Name = user.Name,
                Surname = user.Surname,
                // Username = user.Username,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Photo = user.Photo,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Orders = user.Orders!
                    .Select(
                        order =>
                            new OrderDTO
                            {
                                Id = order.Id,
                                CreatedAt = order.CreatedAt,
                                UpdatedAt = order.UpdatedAt,
                                Status = order.Status,
                                ShipAddress = order.ShipAddress,
                                OrderInformation = order.OrderInformation,
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
                            }
                    )
                    .ToList(),
            };
        }
    }
}

