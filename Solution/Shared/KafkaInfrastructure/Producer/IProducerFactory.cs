using Confluent.Kafka;
using Confluent.SchemaRegistry;

namespace KafkaInfrastructure.Producer
{
    public interface IProducerFactory<TKey, TValue>
    {
        IProducer<TKey, TValue> GetProducer(ISchemaRegistryClient schemaRegistry);
    }
}