using Confluent.Kafka;
using System;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Internal;

namespace NetStreams
{
    public class NetStreamProducer<TKey, TMessage> : IMessageProducer<TKey, TMessage>
    {
        readonly string _topic;
        readonly IProducer<TKey, TMessage> _producer;

        public NetStreamProducer(string topic, IProducer<TKey, TMessage> producer)
        {
            _topic = topic;
            _producer = producer;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task ProduceAsync(TKey key, TMessage message)
        {
            var kafkaMessage = new Message<TKey, TMessage>()
            {
                Key = key,
                Value = message,
                Headers = new Headers()
            };

            kafkaMessage.Headers.Add(new Header(NetStreamConstants.HEADER_TYPE,
                Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName.ToString())));

            await _producer.ProduceAsync(_topic, kafkaMessage);
        }
    }

    public interface IMessageProducer<TKey, TMessage> : IDisposable
    {
        Task ProduceAsync(TKey key, TMessage message);
    }

}
