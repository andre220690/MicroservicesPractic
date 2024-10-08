using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

public interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}