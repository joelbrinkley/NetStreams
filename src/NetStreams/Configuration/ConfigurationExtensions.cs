using System;
using Confluent.Kafka;

namespace NetStreams.Configuration
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
                SslKeyLocation = config.SslKeyLocation,
                SslKeyPassword = config.SslKeyPassword,
                SaslMechanism = ParseSaslMechanism(config),
                SaslUsername = config.SaslUsername,
                SaslPassword = config.SaslPassword
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
                AutoOffsetReset = config.AutoOffsetReset,
                SecurityProtocol = ParseSecurityProtocol(config),
                SslCertificateLocation = config.SslCertificateLocation,
                SslCaLocation = config.SslCaLocation,
                SslKeyLocation = config.SslKeyLocation,
                SslKeyPassword = config.SslKeyPassword,
                SaslMechanism = ParseSaslMechanism(config),
                SaslUsername = config.SaslUsername,
                SaslPassword = config.SaslPassword
            };
        }

        public static SecurityProtocol? ParseSecurityProtocol(this INetStreamConfigurationContext config)
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

        public static SaslMechanism? ParseSaslMechanism(this INetStreamConfigurationContext config)
        {
            if (!string.IsNullOrEmpty(config.SaslMechanism))
            {
                if (Enum.TryParse<SaslMechanism>(config.SaslMechanism, true, out var saslMechanism))
                {
                    return saslMechanism;
                }
                else
                {
                    throw new ArgumentException(nameof(config.SaslMechanism));
                }
            }

            return null;
        }
    }
}
