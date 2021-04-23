﻿using System;
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
        string SslKeystoreLocation { get; set; }
        string SslKeyPassword { get; set; }

        INetStreamConfigurationContext AddTopicConfiguration(Action<ITopicConfiguration> cfg);
    }

}