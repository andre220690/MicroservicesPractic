using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;

public class ProducerProvider : IDisposable, IProducerProvider
{
    private readonly IOptions<NewOrdersProducerConfig> _config;

    private readonly IProducer<string, string> _producer;

    public ProducerProvider(
        IOptions<NewOrdersProducerConfig> config)
    {
        _config = config;
        _producer = new ProducerBuilder<string, string>(
                config.Value.Config)
            .Build();
    }

    public IProducer<string, string> Get() =>
        _producer;

    public void Dispose()
    {
        _producer.Dispose();
    }
}