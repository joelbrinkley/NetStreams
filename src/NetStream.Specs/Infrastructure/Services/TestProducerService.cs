using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Serialization;

namespace NetStreams.Specs.Infrastructure.Services
{
    public class TestProducerService<TKey, TMessage>
    {
        readonly string _topic;
        IMessageProducer<TKey, TMessage> _producer;

        public TestProducerService(string topic, Action<ProducerConfig> configure)
        {
            var producerConfig = new ProducerConfig();
            configure(producerConfig);
            var kafkaProducer = new ProducerBuilder<TKey, TMessage>(producerConfig)
                .SetValueSerializer(new HeaderSerializationStrategy<TMessage>())
                .Build();

            _producer = new NetStreamProducer<TKey, TMessage>(topic, kafkaProducer);
            _topic = topic;
        }

        public void Produce(TKey key, TMessage message)
        {
            _producer.ProduceAsync(key,  message).Await();
        }

        public async Task ProduceAsync(TKey key, TMessage message)
        {
            await _producer.ProduceAsync(key, message);
        }
    }

    public static class TestProducerFactory
    {
        public static TestProducerService<TKey, TMessage> Plaintext<TKey, TMessage>(string topic)
        {
            return new TestProducerService<TKey, TMessage>(
                topic,
                config => { config.BootstrapServers = "localhost:9092"; });
        }

        public static TestProducerService<TKey, TMessage> SaslScram256<TKey, TMessage>(string topic)
        {
            var sslCaCertPath = GetSslSecretPath("snakeoil-ca-1.crt");
            return new TestProducerService<TKey, TMessage>(
                topic,
                config =>
                {
                    config.BootstrapServers = "localhost:9095";
                    config.SecurityProtocol = SecurityProtocol.SaslSsl;
                    config.SslCaLocation = sslCaCertPath;
                    config.SaslMechanism = SaslMechanism.ScramSha256;
                    config.SaslUsername = "client";
                    config.SaslPassword = "client-secret";
                });
        }

        public static TestProducerService<TKey, TMessage> Ssl<TKey, TMessage>(string topic)
        {
            var sslCaCertPath = GetSslSecretPath("snakeoil-ca-1.crt");
            var sslClientCertPath = GetSslSecretPath("client.pem");
            var sslClientKeyPath = GetSslSecretPath("client.key");
            return new TestProducerService<TKey, TMessage>(
                topic,
                config =>
                {
                    config.BootstrapServers = "localhost:9093";
                    config.SecurityProtocol = SecurityProtocol.Ssl;
                    config.SslCaLocation = sslCaCertPath;
                    config.SslCertificateLocation = sslClientCertPath;
                    config.SslKeyLocation = sslClientKeyPath;
                    config.SslKeyPassword = "confluent";
                });
        }

        public static string GetSslSecretPath(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ssl", filename);
        }
    }
}
