using System;
using Confluent.Kafka;

namespace NetStreams.Configuration
{
    public static class ConfigurationExtensions
    {
        public static ProducerConfig ToProducerConfig(this INetStreamConfigurationContext config)
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = config.BootstrapServers,
            };
            config.AuthenticationMethod.Apply(producerConfig);
            return producerConfig;
        }

        public static ConsumerConfig ToConsumerConfig(this INetStreamConfigurationContext config)
        {
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = config.BootstrapServers,
                GroupId = config.ConsumerGroup,
                EnableAutoCommit = config.DeliveryMode.EnableAutoCommit,
                AutoCommitIntervalMs = config.DeliveryMode.AutoCommitIntervalMs,
                AutoOffsetReset = config.AutoOffsetReset,
            };
            config.AuthenticationMethod.Apply(consumerConfig);
            return consumerConfig;
        }
    }
}
