using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Models.PreOrders;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;

public interface IGoodRepository
{
    Task<GoodDb> Find(int id, CancellationToken token);

    Task<int> Create(GoodModel good, CancellationToken token);
}
