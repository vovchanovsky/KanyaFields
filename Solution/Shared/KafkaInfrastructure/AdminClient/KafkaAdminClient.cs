using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace KafkaInfrastructure.AdminClient
{
    public static class KafkaAdminClient
    {
        public static async Task CreateTopicAsync(string bootstrapServers, string topicName)
        {
            using var adminClient = new AdminClientBuilder(
                new AdminClientConfig
                {
                    BootstrapServers = bootstrapServers
                }).Build();

            try
            {
                await adminClient.CreateTopicsAsync(
                    new TopicSpecification[] {
                        new()
                        {
                            Name = topicName, ReplicationFactor = 1, NumPartitions = 1 

                        }
                    });
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
            }
        }
    }
}
