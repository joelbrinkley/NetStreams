using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Internal;
using NetStreams.Internal.Behaviors;

namespace NetStreams
{
    public class NetStreamBuilder<TKey, TMessage> : INetStreamBuilder<TKey, TMessage>
    {
        readonly NetStreamConfiguration _configurationContext = new NetStreamConfiguration();
        public INetStreamConfigurationContext Configuration => _configurationContext;

        readonly IConsumeProcessor<TKey, TMessage> _processor = new ConsumeProcessor<TKey, TMessage>();
        string _consumerTopic;
        IStreamWriter _writer;
        Action<Exception> _onError;

        public NetStreamBuilder(Action<INetStreamConfigurationBuilderContext> setup)
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

            if (!_configurationContext.DeliveryMode.EnableAutoCommit) 
                _processor.AddFirstBehavior(new ConsumerCommitBehavior<TKey, TMessage>(consumer));

            return new NetStream<TKey, TMessage>(_consumerTopic,
                _configurationContext,
                consumer,
                new TopicCreator(_configurationContext),
                _processor);
        }

        public INetStreamBuilder<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            Func<IConsumeContext<TKey, TMessage>, object> actionWrapper = (context) =>
            {
                handle(context);
                return null; //TODO: figure out what to do with this default value
            };

            _processor.AddBehavior(new ConsumeTransformer<TKey, TMessage>(actionWrapper));

            return this;
        }

        public INetStreamBuilder<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handle)
        {
            Func<IConsumeContext<TKey, TMessage>, Task<object>> actionWrapper = async (context) =>
            {
                await handle(context);
                return null; //TODO: figure out what to do with this default value
            };

            _processor.AddBehavior(new AsyncConsumeTransformer<TKey, TMessage>(actionWrapper, _writer));

            return this;
        }

        public INetStreamBuilder<TKey, TMessage> ToTopic<TResponseKey, TResponseMessage>(string topic, Func<TResponseMessage, TResponseKey> resolveKey)
        {
            var producer = new ProducerFactory().Create<TResponseKey, TResponseMessage>(topic, Configuration);

            var writer = new KafkaTopicWriter<TResponseKey, TResponseMessage>(producer, resolveKey);

           _processor.AddBehavior(new WriteOutputToKafkaBehavior<TKey, TMessage>(writer));

           return this;
        }

        public INetStreamBuilder<TKey, TMessage> Transform(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _processor.AddBehavior(new ConsumeTransformer<TKey, TMessage>(handle));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> TransformAsync(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle)
        {
            _processor.AddBehavior(new AsyncConsumeTransformer<TKey, TMessage>(handle, _writer));
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate)
        {
            _processor.AddBehavior(new FilterBehavior<TKey, TMessage>(filterPredicate));
            return this;
        }
        public INetStreamBuilder<TKey, TMessage> OnError(Action<Exception> onError)
        {
            _onError = onError;
            return this;
        }

        public INetStreamBuilder<TKey, TMessage> AddBehavior(ConsumeBehavior<TKey, TMessage> behavior)
        {
            _processor.AddBehavior(behavior);
            return this;
        }

        public void SetWriter(IStreamWriter writer)
        {
            _writer = writer;
        }
    }
}

