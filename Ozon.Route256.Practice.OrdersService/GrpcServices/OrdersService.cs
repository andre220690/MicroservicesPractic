using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository.Models.Filter;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;

public sealed class OrdersService : Orders.OrdersBase
{
    private readonly ILogger<OrdersService> _logger;
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
    private readonly IOrderRepository _orderRepositiry;

    public OrdersService(ILogger<OrdersService> logger,
        IOrderRepository orderRepositiry,
        LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _orderRepositiry = orderRepositiry;
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
        _logger = logger;
    }

    public override async Task<SetOrderCancellResponse> SetOrderCancell(SetOrderCancellRequest request, ServerCallContext context)
    {
        var order = await _orderRepositiry.Find(request.Number, new CancellationToken());

        if (order == null)
        {
            throw new NotFoundException($"Order {request.Number} not found");
        }

        if (order.orderState == OrderStateEnum.SentToCustomer)
        {
            return new SetOrderCancellResponse { Information = $"Order {request.Number} has final status", IsSuccessful = false };
        }

        var lsResulr = _logisticsSimulatorServiceClient.OrderCancel(new LogisticsSimulator.Grpc.Order() { Id = request.Number });

        if (!lsResulr.Success)
        {
            return new SetOrderCancellResponse { Information = $" Error to LogisticSimulator: {lsResulr.Error}", IsSuccessful = false };
        }

        return new SetOrderCancellResponse { Information = $"Successful", IsSuccessful = true };
    }

    public override async Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, ServerCallContext context)
    {
        var order = await _orderRepositiry.Find(request.Number, new CancellationToken());

        if (order == null)
        {
            throw new NotFoundException($"Order {request.Number} not found");
        }

        return new GetOrderStatusResponse { OrderState = (OrderState) order.orderState };

    }

    public override async Task<GetRegionsResponse> GetRegions(Empty empty, ServerCallContext context)
    {
        var regions = await _orderRepositiry.GetRegions(new CancellationToken());

        return new GetRegionsResponse { Regions = { regions } };
    }

    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        var filter = new OrderFilter
        (
            request.Regions.ToArray(),
            (OrderStateEnum)request.OrderState,
            request.Pagination,
            (TypeSorted)request.TypeSorted,
            (FieldSorted)request.FieldSorted
        );

        var orders = await _orderRepositiry.GetOrdersByFilter(filter, new CancellationToken());        

        var result = OrderDtoConverToOrder(orders);

        return new GetOrdersResponse
        {
            Order = { result }
        };
    }

    public override async Task<GetOrdersAgrigaterResponse> GetOrdersAgrigater(GetOrdersAgrigaterRequest request, ServerCallContext context)
    {
        var aggrigate = await _orderRepositiry.OrderAggrigateByRegion(request.Regions, request.StartDate.ToDateTime(), new CancellationToken());

        return new GetOrdersAgrigaterResponse
        {
            OrderAgrigater = { aggrigate.Select(r => new OrderAgrigater { Region = r.Item1, OrederAmount = r.Item2 }) }
        };
    }

    public override async Task<GetOrdersResponse> GetOrdersByClient(GetOrdersByClientRequest request, ServerCallContext context)
    {
        var orders = await _orderRepositiry.GetOrdersByClient(request.ClientId, request.StartDate.ToDateTime(), request.Pagination, new CancellationToken());

        return new GetOrdersResponse
        {
            Order = { OrderDtoConverToOrder(orders) }
        };
    }

    private IEnumerable<Order> OrderDtoConverToOrder(IEnumerable<OrderDto> orders) => orders.Select(r => new Order
    {
        Id = r.id,
        CountProducts = r.countProducts,
        TotalAmount = r.totalAmount,
        TotalWeight = r.totalWeight,
        OrderType = r.orderType,
        OrderDate = Timestamp.FromDateTime(DateTime.SpecifyKind(r.orderDate, DateTimeKind.Utc)),
        FromRegion = r.fromRegion,
        OrderState = (OrderState)((int)r.orderState),
        CustomerName = r.customerName,
        Adress = r.adress,
        PhoneNumber = r.phoneNumber
    }).ToArray();
}
