using System;
using System.Collections.Generic;
using Confluent.Kafka;
using NetStreams.Logging;

namespace NetStreams.Configuration
{
    public class NetStreamConfiguration<TKey, TMessage> : INetStreamConfigurationContext, INetStreamConfigurationBuilderContext<TKey, TMessage>
    {
        public ILog Log { get; set; } = new LogContext();
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.At_Least_Once;
        public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Latest;
        public string BootstrapServers { get; set; }
        public string ConsumerGroup { get; set; }
        public List<ITopicConfiguration> TopicConfigurations { get; set; } = new List<ITopicConfiguration>();
        public bool TopicCreationEnabled { get; private set; }
        public string SecurityProtocol { get; set; }
        public string SslCertificateLocation { get; set; }
        public string SslCaLocation { get; set; }
        public string SslKeyLocation { get; set; }
        public string SslKeyPassword { get; set; }
        public string SaslMechanism { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
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

        public INetStreamConfigurationBuilderContext<TKey, TMessage> AddTopicConfiguration(Action<ITopicConfiguration> cfg)
        {
            TopicCreationEnabled = true;
            var topicConfig = new TopicConfiguration();
            cfg(topicConfig);
            TopicConfigurations.Add(topicConfig);
            return this;
        }

        public INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithPlaintext()
        {
            SecurityProtocol = "PLAINTEXT";
            return this;
        }
        
        public INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSsl(string sslCaCertPath, string sslClientCertPath, string sslClientKeyPath, string sslClientKeyPwd)
        {
            SecurityProtocol = "SSL";
            SslCaLocation = sslCaCertPath;
            SslCertificateLocation = sslClientCertPath;
            SslKeyLocation = sslClientKeyPath;
            SslKeyPassword = sslClientKeyPwd;
            return this;
        }
        
        public INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSaslScram256(string sslCaCertPath, string username, string password)
        {
            SecurityProtocol = "SaslSsl";
            SaslMechanism = "ScramSha256";
            SslCaLocation = sslCaCertPath;
            SaslUsername = username;
            SaslPassword = password;
            return this;
        }

        public INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSaslScram512(string sslCaCertPath, string username, string password)
        {
            SecurityProtocol = "SaslSsl";
            SaslMechanism = "ScramSha512";
            SslCaLocation = sslCaCertPath;
            SaslUsername = username;
            SaslPassword = password;
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
        AutoOffsetReset AutoOffsetReset { get; set; }
        bool ShouldSkipMalformedMessages { get; set; }
        string ConsumerGroup { get; set; }
        string BootstrapServers { get; set; }
        DeliveryMode DeliveryMode { get; set; }
        string SecurityProtocol { get; set; }
        string SslCertificateLocation { get; set; }
        string SslCaLocation { get; set; }
        string SslKeyLocation { get; set; }
        string SslKeyPassword { get; set; }
        Stack<PipelineStep<TKey, TMessage>> PipelineSteps { get; }
        /// <summary>
        /// By default the EnableMessageTypeHeader boolean is set to true.  This will instruct the
        /// NetStreams producer to add the message type as a header to the Kafka Message.  When setting
        /// this value to false the header will not be added to the Kafka Message.
        /// </summary>
        bool EnableMessageTypeHeader { get; set; }
        bool ContinueOnError { get; set; }

        INetStreamConfigurationBuilderContext<TKey, TMessage> AddTopicConfiguration(Action<ITopicConfiguration> cfg);
        INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithPlaintext();
        INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSsl(string sslCaCertPath, string sslClientCertPath, string sslClientKeyPath, string sslClientKeyPwd);
        INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSaslScram256(string sslCaCertPath, string username, string password);
        INetStreamConfigurationBuilderContext<TKey, TMessage> AuthenticateWithSaslScram512(string sslCaCertPath, string username, string password);
        INetStreamConfigurationBuilderContext<TKey, TMessage> ConfigureLogging(Action<LogContext> cfg);
    }
}