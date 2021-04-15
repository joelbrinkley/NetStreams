using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Serialization;

namespace NetStreams.Specs.Infrastructure.Services
{
    public class TestProducerService<TKey, TMessage>
    {
        readonly string _topic;
        IMessageProducer<TKey, TMessage> _producer;

        public TestProducerService(string topic)
        {
            var kafkaProducer = new ProducerBuilder<TKey, TMessage>(new ProducerConfig() { BootstrapServers = "localhost:9092" })
                .SetValueSerializer(new HeaderSerializationStrategy<TMessage>())
                .Build();

            _producer = new NetStreamProducer<TKey, TMessage>(kafkaProducer);
            _topic = topic;
        }

        public void Produce(TKey key, TMessage message)
        {
            _producer.ProduceAsync(_topic, key,  message).Await();
        }

        public async Task ProduceAsync(TKey key, TMessage message)
        {
            await _producer.ProduceAsync(_topic, key, message);
        }
    }
}
