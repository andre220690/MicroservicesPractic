using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;

public class NewOrdersProducerConfig
{
    public string Topic { get; init; } = null!;
    public ProducerConfig Config { get; init; } = new();
}
