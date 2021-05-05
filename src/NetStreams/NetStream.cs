using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Internal.Exceptions;

namespace NetStreams
{
    public class NetStream<TKey, TMessage> : INetStream<TKey, TMessage>
    {
        IHandle<TKey, TMessage> _handler;
        IConsumer<TKey, TMessage> _consumer;
        IConsumerFactory _consumerFactory;
        readonly ITopicCreator _topicCreator;
        string _topic;
        Func<IConsumeContext<TKey, TMessage>, bool> _filterPredicate = (consumeContext) => true;
        NetStreamConfiguration _configuration;
        bool disposedValue;
        Action<Exception> _onError = exception => { throw new StreamFaultedException(exception); };
        Task _streamTask;

        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStream(
            string topic,
            NetStreamConfiguration configuration,
            IConsumerFactory consumerFactory,
            ITopicCreator topicCreator)
        {
            _configuration = configuration;
            _topic = topic;
            _consumerFactory = consumerFactory;
            _topicCreator = topicCreator;
        }

        public Task StartAsync(CancellationToken token)
        {
            if (Configuration.TopicCreationEnabled)
            {
                CreateTopics().Wait(token);
            }

            _consumer = _consumerFactory.Create<TKey, TMessage>(Configuration);

            _consumer.Subscribe(_topic);

            _streamTask = Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(100);

                        var consumeContext = new ConsumeContext<TKey, TMessage>(consumeResult);

                        if (consumeResult != null)
                        {
                            if (_filterPredicate(consumeContext))
                            {
                                if (_handler != null)
                                {
                                    _handler.Handle(consumeContext).Wait(token);
                                }
                            }

                            if (!Configuration.DeliveryMode.EnableAutoCommit) _consumer.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        _onError(ex);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

            return _streamTask;
        }

        async Task CreateTopics()
        {
            foreach (var topicConfig in Configuration.TopicConfigurations)
            {
                await _topicCreator.Create(topicConfig);
            }
        }

        public IHandle<TKey, TMessage, TResponseKey, TResponse> Handle<TResponseKey, TResponse>(Func<IConsumeContext<TKey, TMessage>, TResponse> handle)
        {
            var handler = new HandleFunction<TKey, TMessage, TResponseKey, TResponse>(handle, this);
            _handler = handler;
            return handler;
        }

        public IHandle<TKey, TMessage, TResponseKey, TResponse> HandleAsync<TResponseKey, TResponse>(Func<IConsumeContext<TKey, TMessage>, Task<TResponse>> handle)
        {
            var handler = new HandleFunctionTask<TKey, TMessage, TResponseKey, TResponse>(handle, this);
            _handler = handler;
            return handler;
        }

        public INetStream<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            _handler = new HandleAction<TKey, TMessage>(handle, this);
            return this;
        }

        public INetStream<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handleTask)
        {
            _handler = new HandleActionTask<TKey, TMessage>(handleTask, this);
            return this;
        }

        public INetStream<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate)
        {
            _filterPredicate = filterPredicate;
            return this;
        }

        public INetStream<TKey, TMessage> OnError(Action<Exception> onError)
        {
            _onError = onError;
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
