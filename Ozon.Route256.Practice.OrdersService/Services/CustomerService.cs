using Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;
using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Interfaces;

namespace Ozon.Route256.Practice.OrdersService.Services;

public class CustomerService : ICustomerService
{
    private readonly Customers.CustomersClient _customersClient;
    private readonly IRedisCustomerRepository _redisCustomerRepository;

    public CustomerService(Customers.CustomersClient customersClient, IRedisCustomerRepository redisCustomerRepository)
    {
        _customersClient = customersClient;
        _redisCustomerRepository = redisCustomerRepository;
    }

    public async Task<CustomerDto?> GetCustomerById(int id, CancellationToken token)
    {
        var redisCustomer = await _redisCustomerRepository.Find(id, token);
        if (redisCustomer is not null)
        {
            return redisCustomer;
        }

        var request = new GetCustomerByIdRequest { Id = id };
        var customer = await _customersClient.GetCustomerAsync(request);

        if (customer is null)
        {
            return null;
        }

        var customerDto = ToDomain(customer);

        _ = _redisCustomerRepository.Insert(customerDto, token);

        return customerDto;
    }

    private CustomerDto ToDomain(Customer customer)
    {
        var address = customer.Addresses
            .Select(r => new AddressDto(r.Region, r.City, r.Street, r.Building, r.Apartment, r.Latitude, r.Longitude))
            .ToArray();

        var result = new CustomerDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email,
            customer.DefaultAddress,
            address);

        return result;
    }
}
