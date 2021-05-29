using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace KafkaInfrastructure.Consumer
{
    public interface IMessageConsumer<TKey, TValue> : IDisposable
    {
        ConsumeResult<TKey, TValue> Consume(string topicName);

        IEnumerable<TopicPartitionOffset> Commit(string topicName);
    }
}