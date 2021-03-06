﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Internal;
using NetStreams.Internal.Pipeline;
using NetStreams.Logging;

namespace NetStreams
{
    public class NetStreamBuilder<TKey, TMessage> : INetStreamBuilder<TKey, TMessage>
    {
        readonly NetStreamConfiguration<TKey, TMessage> _configurationContext = new NetStreamConfiguration<TKey, TMessage>();
        public INetStreamConfigurationContext Configuration => _configurationContext;
        public ILog Log => _configurationContext.Log;
        readonly IConsumePipeline<TKey, TMessage> _pipeline = new ConsumePipeline<TKey, TMessage>();
        string _consumerTopic;
        Action<Exception> _onError;

        public NetStreamBuilder(Action<INetStreamConfigurationBuilderContext<TKey, TMessage>> setup)
        {
            setup(_configurationContext);
        }

        public INetStreamBuilder<TKey, TMessage> Stream(string topic)
        {
            _consumerTopic = topic;
            return this;
        }

        public INetStream Build()
        {
            var consumer = new ConsumerFactory().Create<TKey, TMessage>(_configurationContext);

            BuildPipeline(consumer);

            return new NetStream<TKey, TMessage>(_consumerTopic,
                _configurationContext,
                consumer,
                new TopicCreator(_configurationContext, _configurationContext.Log),
                _configurationContext.Log,
                _pipeline,
                _onError);
        }

        void BuildPipeline(IConsumer<TKey, TMessage> consumer)
        {
            while (_configurationContext.PipelineSteps.Any())
            {
                var step = _configurationContext.PipelineSteps.Pop();
                _pipeline.PrependStep(step);
            }

            if (!_configurationContext.DeliveryMode.EnableAutoCommit)
                _pipeline.PrependStep(new ConsumerCommitBehavior<TKey, TMessage>(consumer));

            _pipeline.AppendStep(new NoOpStep<TKey, TMessage>());
        }

        public INetStreamBuilder<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            _pipeline.AppendStep(new HandleStep<TKey, TMessage>(handle));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handle)
        {
            _pipeline.AppendStep(new AsyncHandleStep<TKey, TMessage>(handle));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> ToTopic<TResponseKey, TResponseMessage>(string topic, Func<TResponseMessage, TResponseKey> resolveKey)
        {
            var producer = new ProducerFactory().Create<TResponseKey, TResponseMessage>(topic, Configuration);

            var writer = new KafkaTopicWriter<TResponseKey, TResponseMessage>(producer, resolveKey);

            _pipeline.AppendStep(new WriteStreamStep<TKey, TMessage>(writer));

            return this;
        }

        public INetStreamBuilder<TKey, TMessage> Transform(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _pipeline.AppendStep(new TransformStep<TKey, TMessage>(handle));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> TransformAsync(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle)
        {
            _pipeline.AppendStep(new AsyncTransformStep<TKey, TMessage>(handle));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate)
        {
            _pipeline.AppendStep(new Filter<TKey, TMessage>(filterPredicate));
            return this;
        }
        public INetStreamBuilder<TKey, TMessage> OnError(Action<Exception> onError)
        {
            _onError = onError;
            return this;
        }
    }
}

