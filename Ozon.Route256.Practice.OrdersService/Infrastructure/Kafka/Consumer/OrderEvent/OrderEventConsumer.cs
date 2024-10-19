using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrderEvent;

public class OrderEventConsumer : BackgroundService // TODO: объеденить консьюмеров
{
    private readonly ILogger<OrderEventConsumer> _logger;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<OrderEventConsumerConfig> _config;

    public OrderEventConsumer(ILogger<OrderEventConsumer> logger,
        IConsumerProvider consumerProvider,
        IOptions<OrderEventConsumerConfig> config)
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
        var preOrder = JsonSerializer.Deserialize<object>(consumeResult.Message.Value, new JsonSerializerOptions()); //TODO: привести к объекту

        if (preOrder is null)
        {
            return;
        }

        // TODO: обновить заказ
    }
}
