using Confluent.Kafka;
using System;

namespace NetStreams.Configuration.Internal
{
    public static class ConfigurationExtensions
    {
        public static ProducerConfig ToProducerConfig(this INetStreamConfigurationContext config)
        {
            return new ProducerConfig()
            {
                BootstrapServers = config.BootstrapServers,
                SecurityProtocol = ParseSecurityProtocol(config),
                SslCertificateLocation = config.SslCertificateLocation,
                SslCaLocation = config.SslCaLocation,
                SslKeystoreLocation = config.SslKeystoreLocation,
                SslKeyPassword = config.SslKeyPassword
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
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SecurityProtocol = ParseSecurityProtocol(config),
                SslCertificateLocation = config.SslCertificateLocation,
                SslCaLocation = config.SslCaLocation,
                SslKeystoreLocation = config.SslKeystoreLocation,
                SslKeyPassword = config.SslKeyPassword
            };
        }

        private static SecurityProtocol? ParseSecurityProtocol(this INetStreamConfigurationContext config)
        {
            if (!string.IsNullOrEmpty(config.SecurityProtocol))
            {
                if (Enum.TryParse<SecurityProtocol>(config.SecurityProtocol, true, out var securityProtocol))
                {
                    return securityProtocol;
                }
                else
                {
                    throw new ArgumentException(nameof(config.SecurityProtocol));
                }
            }

            return null;
        }
    }
}
