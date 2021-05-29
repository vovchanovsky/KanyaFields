using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace KafkaInfrastructure.Consumer
{
    public interface IConsumerFactory<TKey, TValue>
    {
        IConsumer<TKey, TValue> GetConsumer(ISchemaRegistryClient schemaRegistry);
    }
}
