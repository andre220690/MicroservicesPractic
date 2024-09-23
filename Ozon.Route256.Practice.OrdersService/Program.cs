using Google.Api;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Ozon.Route256.Practice;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.ClientBalancing.Interfaces;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrder;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository.Models;
using System.Net;
using InMemoryStorage = Ozon.Route256.Practice.OrdersService.Repository.InMemoryStorage;

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


var app = builder.Build();

app.UseRouting();

app.UseEndpoints(x =>
{
    x.MapGrpcService<OrdersService>();
    x.MapGrpcReflectionService();
});

app.Run();