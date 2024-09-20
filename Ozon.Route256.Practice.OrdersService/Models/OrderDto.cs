namespace Ozon.Route256.Practice.OrdersService.Models;

public record OrderDto
(
    long id,
    int countProducts,
    double totalAmount,
    double totalWeight,
    int orderType,
    DateTime orderDate,
    string fromRegion,
    OrderStateEnum orderState,
    int clientId,
    string customerName,
    string adress,
    string phoneNumber
);
