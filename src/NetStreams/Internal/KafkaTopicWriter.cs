using System;
using NetStreams.Internal.Exceptions;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
   
    internal class KafkaTopicWriter<TResponseKey, TResponse> : IStreamWriter<TResponseKey, TResponse>
    {
        readonly IMessageProducer<TResponseKey, TResponse> _producer;
        readonly INetStream _stream;
        string _topic;
        readonly Func<TResponse, TResponseKey> _resolveKey;

        public KafkaTopicWriter(
            string topic,
            IProducerFactory producerFactory,
            INetStream stream,
            Func<TResponse, TResponseKey> resolveKey)
        {
            if (resolveKey == null) resolveKey = response => default;

            _topic = topic;
            _resolveKey = resolveKey;
            _stream = stream;
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

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
