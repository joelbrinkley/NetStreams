using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using NetStreams.Configuration;
using NetStreams.Internal.Exceptions;
using NetStreams.Logging;

namespace NetStreams.Internal
{

    internal class NetStream<TKey, TMessage> : INetStream
    {
        public NetStreamStatus Status { get; private set; } = NetStreamStatus.NotStarted;
        readonly IConsumePipeline<TKey, TMessage> _pipeline;
        readonly string _name;
        readonly ITopicCreator _topicCreator;
        readonly ILog _log;
        readonly string _topic;
        readonly NetStreamConfiguration<TKey, TMessage> _configuration;
        readonly IConsumer<TKey, TMessage> _consumer;
        readonly INetStreamTelemetryClient _telemetryClient;

        bool disposedValue;
        Action<Exception> _onError { get; } = exception => { };

        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStream(
            string topic,
            NetStreamConfiguration<TKey, TMessage> configuration,
            IConsumer<TKey, TMessage> consumer,
            ITopicCreator topicCreator,
            INetStreamTelemetryClient telemetryClient = null,
            IConsumePipeline<TKey, TMessage> pipeline = null,
            Action<Exception> onError = null,
            string name = null)
        {
            _configuration = configuration;
            _topic = topic;
            _consumer = consumer;
            _topicCreator = topicCreator;
            _telemetryClient = telemetryClient ?? new NoOpTelemetryClient();
            _pipeline = pipeline ?? new ConsumePipeline<TKey, TMessage>();
            _name = name ?? $"{topic}:{configuration.ConsumerGroup}";
            if (onError != null) _onError = onError;
        }

        public Task StartAsync(CancellationToken token)
        {
            _telemetryClient.SendAsync(new StreamStarted(_name, _topic, Configuration), token).Wait();

            Status = NetStreamStatus.Running;

            if (Configuration.TopicCreationEnabled)
            {
                _topicCreator.CreateAll(Configuration.TopicConfigurations).Wait(token);
            }

            _consumer.Subscribe(_topic);

            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested && Status != NetStreamStatus.Stopped)
                {
                    ConsumeResult<TKey, TMessage> consumeResult = null;
                    try
                    {
                        consumeResult = _consumer.Consume(100);
                        await ProcessMessageAsync(consumeResult, token);
                    }
                    catch (ConsumeException ce) when (ce.InnerException is MalformedMessageException && _configuration.ShouldSkipMalformedMessages)
                    {
                        _consumer.Commit();
                        await _telemetryClient.SendAsync(new MalformedMessageSkipped(_name), token);
                    }
                    catch (Exception ex)
                    {
                        var consumeContext = consumeResult == null ? null : new ConsumeContext<TKey, TMessage>(consumeResult, _consumer, Configuration.ConsumerGroup);
                        await _telemetryClient.SendAsync(new NetStreamExceptionOccurred<TKey, TMessage>(_name, ex, consumeContext), token);
                        _onError(ex);
                        if (!_configuration.ContinueOnError)
                        {
                            await ResetOffsetAsync(consumeResult, token);
                        }
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        private async Task ResetOffsetAsync(ConsumeResult<TKey, TMessage> consumeResult, CancellationToken token)
        {
            if (consumeResult != null)
            {
                await _telemetryClient.SendAsync(new ResetOffset(_name), token);
                _consumer.Seek(consumeResult.TopicPartitionOffset);
            }
        }

        public async Task StopAsync(CancellationToken token)
        {
            Status = NetStreamStatus.Stopped;
            await _telemetryClient.SendAsync(new StreamStopped(_name), token);
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

        private async Task ProcessMessageAsync(ConsumeResult<TKey, TMessage> consumeResult, CancellationToken token)
        {
            if (consumeResult != null)
            {
                var consumeContext = new ConsumeContext<TKey, TMessage>(consumeResult, _consumer, Configuration.ConsumerGroup);

                await _telemetryClient.SendAsync(new MessageProcessingStarted<TKey, TMessage>(_name, consumeContext), token);

                await _pipeline.ExecuteAsync(consumeContext, token).ConfigureAwait(false);

                await _telemetryClient.SendAsync(new MessageProcessingCompleted<TKey, TMessage>(_name, consumeContext), token);
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
