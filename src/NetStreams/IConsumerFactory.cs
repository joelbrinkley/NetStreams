using Confluent.Kafka;
using NetStreams.Configuration;

namespace NetStreams
{
    public interface IConsumerFactory
    {
        IConsumer<TKey, TValue> Create<TKey, TValue>(INetStreamConfigurationContext config);
    }
}
