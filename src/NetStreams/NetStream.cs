﻿using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public class NetStream<TKey, TMessage> : INetStream
    {
        readonly IConsumeProcessor<TKey, TMessage> _processor;
        readonly ITopicCreator _topicCreator;
        readonly string _topic;
        readonly NetStreamConfiguration<TKey, TMessage> _configuration;
        readonly IConsumer<TKey, TMessage> _consumer;
        bool disposedValue;
        public Action<Exception> OnError { get; set; } = exception => { };
        Task _streamTask;

        public INetStreamConfigurationContext Configuration => _configuration;

        public NetStream(
            string topic,
            NetStreamConfiguration<TKey, TMessage> configuration,
            IConsumer<TKey, TMessage> consumer,
            ITopicCreator topicCreator,
            IConsumeProcessor<TKey, TMessage> processor = null)
        {
            _configuration = configuration;
            _topic = topic;
            _consumer = consumer;
            _topicCreator = topicCreator;
            _processor = processor ?? new ConsumeProcessor<TKey, TMessage>();
        }

        public Task StartAsync(CancellationToken token)
        {
            if (Configuration.TopicCreationEnabled)
            {
                _topicCreator.CreateAll(Configuration.TopicConfigurations).Wait(token);
            }
            
            _consumer.Subscribe(_topic);

            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(100);

                        var consumeContext = new ConsumeContext<TKey, TMessage>(consumeResult, _consumer, Configuration.ConsumerGroup);

                        if (consumeResult != null)
                        {
                            await _processor.ProcessAsync(consumeContext, token).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
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
