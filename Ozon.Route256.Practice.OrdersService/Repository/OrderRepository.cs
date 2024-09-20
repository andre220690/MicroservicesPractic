using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Models.Filter;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly InMemoryStorage _storage;

    private const int COUNT_LINE_ON_PAGE = 20;

    public OrderRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public Task<OrderDto?> Find(long id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<OrderDto?>(token);

        return _storage.Orders.TryGetValue(id, out var orderDto)
            ? Task.FromResult<OrderDto?>(orderDto)
            : Task.FromResult<OrderDto?>(null);
        
    }

    public Task<bool> Update(OrderDto order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        if (_storage.Orders.TryGetValue(order.id, out var orderDb))
        {
            _storage.Orders[order.id] = order;
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }

    public Task<string[]> GetRegions(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<string[]>(token);

        return Task.FromResult(_storage.Orders.Select(r => r.Value.fromRegion).Distinct().ToArray());
    }

    public Task<OrderDto[]> GetOrdersByFilter(OrderFilter filter, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<OrderDto[]>(token);

        var query = _storage
            .Orders
            .Values
            .Where(r => filter.regions.Contains(r.fromRegion) && filter.orderState == r.orderState);

        if (filter.fieldSorted != 0)
        {
            query = filter.typeSorted == TypeSorted.DESC
                ? query.OrderByDescending(r => r.fromRegion)
                : query.OrderBy(r => r.fromRegion);
        }

        return Task.FromResult(query.Skip(COUNT_LINE_ON_PAGE * filter.pagination).Take(COUNT_LINE_ON_PAGE).ToArray());
    }

    public Task<Tuple<string, int>[]> OrderAggrigateByRegion(IEnumerable<string> regions, DateTime dateTime, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Tuple<string, int>[]>(token);

        return Task.FromResult(_storage
            .Orders
            .Values
            .Where(r => regions.Contains(r.fromRegion) && dateTime > r.orderDate)
            .GroupBy(r => r.fromRegion)
            .Select(r => new Tuple<string, int>(r.Key, r.Count()))
            .ToArray());
    }

    public Task<OrderDto[]> GetOrdersByClient(int clientId, DateTime startDate, int pagination, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<OrderDto[]>(token);

        return Task.FromResult(_storage
            .Orders
            .Values
            .Where(r => r.clientId == clientId && r.orderDate > startDate)
            .Skip(COUNT_LINE_ON_PAGE * pagination)
            .Take(COUNT_LINE_ON_PAGE)
            .ToArray());
    }

}
