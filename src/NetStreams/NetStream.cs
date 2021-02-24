using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public class NetStream<T> : INetStream<T> where T : IMessage
    {
        IHandle<T> _handler;
        IConsumer<string, T> _consumer;
        IConsumerFactory _consumerFactory;
        IProducerFactory _producerFactory;
        readonly ITopicCreator _topicCreator;
        string _topic;
        Func<IConsumeContext<T>, bool> _filterPredicate = (consumeContext) => true;
        NetStreamConfiguration _configuration;
        bool disposedValue;

        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStream(
            string topic,
            NetStreamConfiguration configuration,
            IConsumerFactory consumerFactory,
            IProducerFactory producerFactory,
            ITopicCreator topicCreator)
        {
            _configuration = configuration;
            _topic = topic;
            _consumerFactory = consumerFactory;
            _producerFactory = producerFactory;
            _topicCreator = topicCreator;
        }

        public Task StartAsync(CancellationToken token)
        {
            if (Configuration.TopicCreationEnabled)
            {
                CreateTopics().Wait(token);
            }

            _consumer = _consumerFactory.Create<string, T>(Configuration);
            _consumer.Subscribe(_topic);

            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(100);

                        var consumeContext = new ConsumeContext<T>(consumeResult);

                        if (consumeResult != null)
                        {
                            if (_filterPredicate(consumeContext))
                            {
                                if (_handler != null)
                                {
                                    await _handler.Handle(consumeContext);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        async Task CreateTopics()
        {
            foreach (var topicConfig in Configuration.TopicConfigurations)
            {
                await _topicCreator.Create(topicConfig);
            }
        }

        public IHandle<T, TResponse> Handle<TResponse>(Func<IConsumeContext<T>, TResponse> handle) where TResponse : IMessage
        {
            var handler = new HandleFunction<T, TResponse>(handle, this);
            _handler = handler;
            return handler;
        }

        public INetStream<T> Handle(Action<IConsumeContext<T>> handle)
        {
            _handler = new HandleAction<T>(handle, this);
            return this;
        }

        public INetStream<T> Filter(Func<IConsumeContext<T>, bool> filterPredicate)
        {
            _filterPredicate = filterPredicate;
            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _consumer?.Unsubscribe();
                    _consumer?.Close();
                    _consumer?.Dispose();
                    _topicCreator?.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~NetStream()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
