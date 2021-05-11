using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using NetStreams.Configuration;
using NetStreams.Internal.Extensions;
using NetStreams.Logging;

namespace NetStreams.Internal
{
    internal class TopicCreator : ITopicCreator
    {
        readonly ILog _log;
        readonly Lazy<IAdminClient> _adminClient;

        public TopicCreator(INetStreamConfigurationContext configuration, ILog log)
        {
            _log = log;
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = configuration.BootstrapServers,
                SslCertificateLocation = configuration.SslCertificateLocation,
                SslCaLocation = configuration.SslCaLocation,
                SslKeyLocation = configuration.SslKeyLocation,
                SslKeyPassword = configuration.SslKeyPassword,
                SecurityProtocol = configuration.ParseSecurityProtocol()
            };

            var adminClientBuilder = new AdminClientBuilder(adminConfig);
            _adminClient = new Lazy<IAdminClient>(() => adminClientBuilder.Build());
        }

        public async Task Create(ITopicConfiguration topicConfig)
        {
            if (!_adminClient.Value.TopicExists(topicConfig.Name, out _))
            {
                TopicSpecification topicSpecification = new TopicSpecification
                {
                    Name = topicConfig.Name,
                    NumPartitions = topicConfig.Partitions,
                    ReplicationFactor = topicConfig.ReplicationFactor,
                    Configs = new Dictionary<string, string>
                    {
                        {"retention.ms", topicConfig.RetentionMs.ToString()}
                    }
                };

                try
                {
                    await _adminClient.Value.CreateTopicsAsync(new[] { topicSpecification });
                    _log.Information($"{topicConfig.Name} created.");
                }
                catch (CreateTopicsException ex)
                {
                    _log.Error(ex, "An unexpected exception occurred when creating topics.");
                }

                _adminClient.Value.EnsureTopicCreation(topicConfig.Name);
            }
        }

        public async Task CreateAll(IEnumerable<ITopicConfiguration> topicConfigurations)
        {
            foreach (var topicConfig in topicConfigurations)
            {
                await Create(topicConfig);
            }
        }

        public void Dispose()
        {
            _adminClient.Value.Dispose();
        }

    }
    public interface ITopicCreator : IDisposable
    {
        Task Create(ITopicConfiguration topicConfig);
        Task CreateAll(IEnumerable<ITopicConfiguration> topicConfigurations);
    }
}
