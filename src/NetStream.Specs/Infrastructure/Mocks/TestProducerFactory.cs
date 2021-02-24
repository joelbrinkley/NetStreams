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

        public IMessageProducer<TMessage> Create<TMessage>(INetStreamConfigurationContext config) where TMessage : IMessage
        {
            return ((Mock<IMessageProducer<TMessage>>)_mock).Object;
        }
    }
}
