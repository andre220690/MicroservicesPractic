using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerDto?> Find(int customerId, CancellationToken token);
    Task Insert(CustomerDto customer, CancellationToken token);
    Task<bool> IsExists(int customerId, CancellationToken token);
}
