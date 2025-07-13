using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Controllers.Customers;
using Server.Application.Users.GetClients;
using Server.Application.Users.GetUserById;
using Server.Application.Users.RegisterCustomer;
using Server.Domain.Abstractions;

namespace Server.Api.Controllers.Clients;
 
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/customers")]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender) { _sender = sender; }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] GetCustomersRequest request, CancellationToken cancellationToken = default)
    {
        var query = new GetCustomersQuery
        {
            PageSize = request.PageSize,
            Cursor = request.Cursor,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection,
            NameFilter = request.NameFilter,
            EmailFilter = request.EmailFilter,
            CountryFilter = request.CountryFilter,
            CityFilter = request.CityFilter,
            MinTotalSpent = request.MinTotalSpent,
            MaxTotalSpent = request.MaxTotalSpent,
            MinTotalOrders = request.MinTotalOrders,
            MaxTotalOrders = request.MaxTotalOrders,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore
        };

        Result<GetCustomersResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomerById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);

        Result<CustomerResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer(
        RegisterCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCustomerCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Gender,
            request.Country,
            request.City,
            request.ZipCode,
            request.Street,
            request.Password);

        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
