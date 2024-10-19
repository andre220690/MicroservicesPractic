using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

[Migration(2, "Adding types")]
public class AddConstants : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services)
    {
        return @"
        create type order_type as (
        id int,
        order_type int,
        customer_id int,
        address_id int,
        date timestamp,
        order_state int,
        goods int[]
        );

        create type good as (
        id int,
        name text,
        price decimal,
        weight decimal
        );

        create type address as (
        id int,
        region text,
        city text,
        street text,
        building int,
        apartment int,
        latitude decimal,
        longitude decimal
        );        
        ";
    }

    protected override string GetDownSql(IServiceProvider services)
    {
        return @"
        drop type order_type;
        drop type good;
        drop type address;
        ";
    }
}
