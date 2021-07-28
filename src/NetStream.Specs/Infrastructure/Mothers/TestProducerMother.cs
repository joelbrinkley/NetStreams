using System.IO;
using System.Reflection;
using Confluent.Kafka;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Infrastructure.Mothers
{
    public static class TestProducerMother
    {
        public static TestProducerService<TKey, TMessage> New<TKey, TMessage>(string topic)
        {
            return new TestProducerService<TKey, TMessage>(
                topic,
                config => { config.BootstrapServers = "localhost:9092"; });
        }

        public static TestProducerService<TKey, TMessage> UseSaslScram256<TKey, TMessage>(string topic)
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

        public static TestProducerService<TKey, TMessage> UseSsl<TKey, TMessage>(string topic)
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
