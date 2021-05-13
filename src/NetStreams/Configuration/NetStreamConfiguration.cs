using System;
using System.Collections.Generic;
using NetStreams.Logging;

namespace NetStreams.Configuration
{
    public class NetStreamConfiguration<TKey, TMessage> : INetStreamConfigurationContext, INetStreamConfigurationBuilderContext<TKey, TMessage>
    {
        public ILog Log { get; set; } = new LogContext();
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.At_Least_Once;
        public string BootstrapServers { get; set; }
        public string ConsumerGroup { get; set; }
        public List<ITopicConfiguration> TopicConfigurations { get; set; } = new List<ITopicConfiguration>();
        public bool TopicCreationEnabled { get; private set; }
        public string SecurityProtocol { get; set; }
        public string SslCertificateLocation { get; set; }
        public string SslCaLocation { get; set; }
        public string SslKeyLocation { get; set; }
        public string SslKeyPassword { get; set; }
        public bool ShouldSkipMalformedMessages { get; set; } = true;

        public Stack<PipelineStep<TKey, TMessage>> PipelineSteps { get; set; } =
            new Stack<PipelineStep<TKey, TMessage>>();

        public INetStreamConfigurationBuilderContext<TKey, TMessage> ConfigureLogging(Action<LogContext> cfg)
        {
            var loggingContext = new LogContext();
            cfg(loggingContext);
            Log = loggingContext;
            return this;
        }

        public INetStreamConfigurationBuilderContext<TKey, TMessage> AddTopicConfiguration(Action<ITopicConfiguration> cfg)
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

    public interface INetStreamConfigurationBuilderContext<TKey, TMessage>
    {
        bool ShouldSkipMalformedMessages { get; set; }
        ILog Log { get; set; }
        string ConsumerGroup { get; set; }
        string BootstrapServers { get; set; }
        DeliveryMode DeliveryMode { get; set; }
        string SecurityProtocol { get; set; }
        string SslCertificateLocation { get; set; }
        string SslCaLocation { get; set; }
        string SslKeyLocation { get; set; }
        string SslKeyPassword { get; set; }
        Stack<PipelineStep<TKey, TMessage>> PipelineSteps { get; }

        INetStreamConfigurationBuilderContext<TKey, TMessage> AddTopicConfiguration(Action<ITopicConfiguration> cfg);

        INetStreamConfigurationBuilderContext<TKey, TMessage> ConfigureLogging(Action<LogContext> cfg);
    }
}