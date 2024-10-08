﻿using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;

public class RedisDatabaseFactory : IRedisDatabaseFactory, IDisposable
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseFactory(string connectionString)
    {
        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
    }

    public IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    public IServer GetServer()
    {
        var endpoints = _connectionMultiplexer.GetEndPoints();
        return _connectionMultiplexer.GetServer(endpoints.First());
    }

    public void Dispose() => _connectionMultiplexer.Dispose();
}
