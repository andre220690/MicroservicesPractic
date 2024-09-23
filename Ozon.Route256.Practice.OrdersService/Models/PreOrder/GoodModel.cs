namespace Ozon.Route256.Practice.OrdersService.Models.PreOrders;

public record GoodModel(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    uint Weight);