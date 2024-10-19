using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Ozon.Route256.Practice;

var builder = WebApplication.CreateBuilder(args);

//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
//{
//    builder.WebHost.ConfigureKestrel(options =>
//    {
//        var httpPort = int.Parse(Environment.GetEnvironmentVariable("ROUTE256_HTTP_PORT")!);

//        options.Listen(
//            IPAddress.Any,
//            httpPort,
//            listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
//    });
//}

var ordersPorts = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? new[] { new BalancerAddress("localhost", 9081), new BalancerAddress("localhost", 9082) }
    : new[] { new BalancerAddress("orders-service-1", 5001), new BalancerAddress("orders-service-2", 5001) };

var factory = new StaticResolverFactory(address => ordersPorts);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ResolverFactory>(factory);

builder.Services.AddGrpcClient<Orders.OrdersClient>(options =>
{
    options.Address = new Uri("static:///orders-service");
}).ConfigureChannel(x =>
{
    x.Credentials = ChannelCredentials.Insecure;
    x.ServiceConfig = new ServiceConfig()
    {
        LoadBalancingConfigs = { new LoadBalancingConfig("round_robin") }
    };
});
builder.Services.AddGrpcClient<Customers.CustomersClient>(options =>
{
    var url = Environment.GetEnvironmentVariable("ROUTE256_CUSTOMER_ADDRESS");
    if (string.IsNullOrEmpty(url))
        throw new Exception("ROUTE256_CUSTOMER_ADDRESS variable is empty");

    options.Address = new Uri(url);
}).ConfigureChannel(x =>
{
    x.Credentials = ChannelCredentials.Insecure;
}); // TODO: почему то не получается обращаться к этому сервису после переноса в докер

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
