using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Models;
using Ozon.Route256.Practice.OrdersService.Models.PreOrders;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;

public class NewOrderProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<NewOrdersProducerConfig> _config;

    public NewOrderProducer(IProducerProvider producer, IOptions<NewOrdersProducerConfig> config)
    {
        _producerProvider = producer;
        _config = config;
    }

    public async Task Produce(OrderModel order, CancellationToken token)
    {
        var producer = _producerProvider.Get();

        var message = ToKafka(order);

        var result = await producer.ProduceAsync(_config.Value.Topic, message, token);
        Console.WriteLine(result.Message);
    }

    private static Message<string, string> ToKafka(OrderModel order)
    {
        var kafkaOrder = new KafkaNewOrder(order.Id);

        var value = JsonSerializer.Serialize(kafkaOrder);

        return new Message<string, string>
        {
            Key = order.Id.ToString(),
            Value = value
        };
    }

    private record KafkaNewOrder(long OrderId);
}
