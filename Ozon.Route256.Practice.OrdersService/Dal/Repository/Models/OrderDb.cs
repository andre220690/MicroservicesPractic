namespace Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;

public record OrderDb(
    int id,
    int orderType,
    int customerId,
    int addressId,
    DateTime date,
    int orderState,
    int[] goods
    );