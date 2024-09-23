using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrder;

public class PreOrderConsumerConfig
{
    public string Topic { get; init; } = null!;
    public ConsumerConfig Config { get; init; } = new();
}
