using Confluent.SchemaRegistry;
using KafkaInfrastructure.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace KafkaInfrastructure.SchemaRegistry
{
    public class SchemaRegistryFactory : ISchemaRegistryFactory
    {
        private readonly IConfiguration _configuration;

        public SchemaRegistryFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISchemaRegistryClient GetSchemaRegistry()
        {
            return new CachedSchemaRegistryClient(GetSchemaRegistryConfig());
        }

        private SchemaRegistryConfig GetSchemaRegistryConfig()
        {
            return new SchemaRegistryConfig()
            {
                Url = $"{_configuration[EnvironmentVariables.SchemaRegistryHost]}:" +
                      $"{_configuration[EnvironmentVariables.SchemaRegistryPort]}"
            };
        }
    }
}