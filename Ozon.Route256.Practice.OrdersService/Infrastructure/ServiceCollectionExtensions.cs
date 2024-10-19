using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.Configuration;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.ClientBalancing.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Repository;
using Ozon.Route256.Practice.OrdersService.Dal.Repository.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrder;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Redis;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(configuration);
        services.AddRepositories(configuration);
        services.AddRedis(configuration);
        services.AddKafka(configuration);

        services.AddScoped<IRedisDatabaseFactory>(provider => new RedisDatabaseFactory(configuration.GetValue<string>("ROUTE256_REDIS")));

        services.AddSingleton<IDbStore, DbStore>();
        services.AddHostedService<SdConsumerHostedService>();

        var connectionString = configuration.GetValue<string>("ORDERS-DB");

        services.AddSingleton<IPostgresConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));

        //services.AddFluentMigratorCore()
        //    .ConfigureRunner(builder => builder
        //        .AddPostgres()
        //        .ScanIn(typeof(SqlMigration).Assembly)
        //        .For.Migrations()) // TODO: не понятно со строкой подключения
        //    .AddOptions<ProcessorOptions>()
        //    .Configure(
        //        options =>
        //        {
        //            options.ConnectionString = connectionString;
        //            options.Timeout = TimeSpan.FromSeconds(30);
        //        });

        PostgresMapping.MapCompositeTypes();

        return services;
    }

    private static void AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(x => x.Interceptors.Add<LoggerInterceptor>());
        services.AddGrpcClient<SdService.SdServiceClient>(options =>
        {
            var url = configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

            options.Address = new Uri(url);
            //options.Address = new Uri("static:///sd-service");
        });
        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(options =>
        {
            var url = configuration.GetValue<string>("ROUTE256_LS_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_LS_ADDRESS variable is empty");

            options.Address = new Uri(url);
        });
        services.AddGrpcClient<Customers.CustomersClient>(options =>
        {
            var url = Environment.GetEnvironmentVariable("ROUTE256_CUSTOMER_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_CUSTOMER_ADDRESS variable is empty");

            options.Address = new Uri(url);
        });

        services.AddGrpcReflection();
    }

    private static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IAddressRepository, AddressRepository>();
        services.AddSingleton<IGoodRepository, GoodRepository>();

        services.AddSingleton<InMemoryStorage>(new InMemoryStorage());
    }

    private static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRedisDatabaseFactory>(provider => new RedisDatabaseFactory(configuration.GetValue<string>("ROUTE256_REDIS")));
        services.AddScoped<IRedisCustomerRepository, RedisCustomerRepository>();
    }

    private static void AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PreOrderConsumerConfig>()
            .Configure<IConfiguration>((opt, config) =>
                config
                    .GetSection("Kafka:Consumers:PreOrder")
                    .Bind(opt));

        services.AddOptions<NewOrdersProducerConfig>()
            .Configure<IConfiguration>((opt, config) =>
                config
                    .GetSection("Kafka:Producer:NewOrder")
                    .Bind(opt));

        services.AddSingleton<IConsumerProvider, ConsumerProvider>();
        services.AddSingleton<IProducerProvider, ProducerProvider>();

        services.AddTransient<NewOrderProducer>();
    }
}
