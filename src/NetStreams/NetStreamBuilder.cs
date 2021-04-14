using System;
using NetStreams.Configuration;
using NetStreams.Internal;

namespace NetStreams
{
    public class NetStreamBuilder
    {
        readonly NetStreamConfiguration _configurationContext = new NetStreamConfiguration();

        public NetStreamBuilder(Action<INetStreamConfigurationBuilderContext> setup)
        {
            setup(_configurationContext);
        }

        public INetStream<TKey, TMessage> Stream<TKey, TMessage>(string topic)
        {
            return new NetStream<TKey, TMessage>(topic,
                _configurationContext,
                new ConsumerFactory(),
                new ProducerFactory(),
                new TopicCreator(_configurationContext));
        }
    }
}

