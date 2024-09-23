using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Models.PreOrders;
using System.Collections;
using System.Text;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrder;

public class PreOrderConsumer : BackgroundService
{
    private readonly ILogger<PreOrderConsumer> _logger;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<PreOrderConsumerConfig> _config;

    public PreOrderConsumer(
        ILogger<PreOrderConsumer> logger,
        IConsumerProvider consumerProvider,
        IOptions<PreOrderConsumerConfig> config)
    {
        _logger = logger;
        _consumerProvider = consumerProvider;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            using var consumer = _consumerProvider.Create(_config.Value.Config);
            Console.WriteLine($"create consumer {_config.Value.Topic}");
            Console.WriteLine(_config.Value.Config.BootstrapServers);

            try
            {
                consumer.Subscribe(_config.Value.Topic);

                await Consume(consumer, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer error");

                try
                {
                    consumer.Unsubscribe();
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    stoppingToken);
            }
        }
    }

    private async Task Consume(IConsumer<string, string> consumer, CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            var result = consumer.Consume(1000);

            await Handle(result, stoppingToken);

            consumer.Commit();
        }
    }

    private async Task Handle(ConsumeResult<string, string> consumeResult, CancellationToken stoppingToken)
    {
        var preOrder = JsonSerializer.Deserialize<OrderModel>(consumeResult.Message.Value, new JsonSerializerOptions());



        Console.WriteLine($"{consumeResult.Topic} get message with KEY {preOrder.Id}");

        if (preOrder is null)
        {
            return;
        }

        // TODO: сохранить в репозиторий
    }
}