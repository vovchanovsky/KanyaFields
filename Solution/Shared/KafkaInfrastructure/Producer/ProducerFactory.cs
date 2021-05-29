using System.Globalization;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaInfrastructure.Infrastructure;
using KafkaInfrastructure.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KafkaInfrastructure.Producer
{
    public class ProducerFactory<TKey, TValue> : IProducerFactory<TKey, TValue> where TValue : class
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProducerFactory<TKey, TValue>> _logger;

        public ProducerFactory(ILogger<ProducerFactory<TKey, TValue>> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IProducer<TKey, TValue> GetProducer(ISchemaRegistryClient schemaRegistry)
        {
            return new ProducerBuilder<TKey, TValue>(GetProducerConfig())
                //.SetKeySerializer(new AvroSerializer<TKey>(schemaRegistry))
                //.SetValueSerializer(new AvroSerializer<TValue>(schemaRegistry))
                //.SetKeySerializer(new JsonSerializer<TKey>(schemaRegistry))
                .SetValueSerializer(new JsonSerializer<TValue>(schemaRegistry))
                .SetErrorHandler(ErrorHandler)
                .Build();
        }

        private ProducerConfig GetProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = $"{_configuration[EnvironmentVariables.KafkaServiceHost]}:" +
                                   $"{_configuration[EnvironmentVariables.KafkaServicePort]}",
                Partitioner = Partitioner.ConsistentRandom
            };
        }

        private void ErrorHandler(IProducer<TKey, TValue> _, Error error) =>
            _logger.LogError(new KafkaException(error),
                string.Format(CultureInfo.InvariantCulture, ErrorMessages.GeneralError, error.Reason));
    }
}