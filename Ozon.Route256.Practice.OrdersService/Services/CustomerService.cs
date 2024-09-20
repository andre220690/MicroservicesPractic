using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.OrdersService.Services;

public class CustomerService : ICustomerService
{
    private readonly Customers.CustomersClient _customersClient;

    public CustomerService(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    public Task<CustomerDto> GetCustomerById(int id)
    {

        return null;
    }
}
