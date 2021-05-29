using Confluent.SchemaRegistry;

namespace KafkaInfrastructure.SchemaRegistry
{
    public interface ISchemaRegistryFactory
    {
        ISchemaRegistryClient GetSchemaRegistry();
    }
}