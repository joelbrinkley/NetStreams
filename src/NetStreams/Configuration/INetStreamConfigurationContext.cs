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
        DeliveryMode DeliveryMode { get; set; }

        INetStreamConfigurationContext AddTopicConfiguration(Action<ITopicConfiguration> cfg);
    }

}