using System;
using System.IO;
using System.Reflection;
using Confluent.Kafka;

namespace NetStreams.Specs.Infrastructure
{
    internal static class DefaultBuilder
    {
        public static INetStreamBuilder<TKey, TMessage> Plaintext<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                cfg.BootstrapServers = "localhost:9092";
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
        }

        public static INetStreamBuilder<TKey, TMessage> Ssl<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                var sslCaCertPath = GetSslSecretPath("ca.crt");
                var sslClientCertPath = GetSslSecretPath("client.pem");
                var sslClientKeyPath = GetSslSecretPath("client.key");
                cfg.BootstrapServers = "localhost:9093";
                cfg.AuthenticateWithSsl(sslCaCertPath, sslClientCertPath, sslClientKeyPath, "confluent");
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
        }

        public static INetStreamBuilder<TKey, TMessage> SaslScram256<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                var sslCaCertPath = GetSslSecretPath("ca.crt");
                cfg.BootstrapServers = "localhost:9095";
                cfg.AuthenticateWithSaslScram256(sslCaCertPath, "client", "client-secret");
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
        }

        public static string GetSslSecretPath(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ssl", filename);
        }
    }
}
