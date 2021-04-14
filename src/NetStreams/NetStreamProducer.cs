using Confluent.Kafka;
using System;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Internal;

namespace NetStreams
{
    public class NetStreamProducer<TKey, TMessage> : IMessageProducer<TKey, TMessage>
    {
        readonly IProducer<TKey, TMessage> _producer;

        public NetStreamProducer(IProducer<TKey, TMessage> producer)
        {
            _producer = producer;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task ProduceAsync(string topic, TKey key, TMessage message)
        {
            var kafkaMessage = new Message<TKey, TMessage>() {
                Key = key,
                Value = message,
                Headers = new Headers()
            };

            kafkaMessage.Headers.Add(new Header(Constants.HEADER_TYPE, Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName.ToString())));

            await _producer.ProduceAsync(topic, kafkaMessage);
        }
    }

    public interface IMessageProducer<TKey, TMessage> : IDisposable
    {
        Task ProduceAsync(string topic, TKey key, TMessage message);
    }
}
