using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Models;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common;

public class PostgresMapping
{
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        mapper.MapComposite<OrderDb>("order_type");
        mapper.MapComposite<GoodDb>("good");
        mapper.MapComposite<AddressDb>("address");
    }
}
