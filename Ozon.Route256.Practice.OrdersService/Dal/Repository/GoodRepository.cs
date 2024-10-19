using Faker;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Models.PreOrders;
using System.Data;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository;

public class GoodRepository : IGoodRepository
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    private const string Table = "goods";
    private const string Fields = "id, name, price, weight";
    private const string FieldsForInsert = "name, price, weight";

    public GoodRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GoodDb> Find(int id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("id", id);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var goods = await ReadGoodsDb(reader, token);

        return goods.FirstOrDefault();
    }

    public async Task<int> Create(GoodModel good, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:name, :price, :weight)
            returning id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("name", good.Name);
        command.Parameters.Add("price", good.Price);
        command.Parameters.Add("weight", good.Weight);

        await connection.OpenAsync(token);
        var id = await command.ExecuteScalarAsync(token);

        return (int)id!;
    }

    private static async Task<GoodDb[]> ReadGoodsDb(
        NpgsqlDataReader reader,
        CancellationToken token)
    {
        var result = new List<GoodDb>();
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new GoodDb(
                    id: reader.GetFieldValue<int>(0),
                    name: reader.GetFieldValue<string>(1),
                    price: reader.GetFieldValue<decimal>(2),
                    weight: reader.GetFieldValue<decimal>(3)
                ));
        }
        return result.ToArray();
    }
}
