using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Ozon.Route256.Practice;
using Ozon.Route256.Practice.CustomerService.ClientBalancing;
using Ozon.Route256.Practice.CustomerService.Infrastructure;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.ClientBalancing.Interfaces;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository.Models;
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

    options.Listen(
        IPAddress.Any,
        httpPort,
        listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<InMemoryStorage>(new InMemoryStorage());


builder.Services.AddSingleton<IDbStore, DbStore>();
builder.Services.AddGrpc(x => x.Interceptors.Add<LoggerInterceptor>());
builder.Services.AddGrpcClient<SdService.SdServiceClient>(options =>
{
    var url = builder.Configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
    if (string.IsNullOrEmpty(url))
        throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

    options.Address = new Uri(url);
    //options.Address = new Uri("static:///sd-service");
});
builder.Services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(options =>
{
    var url = builder.Configuration.GetValue<string>("ROUTE256_LS_ADDRESS");
    if (string.IsNullOrEmpty(url))
        throw new Exception("ROUTE256_LS_ADDRESS variable is empty");

    options.Address = new Uri(url);
});
builder.Services.AddGrpcClient<Customers.CustomersClient>(options =>
{
    var url = Environment.GetEnvironmentVariable("ROUTE256_CUSTOMER_ADDRESS");
    if (string.IsNullOrEmpty(url))
        throw new Exception("ROUTE256_CUSTOMER_ADDRESS variable is empty");

    options.Address = new Uri(url);
});

builder.Services.AddGrpcReflection();

builder.Services.AddScoped<IRedisDatabaseFactory>(provider => new RedisDatabaseFactory(builder.Configuration.GetValue<string>("ROUTE256_REDIS")));
builder.Services.AddHostedService<SdConsumerHostedService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(x =>
{
    x.MapGrpcService<OrdersService>();
    x.MapGrpcReflectionService();
});

app.Run();