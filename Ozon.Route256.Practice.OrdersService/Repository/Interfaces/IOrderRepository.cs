using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Models.Filter;

namespace Ozon.Route256.Practice.OrdersService.Repository.Interfaces;

public interface IOrderRepository
{
    Task<OrderDto> Find(long id, CancellationToken token);

    Task<bool> Update(OrderDto order, CancellationToken token);

    Task<string[]> GetRegions(CancellationToken token);

    Task<OrderDto[]> GetOrdersByFilter(OrderFilter filter, CancellationToken token);

    Task<Tuple<string, int>[]> OrderAggrigateByRegion(IEnumerable<string> regions, DateTime dateTime, CancellationToken token);

    Task<OrderDto[]> GetOrdersByClient(int clientId, DateTime startDate, int pagination, CancellationToken token);
}
