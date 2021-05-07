using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Internal.Behaviors;

namespace NetStreams
{
    public class NetStream<TKey, TMessage> : INetStream<TKey, TMessage>
    {
        readonly ConsumeProcessor<TKey, TMessage> _processor;
        readonly ITopicCreator _topicCreator;
        readonly string _topic;
        readonly NetStreamConfiguration _configuration;
        IConsumerFactory _consumerFactory;
        IStreamWriter _writer;
        IConsumer<TKey, TMessage> _consumer;
        bool disposedValue;
        Action<Exception> _onError = exception => { };
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
            _processor = new ConsumeProcessor<TKey, TMessage>();
        }

        public async Task StartAsync(CancellationToken token)
        {
            if (Configuration.TopicCreationEnabled)
            {
                await _topicCreator.CreateAll(Configuration.TopicConfigurations);
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
                            var result = await _processor.ProcessAsync(consumeContext, token).ConfigureAwait(false);

                            if (result != null && result.HasValue && _writer != null)
                            {
                                await _writer.WriteAsync(result.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _onError(ex);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        public INetStream<TKey, TMessage> Handle(Action<IConsumeContext<TKey, TMessage>> handle)
        {
            Func<IConsumeContext<TKey, TMessage>, object> actionWrapper = (context) =>
            {
                handle(context);
                return null; //TODO: figure out what to do with this default value
            };

            _processor.AddBehavior(new ConsumeTransformer<TKey, TMessage>(actionWrapper, this));

            return this;
        }

        public INetStream<TKey, TMessage> HandleAsync(Func<IConsumeContext<TKey, TMessage>, Task> handle)
        {
            Func<IConsumeContext<TKey, TMessage>, Task<object>> actionWrapper = async (context) =>
            {
                await handle(context);
                return new None(); //TODO: figure out what to do with this default value
            };

           _processor.AddBehavior(new AsyncConsumeTransformer<TKey, TMessage>(actionWrapper, this));

            return this;
        }

        public INetStream<TKey, TMessage> Transform(Func<IConsumeContext<TKey, TMessage>, object> handle)
        {
            _processor.AddBehavior(new ConsumeTransformer<TKey, TMessage>(handle, this));
            return this;
        }

        public INetStream<TKey, TMessage> TransformAsync(Func<IConsumeContext<TKey, TMessage>, Task<object>> handle)
        {
            _processor.AddBehavior(new AsyncConsumeTransformer<TKey, TMessage>(handle, this));
            return this;
        }

        public INetStream<TKey, TMessage> Filter(Func<IConsumeContext<TKey, TMessage>, bool> filterPredicate)
        {
            _processor.AddBehavior(new FilterBehavior<TKey, TMessage>(filterPredicate));
            return this;
        }

        public INetStream SetWriter(IStreamWriter writer)
        {
            _writer = writer;
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
        ~NetStream()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
