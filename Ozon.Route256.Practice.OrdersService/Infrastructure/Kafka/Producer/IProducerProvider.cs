using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;

public interface IProducerProvider
{
    IProducer<string, string> Get();
}