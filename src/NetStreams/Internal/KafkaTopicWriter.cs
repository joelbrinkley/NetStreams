using System;
using NetStreams.Internal.Exceptions;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class KafkaTopicWriter<TResponseKey, TResponse> : IStreamWriter<TResponseKey, TResponse>
    {
        readonly INetStream _stream;
        readonly IMessageProducer<TResponseKey, TResponse> _producer;
        string _topic;
        readonly Func<TResponse, TResponseKey> _resolveKey;

        public KafkaTopicWriter(
            string topic,
            IProducerFactory producerFactory,
            IConsumeBehavior consumeBehavior,
            Func<TResponse, TResponseKey> resolveKey)
        {
            if (resolveKey == null) resolveKey = response => default;

            _topic = topic;
            _resolveKey = resolveKey;
            _stream = consumeBehavior.Stream;
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

        public async Task WriteAsync(object message)
        {
            await this.WriteAsync((TResponse)message);
        }

        public INetStream To(string topic)
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
