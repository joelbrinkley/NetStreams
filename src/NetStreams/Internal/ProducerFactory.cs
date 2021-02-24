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

        public IMessageProducer<TMessage> Create<TMessage>(INetStreamConfigurationContext config) where TMessage: IMessage
        {
            var producer = new ProducerBuilder<string, TMessage>(config.ToProducerConfig())
                 .SetValueSerializer(new JsonSer<TMessage>())
                 .Build();

            return new NetStreamProducer<TMessage>(producer);
        }
    }
}