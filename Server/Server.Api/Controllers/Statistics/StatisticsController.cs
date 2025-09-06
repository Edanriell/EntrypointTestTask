using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Statistics.GetActiveProducts;
using Server.Application.Statistics.GetCustomerGrowthAndOrderVolume;
using Server.Application.Statistics.GetInventoryStatus;
using Server.Application.Statistics.GetLowStockAlerts;
using Server.Application.Statistics.GetMonthlyRevenue;
using Server.Application.Statistics.GetOrdersAndRevenueTrend;
using Server.Application.Statistics.GetOrderStatusDistribution;
using Server.Application.Statistics.GetRecentOrders;
using Server.Application.Statistics.GetTopSellingProducts;
using Server.Application.Statistics.GetTotalCustomers;
using Server.Application.Statistics.GetTotalOrders;
using Server.Domain.Abstractions;

namespace Server.Api.Controllers.Statistics;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly ISender _sender;

    public StatisticsController(ISender sender) { _sender = sender; }

    [HttpGet("orders/total")]
    public async Task<IActionResult> GetTotalOrders(
        CancellationToken cancellationToken)
    {
        var query = new GetTotalOrdersQuery();

        Result<GetTotalOrdersResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("products/active")]
    public async Task<IActionResult> GetActiveProducts(
        CancellationToken cancellationToken)
    {
        var query = new GetActiveProductsQuery();

        Result<GetActiveProductsResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("customers/total")]
    public async Task<IActionResult> GetTotalCustomers(
        CancellationToken cancellationToken)
    {
        var query = new GetTotalCustomersQuery();

        Result<GetTotalCustomersResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetMonthlyRevenue(
        CancellationToken cancellationToken)
    {
        var query = new GetMonthlyRevenueQuery();

        Result<GetMonthlyRevenueResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("orders/revenue-trend")]
    public async Task<IActionResult> GetOrdersAndRevenueTrend(
        CancellationToken cancellationToken)
    {
        var query = new GetOrdersAndRevenueTrendQuery();

        Result<GetOrdersAndRevenueTrendResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("orders/status")]
    public async Task<IActionResult> GetOrderStatusDistribution(
        CancellationToken cancellationToken)
    {
        var query = new GetOrderStatusDistributionQuery();

        Result<GetOrderStatusDistributionResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("products/top-selling")]
    public async Task<IActionResult> GetTopSellingProducts(
        CancellationToken cancellationToken)
    {
        var query = new GetTopSellingProductsQuery();

        Result<GetTopSellingProductsResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("inventory/status")]
    public async Task<IActionResult> GetInventoryStatus(
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryStatusQuery();

        Result<GetInventoryStatusResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("customers/growth-orders")]
    public async Task<IActionResult> GetCustomerGrowthAndOrderVolume(
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerGrowthAndOrderVolumeQuery();

        Result<GetCustomerGrowthAndOrderVolumeResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("products/low-stock-alerts")]
    public async Task<IActionResult> GetLowStockAlerts(
        CancellationToken cancellationToken)
    {
        var query = new GetLowStockAlertsQuery();

        Result<GetLowStockAlertsResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("orders/recent")]
    public async Task<IActionResult> GetRecentOrders(
        CancellationToken cancellationToken)
    {
        var query = new GetRecentOrdersQuery();

        Result<GetRecentOrdersResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
