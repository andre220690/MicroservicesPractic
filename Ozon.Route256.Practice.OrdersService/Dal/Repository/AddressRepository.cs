using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;
using System.Data;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository;

public class AddressRepository : IAddressRepository
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    private const string Table = "addresses";

    public AddressRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<string[]> GetRegions(CancellationToken token)
    {
        const string sql = @$"
            select region
            from {Table}
            group by region;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = new List<string>();
        while (await reader.ReadAsync(token))
        {
            result.Add(reader.GetFieldValue<string>(0));
        }

        return result.ToArray();
    }
}
