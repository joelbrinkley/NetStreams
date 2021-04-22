using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Configuration.Internal;
using NetStreams.Serialization;

namespace NetStreams.Internal
{
    internal class ProducerFactory : IProducerFactory
    {
        internal ProducerFactory()
        {
        }

        public IMessageProducer<TKey, TMessage> Create<TKey, TMessage>(string topic, INetStreamConfigurationContext config)
        {
            var producer = new ProducerBuilder<TKey, TMessage>(config.ToProducerConfig())
                 .SetValueSerializer(new HeaderSerializationStrategy<TMessage>())
                 .Build();

            return new NetStreamProducer<TKey, TMessage>(topic, producer);
        }
    }
}