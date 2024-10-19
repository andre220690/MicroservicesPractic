using Npgsql;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}