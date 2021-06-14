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
        string SecurityProtocol { get; set; }
        string SslCertificateLocation { get; set; }
        string SslCaLocation { get; set; }
        string SslKeyLocation { get; set; }
        string SslKeyPassword { get; set; }
        AutoOffsetReset AutoOffsetReset { get; set; }
    }

}