using System;
using System.IO;
using System.Reflection;
using Confluent.Kafka;
using NetStreams.Authentication;

namespace NetStreams.Specs.Infrastructure.Mothers
{
    internal static class DefaultBuilder
    {
        public static INetStreamBuilder<TKey, TMessage> New<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                cfg.BootstrapServers = "localhost:9092";
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
        }

        public static INetStreamBuilder<TKey, TMessage> UseSsl<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                var sslCaCertPath = GetSslSecretPath("snakeoil-ca-1.crt");
                var sslClientCertPath = GetSslSecretPath("client.pem");
                var sslClientKeyPath = GetSslSecretPath("client.key");
                cfg.BootstrapServers = "localhost:9093";
                cfg.UseAuthentication(new SslAuthentication(sslCaCertPath, sslClientCertPath, sslClientKeyPath, "confluent"));
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
            });
        }

        public static INetStreamBuilder<TKey, TMessage> UseSaslScram256<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                var sslCaCertPath = GetSslSecretPath("snakeoil-ca-1.crt");
                cfg.BootstrapServers = "localhost:9095";
                cfg.UseAuthentication(new SaslScram256Authentication("client", "client-secret", sslCaCertPath));
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
