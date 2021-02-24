using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace NetStreams.Configuration
{
    public interface INetStreamConfigurationContext
    {
        string BootstrapServers { get; }
        string ConsumerGroup { get; }
        List<ITopicConfiguration> TopicConfigurations { get; }
        bool TopicCreationEnabled { get;  }

        INetStreamConfigurationContext AddTopicConfiguration(Action<ITopicConfiguration> cfg);
    }

}