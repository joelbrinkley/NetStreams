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

        public async Task WriteAsync(NetStreamResult<TResponse> result)
        {
            await _producer.ProduceAsync(_resolveKey(result.Message), result.Message, result.Headers);
        }

        public async Task WriteAsync(NetStreamResult result)
        {
            await this.WriteAsync(new NetStreamResult<TResponse>(result.Message, result.Headers));
        }
    }
}
