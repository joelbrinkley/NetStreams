using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace NetStreams.Configuration
{
    public class NetStreamConfiguration : INetStreamConfigurationContext, INetStreamConfigurationBuilderContext
    {
        public string BootstrapServers { get; set; }
        public string ConsumerGroup { get; set; }
        public List<ITopicConfiguration> TopicConfigurations { get; set; } = new List<ITopicConfiguration>();
        public bool TopicCreationEnabled { get; private set; }

        public INetStreamConfigurationContext AddTopicConfiguration(Action<ITopicConfiguration> cfg)
        {
            TopicCreationEnabled = true;
            var topicConfig = new TopicConfiguration();
            cfg(topicConfig);
            TopicConfigurations.Add(topicConfig);
            return this;
        }
    }

    public class TopicConfiguration : ITopicConfiguration
    {
        public string Name { get; set; }
        public int Partitions { get; set; } = 1;
        public long RetentionMs { get; set; } = 9999999999999;
        public short ReplicationFactor { get; set; } = 1;
    }

    public interface ITopicConfiguration
    {
        string Name { get; set; }
        int Partitions { get; set; }
        long RetentionMs { get; set; }
        short ReplicationFactor { get; set; }
    }

    public interface INetStreamConfigurationBuilderContext
    {
        string ConsumerGroup { get; set; }
        string BootstrapServers { get; set; }
        INetStreamConfigurationContext AddTopicConfiguration(Action<ITopicConfiguration> cfg);
    }
}