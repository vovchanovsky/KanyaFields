using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaInfrastructure.Producer
{
    public interface IMessageProducer<TKey, TValue>
    {
        public Task<DeliveryResult<TKey, TValue>> SendMessageAsync(TKey key, TValue message, string topic);
    }
}