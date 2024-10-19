using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

public class OrdersController : ControllerBase
{
    private readonly Orders.OrdersClient _ordersClient;

    public OrdersController(Orders.OrdersClient ordersClient)
    {
        _ordersClient = ordersClient;
    }

    [HttpGet("SetOrderCancell")]
    public async Task<IActionResult> SetOrderCancellAsync(long orderNumber)
    {
        var result = await _ordersClient.SetOrderCancellAsync(new SetOrderCancellRequest() { Number = orderNumber });

        return Ok(result);
    }

    [HttpGet("GetOrderStatus")]
    public async Task<IActionResult> GetOrderStatusAsync(long orderNumber)
    {
        var result = await _ordersClient.GetOrderStatusAsync(new GetOrderStatusRequest() { Number = orderNumber });

        return Ok(result);
    }

    [HttpGet("GetRegions")]
    public async Task<IActionResult> GetRegionsAsync()
    {
        var result = await _ordersClient.GetRegionsAsync(new Google.Protobuf.WellKnownTypes.Empty());

        return Ok(result);
    }

    [HttpPost("GetOrders")]
    public async Task<IActionResult> GetOrdersAsync(OrdersRequestDto requestDto)
    {
        var request = new GetOrdersRequest();
        request.Regions.AddRange(requestDto.Regions);
        request.OrderState = requestDto.OrderState;
        request.FieldSorted = (GetOrdersRequest.Types.FieldSorted)requestDto.FieldSorted;
        request.Pagination = requestDto.Pagination;
        request.TypeSorted = (GetOrdersRequest.Types.TypeSorted)requestDto.TypeSorted;

        var result = await _ordersClient.GetOrdersAsync(request); // TODO: проверить

        return Ok(result);
    }

    [HttpGet("GetOrdersAgrigater")]
    public async Task<IActionResult> GetOrdersAgrigaterAsync(DateTime startDate, IEnumerable<string> region)
    {
        var request = new GetOrdersAgrigaterRequest();
        request.Regions.AddRange(region);
        request.StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(startDate, DateTimeKind.Utc));

        var result = await _ordersClient.GetOrdersAgrigaterAsync(request);

        return Ok(result);
    }

    [HttpPost("GetOrdersByClient")]
    public async Task<IActionResult> GetOrdersByClientAsync(int clientId, int pagination, DateTime startDate)
    {
        var request = new GetOrdersByClientRequest
        {
            ClientId = clientId,
            Pagination = pagination,
            StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(startDate, DateTimeKind.Utc))
        };

        var result = await _ordersClient.GetOrdersByClientAsync(request);

        return Ok(result);
    }
}
