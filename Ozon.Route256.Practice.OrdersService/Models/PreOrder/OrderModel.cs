namespace Ozon.Route256.Practice.OrdersService.Models.PreOrders;

public record OrderModel(
    long Id,
    OrderSource Source,
    CustomerModel Customer,
    IEnumerable<GoodModel> Goods);