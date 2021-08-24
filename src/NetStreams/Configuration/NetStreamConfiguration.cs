using System;
using System.Collections.Generic;
using Confluent.Kafka;
using NetStreams.Authentication;
using NetStreams.Internal;
using NetStreams.Logging;

namespace NetStreams.Configuration
{
    public class NetStreamConfiguration<TKey, TMessage> : INetStreamConfigurationContext, INetStreamConfigurationBuilderContext<TKey, TMessage>
    {
        public TimeSpan HeartBeatDelayMs { get; set; } = TimeSpan.FromSeconds(30);
        public INetStreamTelemetryClient TelemetryClient = new NoOpTelemetryClient();
        public ILog Log { get; set; } = new LogContext();
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.At_Least_Once;
        public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Latest;
        public string BootstrapServers { get; set; }
        public string ConsumerGroup { get; set; }
        public List<ITopicConfiguration> TopicConfigurations { get; set; } = new List<ITopicConfiguration>();
        public bool TopicCreationEnabled { get; private set; }
        public AuthenticationMethod AuthenticationMethod { get; set; } = new PlainTextAuthentication();
        public bool ShouldSkipMalformedMessages { get; set; } = true;

        public Stack<PipelineStep<TKey, TMessage>> PipelineSteps { get; set; } =
            new Stack<PipelineStep<TKey, TMessage>>();

        public bool EnableMessageTypeHeader { get; set; } = true;
        public bool ContinueOnError { get; set; } = true;

        public INetStreamConfigurationBuilderContext<TKey, TMessage> ConfigureLogging(Action<LogContext> cfg)
        {
            var loggingContext = new LogContext();
            cfg(loggingContext);
            Log = loggingContext;
            return this;
        }

        public INetStreamConfigurationBuilderContext<TKey, TMessage> UseAuthentication(AuthenticationMethod authenticationMethod)
        {
            AuthenticationMethod = authenticationMethod;
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

        public INetStreamConfigurationBuilderContext<TKey, TMessage> SendTelemetry(INetStreamTelemetryClient telemetryClient)
        {
            TelemetryClient = telemetryClient;
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
        /// <summary>
        /// NetStreams emits a heart beat via the telemetry client.  This value is the timespan between heartbeats. The default value is 30 seconds.
        /// </summary>
        TimeSpan HeartBeatDelayMs { get; set; }
        AutoOffsetReset AutoOffsetReset { get; set; }
        /// <summary>
        /// A boolean value that instructs NetStreams to skip the message if it is malformed.  The default value is true.
        /// </summary>
        bool ShouldSkipMalformedMessages { get; set; }
        string ConsumerGroup { get; set; }
        string BootstrapServers { get; set; }
        /// <summary>
        /// The delivery mode set for the stream.  Values are DeliveryMode.At_Most_Once, DeliveryMode.At_Least_Once, or a custom delivery mode
        /// </summary>
        DeliveryMode DeliveryMode { get; set; }        
        /// <summary>
        /// A stack of pipeline steps that will be added to the ConsumePipeline.
        /// </summary>
        Stack<PipelineStep<TKey, TMessage>> PipelineSteps { get; }
        /// <summary>
        /// By default the EnableMessageTypeHeader boolean is set to true.  This will instruct the
        /// NetStreams producer to add the message type as a header to the Kafka Message.  When setting
        /// this value to false the header will not be added to the Kafka Message.
        /// </summary>
        bool EnableMessageTypeHeader { get; set; }

        /// <summary>
        /// By default the ContinueOnError boolean is set to true. This will instruct Netstreams to move on to the next offset 
        /// when encountering an error consuming a message.  If this value is set to false, NetStreams will indefinitely retry the same
        /// message until it is committed successfully.
        /// </summary>
        bool ContinueOnError { get; set; }

        INetStreamConfigurationBuilderContext<TKey, TMessage> AddTopicConfiguration(Action<ITopicConfiguration> cfg);
        INetStreamConfigurationBuilderContext<TKey, TMessage> UseAuthentication(AuthenticationMethod authenticationMethod);
        INetStreamConfigurationBuilderContext<TKey, TMessage> ConfigureLogging(Action<LogContext> cfg);
        INetStreamConfigurationBuilderContext<TKey, TMessage> SendTelemetry(INetStreamTelemetryClient telemetryClient);
    }
}