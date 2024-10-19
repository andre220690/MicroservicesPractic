using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Dal.Common;

namespace Ozon.Route256.Practice.OrdersService.Dal.Migrations;

[Migration(1, "Initial migration")]
public class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services)
    {
        return @"
        create table orders(
            id bigint generated always as identity primary key,
            order_type int,
            customer_id int,
            address_id int,
            date timestamp,
            order_state int,
            goods int[]
        );

        create table goods(
            id int generated always as identity primary key,
            name text,
            price decimal,
            weight decimal
        );

        create table addresses(
            id int generated always as identity primary key,
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
        drop table orders;
        drop table goods;
        drop table adresses;
        ";
    }
}
