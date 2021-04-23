using System;
using NetStreams.Internal.Exceptions;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class KafkaTopicWriter<TKey, TMessage, TResponseKey, TResponse> : IStreamWriter<TKey, TMessage, TResponseKey, TResponse>
    {
        readonly INetStream<TKey, TMessage> _stream;
        readonly IMessageProducer<TResponseKey, TResponse> _producer;
        string _topic;
        readonly Func<TResponse, TResponseKey> _resolveKey;

        public KafkaTopicWriter(
            string topic,
            IProducerFactory producerFactory,
            IHandle<TKey, TMessage, TResponseKey, TResponse> handle,
            Func<TResponse, TResponseKey> resolveKey)
        {
            if (resolveKey == null) resolveKey = response => default;

            _topic = topic;
            _resolveKey = resolveKey;
            _stream = handle.Stream;
            _producer = producerFactory.Create<TResponseKey, TResponse>(_topic, _stream.Configuration);
        }

        public async Task WriteAsync(TResponse message)
        {
            if (string.IsNullOrEmpty(_topic))
            {
                throw new NullOrEmptyTopicException(_topic);
            }

            await _producer.ProduceAsync(_resolveKey(message), message);
        }

        public INetStream<TKey, TMessage> To(string topic)
        {
            this._topic = topic;
            return _stream;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
