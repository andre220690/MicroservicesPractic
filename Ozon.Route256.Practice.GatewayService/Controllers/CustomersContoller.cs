using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers;

public class CustomersContoller : ControllerBase
{
    private readonly Customers.CustomersClient _customersClient;

    public CustomersContoller(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    [HttpGet("GetCustomers")]
    public async Task<IActionResult> GetCustomers()
    {
        var response = await _customersClient.GetCustomersAsync(new Google.Protobuf.WellKnownTypes.Empty());

        return Ok(response);
    }
}
