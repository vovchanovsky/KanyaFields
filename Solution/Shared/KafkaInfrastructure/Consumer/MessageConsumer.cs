using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using KafkaInfrastructure.Resources;
using KafkaInfrastructure.SchemaRegistry;
using Microsoft.Extensions.Logging;

namespace KafkaInfrastructure.Consumer
{
    public class MessageConsumer<TKey, TValue> : IMessageConsumer<TKey, TValue>
    {
        public MessageConsumer(
            ILogger<MessageConsumer<TKey, TValue>> logger,
            IConsumerFactory<TKey, TValue> consumerFactory,
            ISchemaRegistryFactory schemaRegistryFactory)
        {
            _logger = logger;
            _consumerFactory = consumerFactory;
            _schemaRegistryFactory = schemaRegistryFactory;
            _consumerInstances = new ConcurrentDictionary<string, IConsumer<TKey, TValue>>();
            _schemaRegistryInstances = new ConcurrentDictionary<string, ISchemaRegistryClient>();
        }

        public ConsumeResult<TKey, TValue> Consume(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                var ex = new ArgumentException(ErrorMessages.InvalidTopicName);
                _logger.LogError(ex, ErrorMessages.InvalidParameter);
                throw ex;
            }

            try
            {
                var consumer = GetConsumer(topicName);
                //var consumeResult = consumer.Consume(500);
                var consumeResult = consumer.Consume();
                if (consumeResult is null)
                {
                    return null;
                }

                if (consumeResult.IsPartitionEOF)
                {
                    _logger.LogInformation(
                        string.Format(CultureInfo.InvariantCulture, InfoMessages.ReachedEndOfTopic,
                            consumeResult.Topic, consumeResult.Partition, consumeResult.Offset));
                    return null;
                }

                _logger.LogInformation(
                    string.Format(CultureInfo.InvariantCulture, InfoMessages.ReceivedMessage,
                        consumeResult.TopicPartitionOffset, consumeResult.Message.Value));

                return consumeResult;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex,
                    string.Format(CultureInfo.InvariantCulture, ErrorMessages.ConsumeError, ex.Error.Reason));
                return null;
            }
        }

        private IConsumer<TKey, TValue> GetConsumer(string topicName)
        {
            if (_consumerInstances.TryGetValue(topicName, out IConsumer<TKey, TValue> consumer))
            {
                return consumer;
            }

            consumer = CreateConsumer(topicName);

            return consumer;
        }

        private IConsumer<TKey, TValue> CreateConsumer(string topicName)
        {
            _logger.LogInformation(
                string.Format(CultureInfo.InvariantCulture, InfoMessages.CreatingConsumer, topicName));
            var schemaRegistry = _schemaRegistryFactory.GetSchemaRegistry();
            var consumer = _consumerFactory.GetConsumer(schemaRegistry);
            consumer.Subscribe(topicName);
            _schemaRegistryInstances.TryAdd(topicName, schemaRegistry);
            _consumerInstances.TryAdd(topicName, consumer);
            return consumer;
        }

        public IEnumerable<TopicPartitionOffset> Commit(string topicName)
        {
            try
            {
                var consumer = GetConsumer(topicName);
                var commitedOffsets = consumer.Commit();
                commitedOffsets.ForEach(item =>
                    _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, InfoMessages.CommitedMessages,
                        item.Topic, item.Partition, item.Offset)));
                return commitedOffsets;
            }
            catch (TopicPartitionOffsetException e)
            {
                _logger.LogError(e,
                    string.Format(CultureInfo.InvariantCulture, ErrorMessages.PartitionOffsetError, e.Error.Reason));
                return new List<TopicPartitionOffset>();
            }
            catch (KafkaException e)
            {
                _logger.LogError(e,
                    string.Format(CultureInfo.InvariantCulture, ErrorMessages.CommitError, e.Error.Reason));
                return new List<TopicPartitionOffset>();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
                return;
            if (isDisposing)
            {
                if (_schemaRegistryInstances != null)
                {
                    _logger.LogInformation(InfoMessages.DisposingSchemaRegistry);
                    _schemaRegistryInstances.Values.ToList().ForEach(item => item.Dispose());
                }

                if (_consumerInstances != null)
                {
                    _logger.LogInformation(InfoMessages.ClosingConsumer);
                    _consumerInstances.Values.ToList().ForEach(item => item.Close());
                }
            }

            _alreadyDisposed = true;
        }


        private readonly ConcurrentDictionary<string, IConsumer<TKey, TValue>> _consumerInstances;
        private readonly ConcurrentDictionary<string, ISchemaRegistryClient> _schemaRegistryInstances;
        private readonly ILogger<MessageConsumer<TKey, TValue>> _logger;
        private readonly IConsumerFactory<TKey, TValue> _consumerFactory;
        private readonly ISchemaRegistryFactory _schemaRegistryFactory;
        private bool _alreadyDisposed;
    }
}