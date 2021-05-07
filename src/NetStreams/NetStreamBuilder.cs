using System;
using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Internal;
using NetStreams.Internal.Behaviors;

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
            var consumer = new ConsumerFactory().Create<TKey, TMessage>(_configurationContext);

            var consumeProcessor = DefaultProcessor(consumer);

            return new NetStream<TKey, TMessage>(topic,
                _configurationContext,
                consumer,
                new TopicCreator(_configurationContext),
                consumeProcessor);
        }

        IConsumeProcessor<TKey, TMessage> DefaultProcessor<TKey, TMessage>(IConsumer<TKey, TMessage> consumer)
        {
            var processor = new ConsumeProcessor<TKey, TMessage>();

            if (_configurationContext.DeliveryMode.EnableAutoCommit) 
                processor.AddBehavior(new AutoCommitBehavior<TKey, TMessage>(consumer));

            return processor;
        }
    }
}

