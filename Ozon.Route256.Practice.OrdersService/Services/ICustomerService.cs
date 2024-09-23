using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerById(int id, CancellationToken token);
}