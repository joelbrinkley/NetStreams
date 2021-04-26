using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using NetStreams.Configuration;
using NetStreams.Internal.Extensions;
using NetStreams.Configuration.Internal;

namespace NetStreams.Internal
{
    internal class TopicCreator : ITopicCreator
    {
        readonly NetStreamConfiguration _configuration;
        readonly IAdminClient _adminClient;

        public TopicCreator(NetStreamConfiguration configuration)
        {
            _configuration = configuration;

            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = configuration.BootstrapServers,
                SslCertificateLocation = configuration.SslCertificateLocation,
                SslCaLocation = configuration.SslCaLocation,
                SslKeyLocation = configuration.SslKeyLocation,
                SslKeyPassword = configuration.SslKeyPassword,
                SecurityProtocol = configuration.ParseSecurityProtocol()
            };

            _adminClient = new AdminClientBuilder(adminConfig).Build();
        }

        public async Task Create(ITopicConfiguration topicConfig)
        {
            if (!_adminClient.TopicExists(topicConfig.Name, out _))
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
                    await _adminClient.CreateTopicsAsync(new[] { topicSpecification });
                }
                catch (CreateTopicsException ex)
                {
                    Console.WriteLine(ex);
                }

                _adminClient.EnsureTopicCreation(topicConfig.Name);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
    public interface ITopicCreator : IDisposable
    {
        Task Create(ITopicConfiguration topicConfig);
    }
}
