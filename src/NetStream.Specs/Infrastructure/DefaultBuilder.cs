using System;

namespace NetStreams.Specs.Infrastructure
{
    internal static class DefaultBuilder
    {
        public static INetStreamBuilder<TKey, TMessage> New<TKey, TMessage>()
        {
            return new NetStreamBuilder<TKey, TMessage>(cfg =>
            {
                cfg.BootstrapServers = "localhost:9092";
                cfg.ConsumerGroup = Guid.NewGuid().ToString();
            });
        }
    }
}
