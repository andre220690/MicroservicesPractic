using Ozon.Route256.Practice.OrdersService.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;

public class RedisCustomerRepository : IRedisCustomerRepository
{
    private readonly IDatabase _redisDatabase;
    private readonly IServer _redisServer;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();

    public RedisCustomerRepository(IRedisDatabaseFactory redisDatabaseFactory)
    {
        _redisDatabase = redisDatabaseFactory.GetDatabase();
        _redisServer = redisDatabaseFactory.GetServer();
    }

    private static string GetKey(int customerId) =>
        $"customer:{customerId}";

    public async Task<CustomerDto?> Find(int customerId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var value = await _redisDatabase.StringGetAsync(GetKey(customerId)).WaitAsync(token);

        return ToDomain(value);
    }

    public async Task Insert(CustomerDto customer, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var redisValue = ToRedisValue(customer);
        await _redisDatabase
            .StringSetAsync(
                GetKey(customer.id),
                redisValue)
            .WaitAsync(token);
    }

    public async Task<bool> IsExists(int customerId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = await _redisDatabase
            .KeyExistsAsync(
                GetKey(customerId))
            .WaitAsync(token);

        return result;
    }

    private RedisValue ToRedisValue(CustomerDto customer)
    {
        return JsonSerializer.Serialize(
            customer,
            _jsonSerializerOptions);
    }

    private CustomerDto ToDomain(RedisValue redisValue)
    {
        if (string.IsNullOrWhiteSpace(redisValue))
        {
            return null;
        }

        var dto = JsonSerializer.Deserialize<CustomerDto>(redisValue, _jsonSerializerOptions);

        return dto;
    }
}
