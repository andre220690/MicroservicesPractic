using Ozon.Route256.Practice.OrdersService.ClientBalancing.Interfaces;

namespace Ozon.Route256.Practice.OrdersService.ClientBalancing;

public sealed class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    public Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        var endpoints = new DbEndpoint[dbEndpoints.Count];

        var i = 0;

        foreach (var endpoint in dbEndpoints)
        {
            endpoints[i++] = endpoint;
        }

        _endpoints = endpoints;

        return Task.CompletedTask;
    }
}