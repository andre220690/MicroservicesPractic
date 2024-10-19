using Npgsql;
using System.Data.Common;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common;

public static class NpsqlExtensions
{
    public static void Add<T>(this DbParameterCollection parameters, string name, T? value) =>
        parameters.Add(
            new NpgsqlParameter<T>
            {
                ParameterName = name,
                TypedValue = value
            });
}
