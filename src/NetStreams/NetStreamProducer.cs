using Confluent.Kafka;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NetStreams
{
    public class NetStreamProducer<TMessage> : IMessageProducer<TMessage> where TMessage : IMessage
    {
        readonly IProducer<string, TMessage> _producer;

        public NetStreamProducer(IProducer<string, TMessage> producer)
        {
            _producer = producer;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task ProduceAsync(string topic, TMessage message)
        {
            var kafkaMessage = new Message<string, TMessage>() {
                Key = message.Key,
                Value = message,
                Headers = new Headers()
            };

            kafkaMessage.Headers.Add(new Header("Type", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName.ToString())));

            await _producer.ProduceAsync(topic, kafkaMessage);
        }
    }

    public interface IMessageProducer<TMessage>: IDisposable where TMessage : IMessage
    {
        Task ProduceAsync(string topic, TMessage message);
    }
}
