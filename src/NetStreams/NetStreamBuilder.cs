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

        public INetStream<T> Stream<T>(string topic) where T : IMessage
        {
            return new NetStream<T>(topic,
                _configurationContext,
                new ConsumerFactory(),
                new ProducerFactory(),
                new TopicCreator(_configurationContext));
        }
    }
}

