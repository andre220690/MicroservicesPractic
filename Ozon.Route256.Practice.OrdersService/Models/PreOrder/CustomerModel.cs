namespace Ozon.Route256.Practice.OrdersService.Models.PreOrders;

public record CustomerModel(
    long Id,
    AddressModel Address);

public record AddressModel(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);