using System;
using System.Collections.Generic;
using Confluent.Kafka;
using NetStreams.Authentication;

namespace NetStreams.Configuration
{
    public interface INetStreamConfigurationContext
    {
        bool EnableMessageTypeHeader { get; }
        string BootstrapServers { get; }
        string ConsumerGroup { get; }
        List<ITopicConfiguration> TopicConfigurations { get; }
        bool TopicCreationEnabled { get;  }
        DeliveryMode DeliveryMode { get; }
        AuthenticationMethod AuthenticationMethod { get; }
        AutoOffsetReset AutoOffsetReset { get; }
        TimeSpan HeartBeatDelayMs { get; }
    }

}