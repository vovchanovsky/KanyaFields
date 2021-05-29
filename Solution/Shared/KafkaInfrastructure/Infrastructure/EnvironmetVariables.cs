namespace KafkaInfrastructure.Infrastructure
{
    public static class EnvironmentVariables
    {
        public const string KafkaServiceHost = "KAFKA_SERVICE_HOST";
        public const string KafkaServicePort = "KAFKA_SERVICE_PORT";

        public const string SchemaRegistryHost = "SR_SCHEMA_REGISTRY_SERVICE_HOST";
        public const string SchemaRegistryPort = "SR_SCHEMA_REGISTRY_SERVICE_PORT";

        public const string KafkaZookeeperServiceHost = "KAFKA_ZOOKEEPER_SERVICE_HOST";
        public const string KafkaZookeeperServicePort = "KAFKA_ZOOKEEPER_SERVICE_PORT";

        public const string ConsumerGroupId = "CONSUMER_GROUP_ID";
    }
}