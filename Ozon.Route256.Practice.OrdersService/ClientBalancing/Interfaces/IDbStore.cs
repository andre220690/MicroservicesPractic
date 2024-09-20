namespace Ozon.Route256.Practice.OrdersService.ClientBalancing.Interfaces;

public interface IDbStore
{
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);
}
