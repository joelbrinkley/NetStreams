using NetStreams.Internal.Exceptions;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class KafkaTopicWriter<TMessage, TResponse> : IStreamWriter<TMessage, TResponse>
        where TMessage : IMessage
        where TResponse : IMessage
    {
        readonly INetStream<TMessage> _stream;
        readonly IMessageProducer<TResponse> _producer;
        readonly IHandle<TMessage, TResponse> _handle;
        string _topic;

        public KafkaTopicWriter(
            string topic,
            IProducerFactory producerFactory,
            IHandle<TMessage, TResponse> handle)
        {
            _topic = topic;
            _stream = handle.Stream;
            _handle = handle;
            _producer = producerFactory.Create<TResponse>(_stream.Configuration);
        }

        public async Task WriteAsync(TResponse message)
        {
            if (string.IsNullOrEmpty(_topic))
            {
                throw new NullOrEmptyTopicException(_topic);
            }
     
            await _producer.ProduceAsync(_topic, message);
        }

        public INetStream<TMessage> To(string topic)
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
