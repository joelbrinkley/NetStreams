using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Configuration.Internal;
using NetStreams.Serialization;

namespace NetStreams.Internal
{
    internal class ConsumerFactory : IConsumerFactory
    {
        internal ConsumerFactory()
        {
        }
        public IConsumer<TKey, TValue> Create<TKey, TValue>(INetStreamConfigurationContext config)
        {
            return new ConsumerBuilder<TKey, TValue>(config.ToConsumerConfig())
                 .SetValueDeserializer(new JsonSer<TValue>())
                 .Build();
        }
    }


}