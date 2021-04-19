using Confluent.Kafka;

namespace NetStreams.Configuration.Internal
{
    public static class ConfigurationExtensions
    {
        public static ProducerConfig ToProducerConfig(this INetStreamConfigurationContext config)
        {
            return new ProducerConfig()
            {
                BootstrapServers = config.BootstrapServers
            };
        }

        public static ConsumerConfig ToConsumerConfig(this INetStreamConfigurationContext config)
        {
            return new ConsumerConfig()
            {
                BootstrapServers = config.BootstrapServers,
                GroupId = config.ConsumerGroup,
                EnableAutoCommit = config.DeliveryMode.EnableAutoCommit,
                AutoCommitIntervalMs = config.DeliveryMode.AutoCommitIntervalMs,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }
    }
}
