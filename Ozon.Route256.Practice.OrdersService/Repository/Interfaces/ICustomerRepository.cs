using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.OrdersService.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerDto?> Find(int customerId, CancellationToken token);
    Task Insert(CustomerDto customer, CancellationToken token);
    Task<bool> IsExists(int customerId, CancellationToken token);
}
