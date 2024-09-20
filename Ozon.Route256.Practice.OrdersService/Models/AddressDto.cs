namespace Ozon.Route256.Practice.GatewayService.Models;

public record AddressDto(
    string region,
    string city,
    string street,
    string building,
    string apartment,
    double latitude,
    double longitude);