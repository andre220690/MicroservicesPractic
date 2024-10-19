using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Google.Api;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrder;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// TODO: работает ил нет протестировать нужно
builder.WebHost.ConfigureKestrel(options =>
{
    var grpcPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_GRPC_PORT")!);
    var httpPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_HTTP_PORT")!);

    options.Listen(
        IPAddress.Any,
        grpcPort,
        listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<PreOrderConsumer>();


var needMigration = args.Length > 0 && args[0].Equals("migrate");
if (needMigration)
{
    var connectionString = builder.Configuration.GetValue<string>("ORDERS-DB");

    builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(builder => builder
        .AddPostgres()
        .ScanIn(typeof(SqlMigration).Assembly)
        .For.Migrations()) // TODO: не понятно со строкой подключения
    .AddOptions<ProcessorOptions>()
    .Configure(
        options =>
        {
            options.ConnectionString = connectionString;
            options.Timeout = TimeSpan.FromSeconds(30);
        });

    using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    runner.MigrateUp();

    return;
}

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(x =>
{
    x.MapGrpcService<OrdersService>();
    x.MapGrpcReflectionService();
});

app.Run();