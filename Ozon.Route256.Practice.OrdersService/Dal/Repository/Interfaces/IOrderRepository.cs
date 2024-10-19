using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Models.Filter;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;

public interface IOrderRepository
{
    Task<OrderDb> Find(long id, CancellationToken token);

    Task<int> Update(OrderDb order, CancellationToken token);

    Task<OrderDb[]> GetOrdersByFilter(OrderFilter filter, CancellationToken token);

    Task<Tuple<string, int>[]> OrderAggrigateByRegion(IEnumerable<string> regions, DateTime dateTime, CancellationToken token);

    Task<OrderDb[]> GetOrdersByClient(int clientId, DateTime startDate, int pagination, CancellationToken token);
}
