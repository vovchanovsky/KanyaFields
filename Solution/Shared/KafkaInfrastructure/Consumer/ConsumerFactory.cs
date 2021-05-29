using System.Collections.Generic;
using System.Globalization;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaInfrastructure.Infrastructure;
using KafkaInfrastructure.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KafkaInfrastructure.Consumer
{
    public class ConsumerFactory<TKey, TValue> : IConsumerFactory<TKey, TValue> where TValue : class
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsumerFactory<TKey, TValue>> _logger;

        public ConsumerFactory(ILogger<ConsumerFactory<TKey, TValue>> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IConsumer<TKey, TValue> GetConsumer(ISchemaRegistryClient schemaRegistry)
        {
            var consumer = new ConsumerBuilder<TKey, TValue>(GetConsumerConfig())
                //.SetKeyDeserializer(new AvroDeserializer<TKey>(schemaRegistry).AsSyncOverAsync())
                //.SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
                //.SetKeyDeserializer(new JsonDeserializer<TKey>().AsSyncOverAsync())
                .SetValueDeserializer(new JsonDeserializer<TValue>().AsSyncOverAsync())
                .SetErrorHandler(ErrorHandler)
                .SetPartitionsAssignedHandler(PartitionsAssignedHandler)
                .SetPartitionsRevokedHandler(PartitionsRevokedHandler)
                .Build();
            return consumer;
        }

        private ConsumerConfig GetConsumerConfig()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = $"{_configuration[EnvironmentVariables.KafkaServiceHost]}:" +
                    $"{_configuration[EnvironmentVariables.KafkaServicePort]}",
                GroupId = _configuration[EnvironmentVariables.ConsumerGroupId],
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 300000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin,
                MaxPollIntervalMs = 1000000,
                AllowAutoCreateTopics = true
            };
            return config;
        }

        private void ErrorHandler(IConsumer<TKey, TValue> _, Error error) =>
            _logger.LogError(new KafkaException(error),
                string.Format(CultureInfo.InvariantCulture, ErrorMessages.GeneralError, error.Reason));


        private void PartitionsAssignedHandler(IConsumer<TKey, TValue> consumer,
            List<TopicPartition> partitions) =>
            _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, InfoMessages.AssignedPartitions,
                string.Join(", ", partitions)));

        private void PartitionsRevokedHandler(IConsumer<TKey, TValue> consumer,
            List<TopicPartitionOffset> partitionOffsets) =>
            _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, InfoMessages.RevokingAssignment,
                string.Join(", ", partitionOffsets)));
    }
}