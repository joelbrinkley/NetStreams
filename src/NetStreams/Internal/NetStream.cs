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
        readonly ITopicCreator _topicCreator;
        readonly ILog _log;
        readonly string _topic;
        readonly NetStreamConfiguration<TKey, TMessage> _configuration;
        readonly IConsumer<TKey, TMessage> _consumer;
        bool disposedValue;
        Action<Exception> _onError { get; } = exception => { };

        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStream(
            string topic,
            NetStreamConfiguration<TKey, TMessage> configuration,
            IConsumer<TKey, TMessage> consumer,
            ITopicCreator topicCreator,
            ILog log,
            IConsumePipeline<TKey, TMessage> pipeline = null,
            Action<Exception> onError = null)
        {
            _configuration = configuration;
            _topic = topic;
            _consumer = consumer;
            _topicCreator = topicCreator;
            _log = log;
            _pipeline = pipeline ?? new ConsumePipeline<TKey, TMessage>();
            if (onError != null) _onError = onError;
        }

        public Task StartAsync(CancellationToken token)
        {
            _log.Information($"Starting stream for topic {_topic}");

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
                        _log.Error(ce, $"A malformed message was encountered on topic { _topic}. Skipping message. Skipping offset {ce.ConsumerRecord.Offset} on partition {ce.ConsumerRecord.Partition.Value}");
                        _consumer.Commit();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, $"An error occurred processing messages from topic {_topic}");
                        _onError(ex);
                        if (!_configuration.ContinueOnError)
                        {
                            ResetOffset(consumeResult);
                        }
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        private void ResetOffset(ConsumeResult<TKey, TMessage> consumeResult)
        {
            if (consumeResult != null)
            {
                _log.Debug($"Resetting offset to topic: {consumeResult.TopicPartitionOffset.Topic}, partition:{consumeResult.TopicPartition.Partition}, offset: {consumeResult.TopicPartitionOffset.Offset}");
                _consumer.Seek(consumeResult.TopicPartitionOffset);
            }
        }

        public void Stop()
        {
            Status = NetStreamStatus.Stopped;
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

                _log.Debug($"Begin consuming offset {consumeContext.Offset} on partition {consumeContext.Partition} ");
                await _pipeline.ExecuteAsync(consumeContext, token).ConfigureAwait(false);
                _log.Debug($"Finished consuming offset {consumeContext.Offset} on partition {consumeContext.Partition} ");
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
