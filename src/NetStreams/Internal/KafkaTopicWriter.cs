using System;
using System.Threading.Tasks;

namespace NetStreams.Internal
{

    internal class KafkaTopicWriter<TResponseKey, TResponse> : IStreamWriter<TResponseKey, TResponse>
    {
        readonly IMessageProducer<TResponseKey, TResponse> _producer;
        readonly Func<TResponse, TResponseKey> _resolveKey;

        public KafkaTopicWriter(
            IMessageProducer<TResponseKey, TResponse> producer,
            Func<TResponse, TResponseKey> resolveKey)
        {
            if (resolveKey == null) resolveKey = response => default;

            _resolveKey = resolveKey;
            _producer = producer;
        }

        public async Task WriteAsync(TResponse message)
        {
            await _producer.ProduceAsync(_resolveKey(message), message);
        }

        public async Task WriteAsync(object message)
        {
            await this.WriteAsync((TResponse)message);
        }
    }
}
