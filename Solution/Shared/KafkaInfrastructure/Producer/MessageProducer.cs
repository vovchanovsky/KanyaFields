using System.Globalization;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaInfrastructure.Resources;
using KafkaInfrastructure.SchemaRegistry;
using Microsoft.Extensions.Logging;

namespace KafkaInfrastructure.Producer
{
    public class MessageProducer<TKey, TValue> : IMessageProducer<TKey, TValue>
    {
        private readonly ILogger<MessageProducer<TKey, TValue>> _logger;
        private readonly IProducerFactory<TKey, TValue> _producerFactory;
        private readonly ISchemaRegistryFactory _schemaRegistryFactory;

        public MessageProducer(
            ILogger<MessageProducer<TKey, TValue>> logger,
            IProducerFactory<TKey, TValue> producerFactory, 
            ISchemaRegistryFactory schemaRegistryFactory)
        {
            _logger = logger;
            _producerFactory = producerFactory;
            _schemaRegistryFactory = schemaRegistryFactory;
        }

        public async Task<DeliveryResult<TKey, TValue>> SendMessageAsync(TKey key, TValue message, string topic)
        {
            var messageToSend = new Message<TKey, TValue>
            {
                Key = key,
                Value = message
            };

            using var schemaRegistry = _schemaRegistryFactory.GetSchemaRegistry();
            using var producer = _producerFactory.GetProducer(schemaRegistry);

            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, messageToSend).ConfigureAwait(false);
                _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, InfoMessages.DeliveredMessage, deliveryResult.Value, deliveryResult.TopicPartitionOffset));
                return deliveryResult;
            }
            catch (ProduceException<TKey, TValue> ex)
            {
                _logger.LogError(ex, string.Format(CultureInfo.InvariantCulture, ErrorMessages.DeliveryFailed, ex.Error.Reason));
                return null;
            }
        }
    }
}