using System;
using Confluent.Kafka;
using Moq;
using NetStreams.Configuration;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    public class TestConsumerFactory : IConsumerFactory
    {
        object _mock;
        public TestConsumerFactory(Object mock)
        {
            _mock = mock;
        }

        public IConsumer<TKey, T> Create<TKey, T>(INetStreamConfigurationContext config)
        {
            return ((Mock<IConsumer<TKey, T>>)_mock).Object;
        }
    }
}
