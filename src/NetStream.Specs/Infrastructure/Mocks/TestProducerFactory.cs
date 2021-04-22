using System;
using Moq;
using NetStreams.Configuration;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    public class TestProducerFactory : IProducerFactory
    {
        object _mock;
        public TestProducerFactory(Object mock)
        {
            _mock = mock;
        }

        public IMessageProducer<TKey, TMessage> Create<TKey, TMessage>(string topic, INetStreamConfigurationContext config)
        {
            return ((Mock<IMessageProducer<TKey, TMessage>>)_mock).Object;
        }
    }
}
