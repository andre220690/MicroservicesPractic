using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.OrdersService.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerById(int id);
}
