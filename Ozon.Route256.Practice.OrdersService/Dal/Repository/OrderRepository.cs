using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Models.Filter;
using System.Data;

namespace Ozon.Route256.Practice.OrdersService.Dal.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly IPostgresConnectionFactory _connectionFactory;

    private const string Fields = "id, order_type, customer_id, address_id, date, order_state, goods";
    private const string Table = "orders";

    private const int COUNT_LINE_ON_PAGE = 20;

    public OrderRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<OrderDb[]> GetOrdersByFilter(OrderFilter filter, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderDb?> Find(long id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};
            where id = :id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("id", id);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var result = await ReadOrderDb(reader, token);
        return result.FirstOrDefault();
    }

    public async Task<int> Update(OrderDb order, CancellationToken token)
    {
        // TODO: не понял как обновить модель
        // TODO: есть сомнения что это работает
        const string sql = @$"
            update {Table}
            set 
                order_type = :order_type,
                customer_id = :customer_id,
                address_id = :address_id,
                date = :date,
                order_state = :order_state,
                goods = :goods
            where id = :id
            returning id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("id", order.id);
        command.Parameters.Add("order_type", order.orderType);
        command.Parameters.Add("customer_id", order.customerId);
        command.Parameters.Add("address_id", order.addressId);
        command.Parameters.Add("date", order.date);
        command.Parameters.Add("order_state", order.orderState);
        command.Parameters.Add("goods", order.goods);

        await connection.OpenAsync(token);
        var id = await command.ExecuteScalarAsync(token);
        return (int)id!;
    }

    public async Task<OrderDb[]> GetOrdersByRegionsAndState(OrderFilter filter, CancellationToken token)
    {
        var filledStr = filter.fieldSorted switch
        {
            FieldSorted.Region => "a.region",
            _ => "o.id"
        };

        var orderStr = filter.typeSorted switch
        {
            TypeSorted.NoneState => " ",
            TypeSorted.ASC => $" order by {filledStr} ",
            TypeSorted.DESC => $" order by {filledStr} desc "
        };

        string sql = @$"
            select {Fields}
            from {Table} as o
            join addresses as a
            on o.address_id = a.id
            where o.order_state = :oreder_state and a.region = any(:regions::text[])"
            + orderStr
            + $"limit {COUNT_LINE_ON_PAGE} offset {COUNT_LINE_ON_PAGE * filter.pagination};";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("order_state", filter.orderState);
        command.Parameters.Add("regions", filter.regions);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await ReadOrderDb(reader, token);

        return result;
    }

    public async Task<Tuple<string, int>[]> OrderAggrigateByRegion(IEnumerable<string> regions, DateTime dateTime, CancellationToken token)
    {
        string sql = @$"
            select a.region, o.id
            from {Table} as o
            join addresses as a
            on o.address_id = a.id
            where o.date <= :date and a.region = any(:regions::text[]);
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("date", dateTime);
        command.Parameters.Add("regions", regions);

        throw new NotImplementedException();
    }

    public async Task<OrderDb[]> GetOrdersByClient(int clientId, DateTime startDate, int pagination, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where customer_id = :customer_id and date >= :date
            limit :take offset :skip;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("customer_id", clientId);
        command.Parameters.Add("date", startDate);
        command.Parameters.Add("take", COUNT_LINE_ON_PAGE);
        command.Parameters.Add("skip", COUNT_LINE_ON_PAGE * pagination);

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var result = await ReadOrderDb(reader, token);

        return result;
    }

    private static async Task<OrderDb[]> ReadOrderDb(
        NpgsqlDataReader reader,
        CancellationToken token)
    {
        var result = new List<OrderDb>();
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new OrderDb(
                    id: reader.GetFieldValue<int>(0),
                    orderType: reader.GetFieldValue<int>(1),
                    customerId: reader.GetFieldValue<int>(2),
                    addressId: reader.GetFieldValue<int>(3),
                    date: reader.GetFieldValue<DateTime>(4),
                    orderState: reader.GetFieldValue<int>(5),
                    goods: reader.GetValue(6) as int[] ?? Array.Empty<int>()
                ));
        }
        return result.ToArray();
    }
}
